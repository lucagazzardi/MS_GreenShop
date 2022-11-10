using Common;
using RabbitMQService.cs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService.cs.EventsCollection
{
    public class SendNotificationEvent : Event
    {
        public Dictionary<string, string> Fields { get; set; }

        public Dictionary<string, string> Emails { get; set; }
        public CommonElements.NotificationDictionary NotificationType { get; set; }

        public SendNotificationEvent(Dictionary<string,string> fields, Dictionary<string, string> emails, CommonElements.NotificationDictionary notificationType)
        {
            Fields = fields;
            Emails = emails;
            NotificationType = notificationType;
        }
    }
}
