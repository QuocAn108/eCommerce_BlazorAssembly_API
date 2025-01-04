using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneShopServer.Repositories;
using PhoneShopShareLibrary.DTOs;

namespace PhoneShopServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserAccount userAccount) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO model)
        {
            if (model is null)
                return BadRequest("Model is null");
            var response = await userAccount.Register(model);
            return Ok(response);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (model is null)
                return BadRequest("Model is null");
            var response = await userAccount.Login(model);
            return Ok(response);
        }

        [HttpGet("user-info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var token = GetTokenFromHeader();
            if (token is null)
                return Unauthorized("Token is null");
            var getUser = await userAccount.GetUserByToken(token!);
            if (getUser is null || string.IsNullOrEmpty(getUser.Email))
                return Unauthorized("User not found");
            return Ok(getUser);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(PostRefreshTokenDTO model)
        {
            if (model is null)
                return Unauthorized();
            var response = await userAccount.GetRefreshToken(model);
            return Ok(response);
        }

        private string GetTokenFromHeader()
        {
            string Token = string.Empty;
            foreach (var header in Request.Headers)
            {
                if (header.Key.ToString().Equals("Authorization"))
                {
                    Token = header.Value.ToString();
                    break;
                }
            }
            return Token.Split(" ").LastOrDefault()!;
        }
    }
}
