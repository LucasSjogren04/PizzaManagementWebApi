using Microsoft.AspNetCore.Identity;
using Tomaso_Pizza.Data.DTO;

namespace Tomaso_Pizza.Services
{
    public interface IAuthService
    {
        Task<string> GenerateTokenString(LoginUser user);
        Task<bool> Login(LoginUser user);
        Task<IdentityResult> RegisterUser(RegisterUserDTO registerUser);
        public Task<(IdentityUser? user, bool)> AuthenticateUserClaim(string claimedRole, string email);


    }
}