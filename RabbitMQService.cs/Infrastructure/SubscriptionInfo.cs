using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService.cs.Infrastructure
{
    public partial class SubscriptionManager
    {
        public class SubscriptionInfo
        {
            public Type HandlerType { get; }

            public SubscriptionInfo(Type handlerType)
            {
                HandlerType = handlerType;
            }
        }        
    }
}
