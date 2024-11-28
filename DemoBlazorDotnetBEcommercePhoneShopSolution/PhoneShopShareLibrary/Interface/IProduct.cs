using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopShareLibrary.Interface
{
    public interface IProduct
    {
        Task<ServiceResponse> AddProductAsync(Product product);
        Task<List<Product>> GetAllProductsAsync(bool featuredProducts);
    }
}
