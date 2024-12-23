using Microsoft.EntityFrameworkCore;
using PhoneShopServer.Data;
using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopServer.Repositories
{
    public class CategoryRepository(ApplicationDBContext context) : ICategory
    {
        private readonly ApplicationDBContext _dbContext = context;

        public async Task<ServiceResponse> AddCategoryAsync(Category category)
        {
            if (category is null) return new ServiceResponse(false, "Category is null");
            var (flag, message) = await CheckNameAsync(category.Name!);
            if (flag)
            {
                _dbContext.Categories.Add(category);
                await Commit();
                return new ServiceResponse(true, "Category Saved");
            }
            return new ServiceResponse(flag, message);
        }

        public async Task<List<Category>> GetAllCategoriesAsync() => await _dbContext.Categories.ToListAsync();

        private async Task<ServiceResponse> CheckNameAsync(string name)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name!.ToLower()!.Equals(name.ToLower()));
            return category is null ? new ServiceResponse(true, null!) : new ServiceResponse(false, "Category is already exist");
        }
        private async Task Commit() => await _dbContext.SaveChangesAsync();

    }
}
