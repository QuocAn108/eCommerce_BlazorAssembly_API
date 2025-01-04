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
        public List<Category> AllCategories { get; set; }
        public Action? ProductAction { get; set; }
        public List<Product> AllProducts { get; set; }
        public List<Product> FeaturedProducts { get; set; }
        public List<Product> ProductsByCategory { get; set; }
        public bool IsVisible { get; set; }


        //product

        public async Task<ServiceResponse> AddProduct(Product product)
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
            await GetAllProducts(featuredProducts);
            await GetAllProducts(allProducts);
        }

        public async Task GetAllProducts(bool featuredProducts)
        {
            if (featuredProducts && FeaturedProducts is null)
            {
                IsVisible = true;
                FeaturedProducts = await GetProducts(featuredProducts);
                IsVisible = false;
                ProductAction?.Invoke();
                return;
            }
            else
            {
                if (!featuredProducts && AllProducts is null)
                {
                    IsVisible = true;
                    AllProducts = await GetProducts(featuredProducts);
                    IsVisible = false;
                    ProductAction?.Invoke();
                    return;
                }
            }

        }
        public Product GetRandomProduct()
        {
            if (FeaturedProducts is null || !FeaturedProducts.Any())
                return null!;

            Random random = new();
            return FeaturedProducts.ElementAt(random.Next(FeaturedProducts.Count));
        }
        public async Task GetProductsByCategory(int categoryId)
        {
            bool featured = false;
            await GetAllProducts(featured);
            ProductsByCategory = AllProducts.Where(p => p.CategoryId == categoryId).ToList();
            ProductAction?.Invoke();
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

        public async Task<ServiceResponse> AddCategory(Category category)
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

        public async Task GetAllCategories()
        {
            if (AllCategories is null)
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
            await GetAllCategories();
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
