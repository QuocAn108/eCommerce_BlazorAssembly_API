using PhoneShopShareLibrary.Interface;
using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopClient.Services
{
    public class ClientServices(HttpClient httpClient) : IProduct
    {
        private const string BaseUrl = "api/product";
        public Task<ServiceResponse> AddProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> GetAllProductsAsync(bool featuredProducts)
        {
            throw new NotImplementedException();
        }
    }
}
