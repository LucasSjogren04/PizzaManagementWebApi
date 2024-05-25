using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tomaso_Pizza.Data.Entities;
using Tomaso_Pizza.Services;

namespace Tomaso_Pizza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService service) : ControllerBase
    {
        private readonly IAuthService _service = service;

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUser registerUser)
        {
            IdentityResult result = await _service.RegisterUser(registerUser);
            if(result.Succeeded == true)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser user)
        {
            var result = await _service.Login(user);
            if(result == true)
            {
                return Ok("Wow");
            }
            return BadRequest();
        }
    }
}
