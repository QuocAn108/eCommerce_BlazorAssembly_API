using Blazored.LocalStorage;
using Microsoft.AspNetCore.WebUtilities;
using PhoneShopClient.Services;
using PhoneShopShareLibrary.DTOs;
using PhoneShopShareLibrary.Responses;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace PhoneShopClient.Authentication
{
    public class AuthenticationService(ILocalStorageService localStorageService, HttpClient httpClient)
    {
        public async Task<UserSession> GetUserDetails()
        {
            var token = await GetTokenFromLocalStorage();
            if (string.IsNullOrEmpty(token)) return null!;

            var httpClient = await AddHeaderToHttpClient();
            var userDetails = General.DeserializeJsonString<TokenProp>(token);
            if (userDetails is null || string.IsNullOrEmpty(userDetails.Token)) return null!;
            var response = await GetUserDetailsFromApi();
            if (response.IsSuccessStatusCode)
                return await GetUserSession(response);
            if (httpClient.DefaultRequestHeaders.Contains("Authorization") && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var encodedToken = Encoding.UTF8.GetBytes(userDetails.RefreshToken!);
                var validToken = WebEncoders.Base64UrlEncode(encodedToken);
                var model = new PostRefreshTokenDTO() { RefreshToken = validToken };
                var result = await httpClient.PostAsync("api/Account/refresh-token",
                    General.GenerateStringContent(General.SerializedObj(model)));
                if (result.IsSuccessStatusCode && result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await result.Content.ReadAsStringAsync();
                    var loginResponse = General.DeserializeJsonString<LoginResponse>(apiResponse);

                    await SetTokenToLocalStorage(General.SerializedObj(new TokenProp()
                    { Token = loginResponse.token, RefreshToken = loginResponse.refreshToken }));
                    var callApiAgain = await GetUserDetailsFromApi();
                    if (callApiAgain.IsSuccessStatusCode)
                        return await GetUserSession(callApiAgain);
                }
            }
            return null!;
        }

        public async Task SetTokenToLocalStorage(string token) => await localStorageService.SetItemAsStringAsync("access_token", token);
        public async Task RemoveTokenFromLocalStorage() => await localStorageService.RemoveItemAsync("access_token");
        public ClaimsPrincipal SetClaimPrincipal(UserSession model)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Name!),
                new Claim(ClaimTypes.Email, model.Email!),
                new Claim(ClaimTypes.Role, model.Role!)
            }, "AccessTokenAuth"));
        }
        public async Task<HttpClient> AddHeaderToHttpClient()
        {
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", General.DeserializeJsonString<TokenProp>(await GetTokenFromLocalStorage()).Token);
            return httpClient;
        }
        public async Task<UserSession> GetUserSession(HttpResponseMessage response)
        {
            var apiResponse = await response.Content.ReadAsStringAsync();
            return General.DeserializeJsonString<UserSession>(apiResponse);
        }
        private async Task<string> GetTokenFromLocalStorage() => await localStorageService.GetItemAsStringAsync("access_token");
        private async Task<HttpResponseMessage> GetUserDetailsFromApi()
        {
            var httpClient = await AddHeaderToHttpClient();
            return await httpClient.GetAsync("api/Account/user-info");
        }
    }
}
