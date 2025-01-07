using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PhoneShopClient;
using PhoneShopClient.Authentication;
using PhoneShopClient.Services;
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IProductService, ClientServices>();
builder.Services.AddScoped<ICategoryService, ClientServices>();
builder.Services.AddScoped<IUserAccountService, ClientServices>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<MessageDialogService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddSyncfusionBlazor();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF1cX2hIYVRpR2Nbek54flBPal5SVAciSV9jS3tTd0VkWXpfeXBUQWVVVw==");
builder.Services.AddBlazoredLocalStorage();
await builder.Build().RunAsync();
