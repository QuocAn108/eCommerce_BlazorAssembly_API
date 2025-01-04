
namespace PhoneShopShareLibrary.Responses
{
    public record class ServiceResponse(bool flag, string message);
    public record class LoginResponse(bool flag, string? message, string token = null!, string refreshToken = null!);
}
