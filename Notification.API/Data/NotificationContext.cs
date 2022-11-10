using Common;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Notification.API.Model;

namespace Notification.API.Data
{
    public class NotificationContext
    {
        private readonly IMongoDatabase _notificationContext = null;
        public NotificationContext(IOptions<NotificationSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
            {
                _notificationContext = client.GetDatabase(settings.Value.DatabaseName);

                // SEED
                PopulateIfEmpty();
            }
        }   
        
        public async Task<List<NotificationTemplate>> GetAsync() =>
            await NotificationTemplates.Find(_ => true).ToListAsync();

        public IMongoCollection<NotificationTemplate> NotificationTemplates
        {
            get
            {
                return _notificationContext.GetCollection<NotificationTemplate>("NotificationTemplate");
            }
        }

        public IMongoCollection<NotificationHistory> NotificationHistories
        {
            get
            {
                return _notificationContext.GetCollection<NotificationHistory>("NotificationHistory");
            }
        }

        public void PopulateIfEmpty()
        {
            if (!NotificationTemplates.Find(p => true).Any<NotificationTemplate>())
            {
                NotificationTemplates.InsertMany(PopulateMockTemplates());
            }

            _notificationContext.CreateCollection("NotificationHistory");
        }

        private List<NotificationTemplate> PopulateMockTemplates()
        {
            return new List<NotificationTemplate>()
            {
                new NotificationTemplate()
                {
                    Id = Guid.NewGuid(),
                    Text = "A new product has been added in your followed category \"[Category]\": \"[Product]\"",
                    NotificationType = CommonElements.NotificationDictionary.NewProductInCategory.ToString()
                }
            };
        }
    }
}
