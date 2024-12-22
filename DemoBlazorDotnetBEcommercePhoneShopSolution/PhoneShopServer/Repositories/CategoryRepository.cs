//using Microsoft.EntityFrameworkCore;
//using PhoneShopServer.Data;
//using PhoneShopShareLibrary.Models;
//using PhoneShopShareLibrary.Responses;

//namespace PhoneShopServer.Repositories
//{
//    public class CategoryRepository(ApplicationDBContext context) : ICategory
//    {
//        private readonly ApplicationDBContext _dbContext = context;

//        public async Task<ServiceResponse> AddCategoryAsync(Category category)
//        {
//            if (category is null) return new ServiceResponse(false, "Category is null");
//            var (flag, message) = await CheckNameAsync(category.Name!);
//            if (flag)
//            {
//                _dbContext.Categories.Add(category);
//                await Commit();
//                return new ServiceResponse(true, "Category Saved");
//            }
//            return new ServiceResponse(flag, message);
//        }

//        public async Task<List<Product>> GetAllCategoriesAsync(

//        private async Task<ServiceResponse> CheckNameAsync(string name)
//        {
//            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Name.ToLower()!.Equals(name.ToLower()));
//            return product is null ? new ServiceResponse(true, null!) : new ServiceResponse(false, "Product is already exist");
//        }
//        private async Task Commit() => await _dbContext.SaveChangesAsync();
//    }
//}
