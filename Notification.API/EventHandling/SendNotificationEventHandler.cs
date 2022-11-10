using Notification.API.Business;
using RabbitMQService.cs.EventsCollection;
using RabbitMQService.cs.Infrastructure.Interfaces;

namespace Notification.API.EventHandling
{
    public class SendNotificationEventHandler : IEventHandler<SendNotificationEvent>
    {
        private readonly INotificationService _notificationService;

        public SendNotificationEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(SendNotificationEvent @event)
        {
            System.Diagnostics.Debug.WriteLine($"Arrivato evento {@event.Id}, {@event.CreationDate}");

            await _notificationService.MockSendNotification(@event.NotificationType, @event.Emails, @event.Fields);
        }
    }
}
