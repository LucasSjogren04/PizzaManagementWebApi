using Microsoft.AspNetCore.Identity;
using Tomaso_Pizza.Data.Entities;

namespace Tomaso_Pizza.Services
{
    public class AuthService(UserManager<IdentityUser> userManager) : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;

        public async Task<bool> Login(LoginUser user)
        {
            var identityUser = await _userManager.FindByEmailAsync(user.Email);
            if (identityUser == null)
            {
                return false;
            }

            return await _userManager.CheckPasswordAsync(identityUser, user.Password);
        }

        public async Task<IdentityResult> RegisterUser(RegisterUser registerUser)
        {
            IdentityUser identityUser = new()
            {
                UserName = registerUser.UserName,
                Email = registerUser.Email,
                PhoneNumber = registerUser.PhoneNumber
            };

            IdentityResult result = await _userManager.CreateAsync(identityUser, registerUser.Password);
            return result;
        }
    }
}
