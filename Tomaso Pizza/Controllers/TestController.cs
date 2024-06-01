using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Tomaso_Pizza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController(UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpGet]
        public string Get()
        {
            return "Hello!";
        }
        [HttpGet("Claims")]
        public async Task<IActionResult> GetClaims()
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (emailClaim != null && roleClaim != null)
            {
                return Ok(new { Email = emailClaim, Role = roleClaim });
            }

            return BadRequest("Claims not found.");
        }
    }
}
