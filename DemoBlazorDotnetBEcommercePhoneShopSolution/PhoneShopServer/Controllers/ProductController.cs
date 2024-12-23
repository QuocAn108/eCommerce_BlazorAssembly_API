using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneShopShareLibrary.Interface;
using PhoneShopShareLibrary.Models;

namespace PhoneShopServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productService) : ControllerBase
    {
        private readonly IProduct _productService = productService;

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
