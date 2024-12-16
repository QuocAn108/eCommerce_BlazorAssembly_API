using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PhoneShopClient;
using PhoneShopClient.Services;
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IProductService,ClientServices>();
builder.Services.AddSyncfusionBlazor();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzYxMzc4OUAzMjM3MmUzMDJlMzBkdDhCWlhmd05RbWE0NG5aSkNxKy9HNFZFYWN3RGJLSVVydko4Q2dqaFp3PQ==");
await builder.Build().RunAsync();
