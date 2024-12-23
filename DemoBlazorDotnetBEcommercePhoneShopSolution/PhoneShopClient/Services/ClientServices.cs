using PhoneShopShareLibrary.Models;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopClient.Services
{
    public class ClientServices(HttpClient httpClient) : IProductService, ICategoryService
    {
        private const string ProductBaseUrl = "api/product";
        private const string CategoryBaseUrl = "api/category";

        public Action? CategoryAction { get; set; }
        public List<Category> AllCategories { get; set; }
        public Action? ProductAction { get; set; }
        public List<Product> AllProducts { get; set; }
        public List<Product> FeaturedProducts { get; set; }


        //product

        public async Task<ServiceResponse> AddProductAsync(Product product)
        {
            var response = await httpClient.PostAsync(ProductBaseUrl, General.GenerateStringContent(General.SerializedObj(product)));

            var result = CheckResponse(response);
            if (!result.flag) return result;

            var apiResponse = await ReadContent(response);
            await ClearAndGetAllProduct();
            return General.DeserializeJsonString<ServiceResponse>(apiResponse);
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
            await ClearAndGetAllCategory();
            return General.DeserializeJsonString<ServiceResponse>(apiResponse);
        }

        public async Task GetAllCategoriesAsync()
        {
            if (AllCategories != null)
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
        private async Task<String> ReadContent(HttpResponseMessage response) => await response.Content.ReadAsStringAsync();

        private ServiceResponse CheckResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return new ServiceResponse(false, "Error occured. Try again later...");
            else return new ServiceResponse(true, null!);
        }
    }
}
