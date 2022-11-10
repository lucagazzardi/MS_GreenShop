using Common;
using RabbitMQService.cs;
using RabbitMQService.cs.EventsCollection;
using RabbitMQService.cs.Infrastructure.Interfaces;
using User.API.Repository;

namespace User.API.EventHandling
{
    public class AddedNewProductEventHandler : IEventHandler<AddedNewProductEvent>
    {
        private readonly IUserService _userRepository;
        private readonly IRabbitMQManager _rabbitMQManager;

        public AddedNewProductEventHandler(IUserService userRepository, IRabbitMQManager rabbitMQManager)
        {
            _userRepository = userRepository;
            _rabbitMQManager = rabbitMQManager;
        }

        public async Task Handle(AddedNewProductEvent @event)
        {
            Dictionary<string,string> emailsToNotify = await _userRepository.GetEmailsToNotify(@event.CategoryId);
            Dictionary<string, string> fields = new Dictionary<string, string>() { { "Category", @event.CategoryName }, { "Product", @event.ProductName } };

            var toPublish = new SendNotificationEvent(fields, emailsToNotify, CommonElements.NotificationDictionary.NewProductInCategory);

            _rabbitMQManager.Publish(toPublish);
            System.Diagnostics.Debug.WriteLine($"Pubblicato {toPublish.Id}, {toPublish.CreationDate}");

        }
    }
}
