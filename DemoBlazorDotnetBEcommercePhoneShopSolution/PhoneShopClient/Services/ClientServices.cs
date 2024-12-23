using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhoneShopClient.Services
{
    public class ClientServices(HttpClient httpClient) : IProductService
    {
        private const string BaseUrl = "api/product";
        private static string SerializedObj(object modelObj) => JsonSerializer.Serialize(modelObj, JsonOptions());
        private static T DeserializeJsonString<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString, JsonOptions())!;
        private static StringContent GenerateStringContent(string serializedObj) => new(serializedObj, System.Text.Encoding.UTF8, "application/json");
        private static IList<T> DeserializeJsonStringList<T>(string jsonString) => JsonSerializer.Deserialize<IList<T>>(jsonString, JsonOptions())!;
        private static JsonSerializerOptions JsonOptions()
        {   
            return new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
            };
        }

        public async Task<ServiceResponse> AddProductAsync(Product product)
        {
            var response = await httpClient.PostAsync(BaseUrl, GenerateStringContent(SerializedObj(product)));
            if(!response.IsSuccessStatusCode)
                return new ServiceResponse(false, "Error occured. Try again later...");
            var apiResponse = await response.Content.ReadAsStringAsync();
            return DeserializeJsonString<ServiceResponse>(apiResponse);
        }

        public async Task<List<Product>> GetAllProductsAsync(bool featuredProducts)
        {
            var response = await httpClient.GetAsync($"{BaseUrl}?featured={featuredProducts}");
            if (!response.IsSuccessStatusCode)
                return null!;
            var result = await response.Content.ReadAsStringAsync();
            return [.. DeserializeJsonStringList<Product>(result)];
        }
    }
}
