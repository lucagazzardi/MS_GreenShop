using Catalog.API.Model;
using Catalog.API.Model.Others;

namespace Catalog.API.Business.Interfaces
{
    public interface ICatalogService
    {
        List<Product> GetCatalog();
        void AddNewProduct(ProductAddEdit product);
        void EditProduct(int productId, ProductAddEdit product);
        void RemoveFromCatalog(int productId);
        void AddToCatalog(int productId);
    }
}
