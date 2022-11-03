using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Model
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string Pic { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}
