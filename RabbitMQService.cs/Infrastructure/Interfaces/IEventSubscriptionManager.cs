using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RabbitMQService.cs.Infrastructure.SubscriptionManager;

namespace RabbitMQService.cs.Infrastructure.Interfaces
{
    public interface IEventSubscriptionManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>()
            where T : Event
            where TH : IEventHandler<T>;

        void RemoveSubscription<T, TH>()
            where TH : IEventHandler<T>
            where T : Event;

        bool HasSubscriptionsForEvent<T>() where T : Event;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : Event;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}
