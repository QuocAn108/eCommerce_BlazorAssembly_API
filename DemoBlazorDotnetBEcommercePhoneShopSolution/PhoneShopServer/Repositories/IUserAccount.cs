using PhoneShopShareLibrary.DTOs;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopServer.Repositories
{
    public interface IUserAccount
    {
        Task<ServiceResponse> Register(UserDTO model);
        Task<LoginResponse> Login(UserDTO model);
        Task<UserSession> GetUserByToken(string token);
        Task<LoginResponse> GetRefreshToken(PostRefreshTokenDTO model);

    }
}
