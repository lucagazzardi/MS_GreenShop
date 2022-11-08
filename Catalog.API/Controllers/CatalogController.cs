using Catalog.API.Business.Interfaces;
using Catalog.API.Data;
using Catalog.API.Model;
using Catalog.API.Model.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [Authorize(Roles = "Customer, Administrator")]
        [HttpGet("list")]
        public IActionResult GetCatalog()
        {            
            return Ok(_catalogService.GetCatalog());
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("product/new")]
        public IActionResult AddNewProduct(ProductAddEdit product)
        {
            _catalogService.AddNewProduct(product);
            return Ok();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("product/edit/{productId:int}")]
        public IActionResult EditProduct(int productId, ProductAddEdit product)
        {
            _catalogService.EditProduct(productId, product);
            return Ok();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("product/remove/{productId:int}")]
        public IActionResult RemoveFromCatalog(int productId)
        {
            _catalogService.RemoveFromCatalog(productId);
            return Ok();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("product/add/{productId:int}")]
        public IActionResult AddToCatalog(int productId)
        {
            _catalogService.AddToCatalog(productId);
            return Ok();
        }
    }
}
