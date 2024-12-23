using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopServer.Repositories
{
    public interface ICategory
    {
        Task<ServiceResponse> AddCategoryAsync(Category category);
        Task<List<Category>> GetAllCategoriesAsync();
    }
}
