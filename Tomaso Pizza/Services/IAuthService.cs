using Microsoft.AspNetCore.Identity;
using Tomaso_Pizza.Data.Entities;

namespace Tomaso_Pizza.Services
{
    public interface IAuthService
    {
        Task<bool> Login(LoginUser user);
        Task<IdentityResult> RegisterUser(RegisterUser registerUser);
    }
}