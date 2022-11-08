namespace Catalog.API.Model.Others
{
    public class ProductAddEdit
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string Pic { get; set; }
        public bool InCatalog { get; set; }
    }
}
