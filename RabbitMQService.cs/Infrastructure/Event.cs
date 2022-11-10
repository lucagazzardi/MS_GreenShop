using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService.cs.Infrastructure
{
    public class Event
    {
        public Guid Id { get; private init; } = Guid.NewGuid();
        public DateTime CreationDate { get; private init; } = DateTime.UtcNow;
    }
}
