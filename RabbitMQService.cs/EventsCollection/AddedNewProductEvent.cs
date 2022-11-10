using RabbitMQService.cs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService.cs.EventsCollection
{
    public class AddedNewProductEvent : Event
    {
        public int CategoryId { get; init; }
        public string CategoryName { get; init; }
        public string ProductName { get; init; }

        public AddedNewProductEvent(int categoryId, string categoryName, string productName)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            ProductName = productName;
        }
    }
}
