using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tomaso_Pizza.Data.DTO;
using Tomaso_Pizza.Services;

namespace Tomaso_Pizza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService service, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly IAuthService _service = service;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDTO registerUser)
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
                var tokenString = _service.GenerateTokenString(user);
                return Ok(tokenString);
            }
            return BadRequest();
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UpdateUserDTO.ChangePasswordModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                return Unauthorized();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password changed successfully");
        }

        // Update Email, Username, and Phone Number
        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDTO.UpdateProfileModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                return Unauthorized();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            user.Email = model.NewEmail ?? user.Email;
            user.UserName = model.NewUsername ?? user.UserName;
            user.PhoneNumber = model.NewPhoneNumber ?? user.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Profile updated successfully");
        }
    }
}
