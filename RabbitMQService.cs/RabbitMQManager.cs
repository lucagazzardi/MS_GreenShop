using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQService.cs.Infrastructure;
using RabbitMQService.cs.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Polly.Retry;
using Polly;

namespace RabbitMQService.cs
{
    public class RabbitMQManager : IRabbitMQManager
    {
        const string BROKER_NAME = "msgreenshop_event_bus";

        private readonly IRabbitMQConnection _connection;
        private readonly IEventSubscriptionManager _subsManager;
        private readonly IServiceProvider _spProvider;
        private readonly ILogger<RabbitMQManager> _logger;

        private IModel _consumerChannel;
        private string _queueName;
        private int _retryCount = 5;

        public RabbitMQManager(IRabbitMQConnection connection, IEventSubscriptionManager subsManager, IServiceProvider spProvider, ILogger<RabbitMQManager> logger, string queueName = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _subsManager = subsManager ?? new SubscriptionManager();
            _queueName = queueName;
            _spProvider = spProvider;
            _consumerChannel = CreateConsumerChannel();
            
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using var channel = _connection.CreateModel();
            channel.QueueUnbind(queue: _queueName,
                exchange: BROKER_NAME,
                routingKey: eventName);

            if (_subsManager.IsEmpty)
            {
                _queueName = string.Empty;
                _consumerChannel.Close();
            }
        }

        public void Publish(Event @event)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
            });

            var eventName = @event.GetType().Name;

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

            using var channel = _connection.CreateModel();
            _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });

            System.Diagnostics.Debug.WriteLine($"Sto per eseguire il publish di {@event.Id}, {@event.CreationDate}");

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                channel.BasicPublish(
                    exchange: BROKER_NAME,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);

            _logger.LogInformation("Subscribing to event {EventName}", eventName);

            _subsManager.AddSubscription<T, TH>();
            InitializeConsume();
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_connection.IsConnected)
                {
                    _connection.TryConnect();
                }

                _consumerChannel.QueueBind(queue: _queueName,
                                    exchange: BROKER_NAME,
                                    routingKey: eventName);
            }
        }

        public void Unsubscribe<T, TH>()
        where T : Event
        where TH : IEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subsManager.RemoveSubscription<T, TH>();
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }

            _subsManager.Clear();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _connection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME,
                                    type: "direct");

            channel.QueueDeclare(queue: _queueName,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                InitializeConsume();
            };

            return channel;
        }

        private void InitializeConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }
                        
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _spProvider.CreateAsyncScope())
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                        if (handler == null) continue;
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            }
        }
    }
}