using Common;

namespace Notification.API.Business
{
    public interface INotificationService
    {
        Task MockSendNotification(CommonElements.NotificationDictionary notificationType, Dictionary<string, string> emails, Dictionary<string, string> fields);
    }
}
