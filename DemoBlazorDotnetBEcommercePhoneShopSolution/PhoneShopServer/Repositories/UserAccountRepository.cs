using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using PhoneShopServer.Data;
using PhoneShopShareLibrary.DTOs;
using PhoneShopShareLibrary.Responses;
using System.Security.Cryptography;
using System.Text;

namespace PhoneShopServer.Repositories
{
    public class UserAccountRepository(ApplicationDBContext context) : IUserAccount
    {
        public async Task<LoginResponse> GetRefreshToken(PostRefreshTokenDTO model)
        {
            var decodedToken = WebEncoders.Base64UrlDecode(model.RefreshToken!);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var getTokens = await context.TokenInfo.FirstOrDefaultAsync(t => t.RefreshToken == normalToken);
            if (getTokens is null) return null!;

            var (newAccessToken, newRefreshToken) = await GenerateTokens();
            await SaveToTokenInfo(getTokens.UserId, newAccessToken, newRefreshToken);
            return new LoginResponse(true, "Token refreshed", newAccessToken, newRefreshToken);
        }

        public async Task<UserSession> GetUserByToken(string token)
        {
            var result = await context.TokenInfo.FirstOrDefaultAsync(t => t.AccessToken! == token);
            if (result is null) return null!;
            var getUserInfo = await context.UserAccount.FirstOrDefaultAsync(u => u.Id == result.UserId);
            if (getUserInfo is null) return null!;
            if (result.ExpiryDate < DateTime.Now) return null!;
            var getUserRole = await context.UserRole.FirstOrDefaultAsync(u => u.UserId == getUserInfo.Id);
            if (getUserRole is null) return null!;
            var role = await context.SystemRole.FirstOrDefaultAsync(r => r.Id == getUserRole.RoleId);
            if (role is null) return null!;
            return new UserSession() { Email = getUserInfo.Email, Name = getUserInfo.Name, Role = role.Name };
        }

        public async Task<LoginResponse> Login(LoginDTO model)
        {
            if (model is null) return new LoginResponse(false, "Model is null");
            var findUser = await context.UserAccount.FirstOrDefaultAsync(u => u.Email! == model.Email!);
            if (findUser is null) return new LoginResponse(false, "User not found");
            if (!BCrypt.Net.BCrypt.Verify(model!.Password, findUser.Password))
                return new LoginResponse(false, "Invalid username/password");
            var (accessToken, refreshToken) = await GenerateTokens();
            await SaveToTokenInfo(findUser.Id, accessToken, refreshToken);
            return new LoginResponse(true, "Login successful", accessToken, refreshToken);
        }

        private async Task SaveToTokenInfo(int userId, string accessToken, string refreshToken)
        {
            var getUser = await context.TokenInfo.FirstOrDefaultAsync(t => t.UserId == userId);
            if (getUser is null)
            {
                context.TokenInfo.Add(new TokenInfo
                {
                    UserId = userId,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
                await Commit();
            }
            else
            {
                getUser.AccessToken = accessToken;
                getUser.RefreshToken = refreshToken;
                getUser.ExpiryDate = DateTime.Now.AddDays(1);
                await Commit();
            }
        }
        private async Task<(string AccessToken, string RefreshToken)> GenerateTokens()
        {
            var accessToken = GenerateToken(256);
            var refreshToken = GenerateToken(64);
            while (!await VerifyToken(accessToken))
            {
                accessToken = GenerateToken(256);
            }
            while (!await VerifyToken(refreshToken))
            {
                refreshToken = GenerateToken(256);
            }
            return (accessToken, refreshToken);
        }

        private async Task<bool> VerifyToken(string refreshToken = null!, string accessToken = null!)
        {
            TokenInfo tokenInfo = new();
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var getRefreshToken = await context.TokenInfo.FirstOrDefaultAsync(t => t.RefreshToken! == refreshToken);
                return getRefreshToken is null ? true : false;
            }
            else
            {
                var getAccessToken = await context.TokenInfo.FirstOrDefaultAsync(t => t.AccessToken! == accessToken);
                return getAccessToken is null;
            }
        }
        private static string GenerateToken(int numberOfBytes) => Convert.ToBase64String(RandomNumberGenerator.GetBytes(numberOfBytes));

        public async Task<ServiceResponse> Register(UserDTO model)
        {
            if (model is null) return new ServiceResponse(false, "Model is null");
            var findUser = await context.UserAccount.FirstOrDefaultAsync(u => u.Email!.ToLower() == model.Email!.ToLower());
            if (findUser != null)
                return new ServiceResponse(false, "Email already exists");
            var user = context.UserAccount.Add(new UserAccount
            {
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Name = model.Name,
                Email = model.Email
            }).Entity;
            await Commit();
            var checkIfAdminIsCreated = await context.SystemRole.FirstOrDefaultAsync(_ => _.Name!.ToLower() == "admin");
            if (checkIfAdminIsCreated is null)
            {
                var result = context.SystemRole.Add(new SystemRole
                {
                    Name = "Admin"
                }).Entity;
                await Commit();
                context.UserRole.Add(new UserRole
                {
                    RoleId = result.Id,
                    UserId = user.Id
                });
            }
            else
            {
                var checkIfUserIsCreated = await context.SystemRole.FirstOrDefaultAsync(_ => _.Name!.ToLower() == "user");
                int RoleId = 0;
                if (checkIfUserIsCreated is null)
                {
                    var result = context.SystemRole.Add(new SystemRole
                    {
                        Name = "User"
                    }).Entity;
                    await Commit();
                    RoleId = result.Id;
                }
                context.UserRole.Add(new UserRole
                {
                    RoleId = RoleId == 0 ? checkIfUserIsCreated!.Id : RoleId,
                    UserId = user.Id
                });
                await Commit();
            }
            return new ServiceResponse(true, "Account created successfully");
        }
        private async Task Commit()
        {
            await context.SaveChangesAsync();
        }
    }
}
