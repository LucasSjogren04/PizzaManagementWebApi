using Microsoft.AspNetCore.Identity;
using Tomaso_Pizza.Data.Entities;

namespace Tomaso_Pizza.Services
{
    public class AuthService(UserManager<IdentityUser> userManager)
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;

        public async Task<bool> RegisterUser(RegisterUser registerUser)
        {
            IdentityUser identityUser = new()
            {
                UserName = registerUser.UserName,
                Email = registerUser.Email,
                PhoneNumber = registerUser.PhoneNumber
            };



        }
    }
}
