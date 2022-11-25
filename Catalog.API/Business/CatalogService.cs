using AutoMapper;
using Catalog.API.Business.Interfaces;
using Catalog.API.Data;
using Catalog.API.Model;
using Catalog.API.Model.Others;
using Microsoft.AspNetCore.Razor.TagHelpers;
using RabbitMQService.cs;
using RabbitMQService.cs.EventsCollection;

namespace Catalog.API.Business
{
    public class CatalogService : ICatalogService
    {
        private readonly CatalogContext _catalogContext;
        private readonly IRabbitMQManager _rabbitMQManager;
        private readonly IMapper _mapper;
        private readonly ILogger<CatalogService> _logger;

        public CatalogService(CatalogContext catalogContext, IRabbitMQManager rabbitMQManager, IMapper mapper, ILogger<CatalogService> logger)
        {
            _catalogContext = catalogContext;
            _rabbitMQManager = rabbitMQManager;
            _mapper = mapper;
            _logger = logger;
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
            _logger.LogTrace($"Added product {newProduct.Name}");

            HandleCategoryPreferences(newProduct);
        }

        public void EditProduct(int productId, ProductAddEdit product)
        {
            var toUpdate = GetProductById(productId);

            toUpdate.Name = product.Name;
            toUpdate.Description = product.Description;
            toUpdate.Pic = product.Pic;
            toUpdate.CategoryId = product.CategoryId;
            _logger.LogTrace($"Edited product {toUpdate.Name}");

            _catalogContext.Update(toUpdate);
            _catalogContext.SaveChanges();
        }

        public void RemoveFromCatalog(int productId)
        {
            var product = GetProductById(productId);
            product.InCatalog = false;
            _catalogContext.Update(product);
            _catalogContext.SaveChanges();
            _logger.LogTrace($"Product {product.Name} removed from catalog");
        }

        public void AddToCatalog(int productId)
        {
            var product = SearchForProductById(productId);
            if(product != null)
            {
                product.InCatalog = true;
                _catalogContext.Update(product);
                _catalogContext.SaveChanges();
                _logger.LogTrace($"Product {product.Name} added to catalog");
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
            Category category = _catalogContext.Categories.SingleOrDefault(x => x.ID == product.CategoryId);
            if (category != null)
            {
                AddedNewProductEvent addedNewProductEvent = new AddedNewProductEvent(product.CategoryId, category.Name, product.Name);
                _rabbitMQManager.Publish(addedNewProductEvent);
            }
        }

        #endregion
    }
}
