using PhoneShopShareLibrary.DTOs;
using PhoneShopShareLibrary.Responses;

namespace PhoneShopClient.Services
{
    public interface IUserAccountService
    {
        Task<ServiceResponse> Register(UserDTO model);
        Task<LoginResponse> Login(LoginDTO model);
    }
}
