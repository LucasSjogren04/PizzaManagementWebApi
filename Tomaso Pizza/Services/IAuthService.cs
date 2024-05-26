using Microsoft.AspNetCore.Identity;
using Tomaso_Pizza.Data.DTO;

namespace Tomaso_Pizza.Services
{
    public interface IAuthService
    {
        string GenerateTokenString(LoginUser user);
        Task<bool> Login(LoginUser user);
        Task<IdentityResult> RegisterUser(RegisterUser registerUser);
    }
}