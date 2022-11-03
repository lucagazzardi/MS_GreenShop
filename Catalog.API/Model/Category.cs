namespace Catalog.API.Model
{
    public class Category
    {
        public int ID { get; set; }        
        public string Name { get; set; }

        List<Product> Products { get; set; }
    }
}
