using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopServer.Repositories
{
    public interface IProduct
    {
        Task<ServiceResponse> AddProductAsync(Product product);
        Task<List<Product>> GetAllProductsAsync(bool featuredProducts);
    }
}
