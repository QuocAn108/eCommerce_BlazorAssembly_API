using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopClient.Services
{
    public interface IProductService
    {
        Action? ProductAction { get; set; }
        Task<ServiceResponse> AddProductAsync(Product product);
        Task GetAllProductsAsync(bool featuredProducts);
        List<Product> AllProducts { get; set; }

        List<Product> FeaturedProducts { get; set; }
    }
}
