using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneShopServer.Repositories;
using PhoneShopShareLibrary.Models;

namespace PhoneShopServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProduct _productService;
        public ProductController(IProduct productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProduct(bool featured)
        {
            var product = await _productService.GetAllProductsAsync(featured);
            return Ok(product);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (product is null) return BadRequest("Product is null");
            var response = await _productService.AddProductAsync(product);
            return Ok(response);
        }
    }
}
