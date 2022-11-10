using MongoDB.Bson.Serialization.Attributes;

namespace Notification.API.Model
{
    public class NotificationHistory
    {
        [BsonId]
        public Guid Id { get; set; }
        public string NotificationType { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
    }
}
