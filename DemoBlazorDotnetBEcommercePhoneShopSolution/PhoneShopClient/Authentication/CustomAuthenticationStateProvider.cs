using Microsoft.AspNetCore.Components.Authorization;
using PhoneShopClient.Services;
using System.Security.Claims;

namespace PhoneShopClient.Authentication
{
    public class CustomAuthenticationStateProvider(AuthenticationService authenticationService) : AuthenticationStateProvider
    {
        private ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var getUserSession = await authenticationService.GetUserDetails();
                if (getUserSession is null || string.IsNullOrEmpty(getUserSession.Email))
                    return await Task.FromResult(new AuthenticationState(anonymous));
                var claimPrincipal = authenticationService.SetClaimPrincipal(getUserSession);
                return await Task.FromResult(new AuthenticationState(claimPrincipal));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AuthenticationState(anonymous));
            }
        }

        public async Task UpdateAuthenticationState(TokenProp tokenProp)
        {
            ClaimsPrincipal claimPrincipal = new();
            if (tokenProp is not null || string.IsNullOrEmpty(tokenProp!.Token))
            {
                await authenticationService.SetTokenToLocalStorage(General.SerializedObj(tokenProp));
                var getUserSession = await authenticationService.GetUserDetails();
                if(getUserSession is not null && !string.IsNullOrEmpty(getUserSession.Email))
                    claimPrincipal = authenticationService.SetClaimPrincipal(getUserSession);
            }
            else
            {
                claimPrincipal = anonymous;
                await authenticationService.RemoveTokenFromLocalStorage();
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimPrincipal)));
        }

    }
}