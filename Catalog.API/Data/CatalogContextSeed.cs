using Catalog.API.Model;

namespace Catalog.API.Data
{
    public class CatalogContextSeed
    {
        public void Seed(CatalogContext context)
        {
            context.Database.EnsureCreated();

            if (context.Products.Any())
                return;

            context.Categories.AddRange(PopulateMockCategories());
            context.SaveChanges();

            context.Products.AddRange(PopulateMockProducts());
            context.SaveChanges();
        }

        private List<Category> PopulateMockCategories()
        {
            return new List<Category>()
            {
                new Category()
                {
                    Name = "Estive"
                },
                new Category()
                {
                    Name = "Sempreverdi"
                }
            };
        }
        private List<Product> PopulateMockProducts()
        {
            return new List<Product>()
            {
                new Product()
                {
                    Name = "Margherita",
                    Description = "Una bellissima margherita",
                    Pic = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/Fleur_en_Vanoise_%2821%29.JPG/1280px-Fleur_en_Vanoise_%2821%29.JPG",
                    CategoryId = 1
                },
                new Product()
                {
                    Name = "Pino",
                    Description = "Ottimo per gli alberi di Natale",
                    Pic = "https://www.picturethisai.com/wiki-image/1080/214256762847002624.jpeg",
                    CategoryId = 2
                }
            };
        }
    }
}
