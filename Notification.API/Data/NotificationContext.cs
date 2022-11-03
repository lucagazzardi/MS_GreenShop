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
                //PopulateIfEmpty();
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

        public void PopulateIfEmpty()
        {
            if (!NotificationTemplates.Find(p => true).Any<NotificationTemplate>())
            {
                NotificationTemplates.InsertMany(PopulateMockTemplates());
            }
        }

        private List<NotificationTemplate> PopulateMockTemplates()
        {
            return new List<NotificationTemplate>()
            {
                new NotificationTemplate()
                {
                    Id = Guid.NewGuid(),
                    Text = "A new product has been added in your followed category \"[1]\": \"[2]\"",
                    NotificationType = NotificationType.NewProductInCategory.ToString(),
                    Fields = new List<Fields>() { new Fields() { Key = 1, Value = "Category" }, new Fields() { Key = 2, Value = "Product" } }
                }
            };
        }

        private async Task CreateCategoryNameIndex()
        {
            var indexNotificationType = Builders<NotificationTemplate>.IndexKeys.Ascending(indexKey => indexKey.NotificationType);
            await NotificationTemplates.Indexes.CreateOneAsync(new CreateIndexModel<NotificationTemplate>(indexNotificationType));
        }
    }
}
