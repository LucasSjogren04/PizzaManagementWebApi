using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tomaso_Pizza.Data.DTO;

namespace Tomaso_Pizza.Services
{
    public class AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        public string GenerateTokenString(LoginUser user)
        {
            IEnumerable<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,"Customer"),
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            
            SecurityToken securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                signingCredentials: signingCredentials
                )
                ;


            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }

        public async Task<bool> Login(LoginUser user)
        {
            var identityUser = await _userManager.FindByEmailAsync(user.Email);
            if (identityUser == null)
            {
                return false;
            }

            return await _userManager.CheckPasswordAsync(identityUser, user.Password);
        }

        public async Task<IdentityResult> RegisterUser(RegisterUserDTO registerUser)
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
