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
            if(model is null)
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
    }
}
