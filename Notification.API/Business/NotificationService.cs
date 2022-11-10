using Common;
using MongoDB.Driver;
using Notification.API.Data;
using Notification.API.Model;

namespace Notification.API.Business
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationContext _notificationContext;

        public NotificationService(NotificationContext notificationContext)
        {
            _notificationContext = notificationContext;
        }
        public async Task MockSendNotification(CommonElements.NotificationDictionary notificationType, Dictionary<string,string> emails, Dictionary<string, string> fields)
        {
            var template = await GetNotificationTemplate(notificationType);
            List<NotificationHistory> to = new List<NotificationHistory>();
            
            if(template != null)
            {
                string text = ReplaceTextFields(template, fields);

                foreach(var email in emails)
                {
                    NotificationHistory history = new NotificationHistory()
                    {
                        Id = Guid.NewGuid(),
                        NotificationType = notificationType.ToString(),
                        UserId = email.Key,
                        Text = text
                    };
                    to.Add(history);
                }
                
                await _notificationContext.NotificationHistories.InsertManyAsync(to);

                System.Diagnostics.Debug.WriteLine($"Scritto evento nella history");
            }
        }

        private async Task<NotificationTemplate> GetNotificationTemplate(CommonElements.NotificationDictionary type)
        {
            return (await _notificationContext.NotificationTemplates.FindAsync(Builders<NotificationTemplate>.Filter.Eq("NotificationType", type.ToString()))).FirstOrDefault();
        }

        private string ReplaceTextFields(NotificationTemplate template, Dictionary<string, string> fields)
        {
            string text = template.Text;

            foreach(var field in fields)
            {
                text = text.Replace($"[{field.Key}]", field.Value);
            }

            return text;
        }
    }
}