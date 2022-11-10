using RabbitMQService.cs.Infrastructure;
using RabbitMQService.cs.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService.cs
{
    public interface IRabbitMQManager
    {
        void Publish(Event @event);
        void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>;
    }
}
