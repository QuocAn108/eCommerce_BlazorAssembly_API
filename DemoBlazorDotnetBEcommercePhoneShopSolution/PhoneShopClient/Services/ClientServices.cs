using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PhoneShopClient.Services
{
    public class ClientServices(HttpClient httpClient) : IProductService, ICategoryService
    {
        private const string ProductBaseUrl = "api/Product";
        private const string CategoryBaseUrl = "api/Category";

        public Action? CategoryAction { get; set; }
        public List<Category> AllCategories { get; set; } = new();
        public Action? ProductAction { get; set; }
        public List<Product> AllProducts { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();


        //product

        public async Task<ServiceResponse> AddProductAsync(Product product)
        {
            var response = await httpClient.PostAsync(ProductBaseUrl, General.GenerateStringContent(General.SerializedObj(product)));

            var result = CheckResponse(response);
            if (!result.flag) return result;

            var apiResponse = await ReadContent(response);
            var data = General.DeserializeJsonString<ServiceResponse>(apiResponse);
            if (!data.flag) return data;
            await ClearAndGetAllProduct();
            return data;
        }

        private async Task ClearAndGetAllProduct()
        {
            bool featuredProducts = true;
            bool allProducts = false;
            AllProducts = null!;
            FeaturedProducts = null!;
            await GetAllProductsAsync(featuredProducts);
            await GetAllProductsAsync(allProducts);
        }

        public async Task GetAllProductsAsync(bool featuredProducts)
        {
            if (featuredProducts && FeaturedProducts is null)
            {
                FeaturedProducts = await GetProducts(featuredProducts);
                ProductAction?.Invoke();
                return;
            }
            else
            {
                if (!featuredProducts && AllProducts is null)
                {
                    AllProducts = await GetProducts(featuredProducts);
                    ProductAction?.Invoke();
                    return;
                }
            }


        }
        private async Task<List<Product>> GetProducts(bool featured)
        {
            var response = await httpClient.GetAsync($"{ProductBaseUrl}?featured={featured}");
            var (flag, _) = CheckResponse(response);
            if (!flag) return null!;
            var result = await ReadContent(response);
            return (List<Product>?)General.DeserializeJsonStringList<Product>(result)!;
        }

        //category

        public async Task<ServiceResponse> AddCategoryAsync(Category category)
        {
            var response = await httpClient.PostAsync(CategoryBaseUrl, General.GenerateStringContent(General.SerializedObj(category)));

            var result = CheckResponse(response);
            if (!result.flag) return result;

            var apiResponse = await ReadContent(response);

            var data = General.DeserializeJsonString<ServiceResponse>(apiResponse);
            if (!data.flag) return data;
            await ClearAndGetAllCategory();
            return data;
        }

        public async Task GetAllCategoriesAsync()
        {
            if (AllCategories == null)
            {
                var response = await httpClient.GetAsync($"{CategoryBaseUrl}");
                var (flag, _) = CheckResponse(response);
                if (!flag) return;
                var result = await ReadContent(response);
                AllCategories = (List<Category>?)General.DeserializeJsonStringList<Category>(result)!;
                CategoryAction?.Invoke();
            }
        }

        private async Task ClearAndGetAllCategory()
        {
            AllCategories = null!;
            await GetAllCategoriesAsync();
        }
        //General method
        private async Task<string> ReadContent(HttpResponseMessage response) => await response.Content.ReadAsStringAsync();

        private ServiceResponse CheckResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return new ServiceResponse(false, "Error occured. Try again later...");
            else return new ServiceResponse(true, null!);
        }
    }
}
