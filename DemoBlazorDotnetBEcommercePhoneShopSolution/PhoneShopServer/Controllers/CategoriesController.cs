using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneShopServer.Repositories;
using PhoneShopShareLibrary.Models;

namespace PhoneShopServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategory categoriesService) : ControllerBase
    {
        private readonly ICategory _categoriesService = categoriesService;

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoriesService.GetAllCategoriesAsync();
            return Ok(categories);
        }
        [HttpPost]
        public async Task<IActionResult> AddCategories(Category categories)
        {
            if (categories is null) return BadRequest("Category is null");
            var response = await _categoriesService.AddCategoryAsync(categories);
            return Ok(response);
        }
    }
}
