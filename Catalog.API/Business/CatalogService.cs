using AutoMapper;
using Catalog.API.Business.Interfaces;
using Catalog.API.Data;
using Catalog.API.Model;
using Catalog.API.Model.Others;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Catalog.API.Business
{
    public class CatalogService : ICatalogService
    {
        private readonly CatalogContext _catalogContext;
        private readonly IMapper _mapper;

        public CatalogService(CatalogContext catalogContext, IMapper mapper)
        {
            _catalogContext = catalogContext;
            _mapper = mapper;
        }

        public List<Product> GetCatalog()
        {
            return _catalogContext.Products.Where(x => x.InCatalog).ToList();
        }

        

        public void AddNewProduct(ProductAddEdit product)
        {
            Product newProduct = _mapper.Map<Product>(product);
            _catalogContext.Products.Add(newProduct);
            _catalogContext.SaveChanges();
        }

        public void EditProduct(int productId, ProductAddEdit product)
        {
            var toUpdate = GetProductById(productId);

            toUpdate.Name = product.Name;
            toUpdate.Description = product.Description;
            toUpdate.Pic = product.Pic;
            toUpdate.CategoryId = product.CategoryId;

            _catalogContext.Update(toUpdate);
            _catalogContext.SaveChanges();
        }

        public void RemoveFromCatalog(int productId)
        {
            var product = GetProductById(productId);
            product.InCatalog = false;
            _catalogContext.Update(product);
            _catalogContext.SaveChanges();
        }

        public void AddToCatalog(int productId)
        {
            var product = SearchForProductById(productId);
            if(product != null)
            {
                product.InCatalog = true;
                _catalogContext.Update(product);
                _catalogContext.SaveChanges();
            }
        }

        #region Private

        private Product GetProductById(int productId)
        {
            return _catalogContext.Products.Single(x => x.ID == productId);
        }

        private Product SearchForProductById(int productId)
        {
            return _catalogContext.Products.SingleOrDefault(x => x.ID == productId);
        }

        private void HandleCategoryPreferences(Product product)
        {
            var categoryToCheck = _catalogContext.Categories.Single(x => x.ID == product.CategoryId);

            //TODO: Enqueue Rabbit MQ for user notification
        }

        #endregion
    }
}
