using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopClient.Services
{
    public interface ICategoryService
    {
        Action? CategoryAction { get; set; }
        Task<ServiceResponse> AddCategoryAsync(Category category);
        Task GetAllCategoriesAsync();

        List<Category> AllCategories { get; set; }
    }
}
