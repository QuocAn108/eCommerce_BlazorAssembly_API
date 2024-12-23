using Microsoft.EntityFrameworkCore;
using PhoneShopServer.Data;
using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopServer.Repositories
{
    public class ProductRepository : IProduct
    {
        private readonly ApplicationDBContext _dbContext;
        public ProductRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }
        public async Task<ServiceResponse> AddProductAsync(Product product)
        {
            if (product is null) return new ServiceResponse(false, "Product is null");
            var (flag, message) = await CheckNameAsync(product.Name!);
            if (flag)
            {
                _dbContext.Products.Add(product);
                await Commit();
                return new ServiceResponse(true, "Product Saved");
            }
            return new ServiceResponse(flag, message);
        }

        public async Task<List<Product>> GetAllProductsAsync(bool featuredProducts)
        {
            if(featuredProducts)
            return await _dbContext.Products.Where(_ => _.Featured).ToListAsync();
            else return await _dbContext.Products.ToListAsync();
        }

        private async Task<ServiceResponse> CheckNameAsync(string name)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Name.ToLower()!.Equals(name.ToLower()));
            return product is null ? new ServiceResponse(true, null!) : new ServiceResponse(false, "Product is already exist");
        }
        private async Task Commit() => await _dbContext.SaveChangesAsync();
    }
}
