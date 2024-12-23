using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopClient.Services
{
    public interface IProductService
    {
        Task<ServiceResponse> AddProductAsync(Product product);
        Task<List<Product>> GetAllProductsAsync(bool featuredProducts);
    }
}
