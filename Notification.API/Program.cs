using Notification.API.Data;
using RabbitMQ.Client;
using RabbitMQService.cs.EventsCollection;
using RabbitMQService.cs.Infrastructure.Interfaces;
using RabbitMQService.cs.Infrastructure;
using RabbitMQService.cs;
using Notification.API.EventHandling;
using Notification.API.Business;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<NotificationSettings>(builder.Configuration.GetSection("NotificationDatabase"));

builder.Services.AddSingleton<NotificationContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<INotificationService, NotificationService>();

RegisterRabbitMQ();

var app = builder.Build();

CreateDbIfNotExists(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ConfigureRabbitMQ(app);

app.Run();

static void CreateDbIfNotExists(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<NotificationContext>();
            context.PopulateIfEmpty();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred creating the DB.");
        }
    }
}

void RegisterRabbitMQ()
{
    builder.Services.AddSingleton<IRabbitMQConnection>(sp =>
    {

        var factory = new ConnectionFactory()
        {
            HostName = builder.Configuration["EventBusConnection"],
            DispatchConsumersAsync = true
        };

        if (!string.IsNullOrEmpty(builder.Configuration["EventBusUserName"]))
        {
            factory.UserName = builder.Configuration["EventBusUserName"];
        }

        if (!string.IsNullOrEmpty(builder.Configuration["EventBusPassword"]))
        {
            factory.Password = builder.Configuration["EventBusPassword"];
        }

        return new DefaultRabbitMQConnection(factory);
    });

    builder.Services.AddSingleton<IRabbitMQManager, RabbitMQManager>(sp =>
    {
        var subscriptionClientName = builder.Configuration["SubscriptionClientName"];
        var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQConnection>();
        var eventBusSubscriptionsManager = sp.GetRequiredService<IEventSubscriptionManager>();

        return new RabbitMQManager(rabbitMQPersistentConnection, eventBusSubscriptionsManager, sp, subscriptionClientName);
    });

    builder.Services.AddSingleton<IEventSubscriptionManager, SubscriptionManager>();

    builder.Services.AddTransient<SendNotificationEventHandler>();
}

void ConfigureRabbitMQ(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IRabbitMQManager>();

    eventBus.Subscribe<SendNotificationEvent, SendNotificationEventHandler>();
}
