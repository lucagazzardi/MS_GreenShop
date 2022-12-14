using MongoDB.Bson.Serialization.Attributes;

namespace Notification.API.Model
{
    public class NotificationTemplate
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string NotificationType { get; set; }
    }
}
