using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tomaso_Pizza.Data;
using Tomaso_Pizza.Data.DTO;
using Tomaso_Pizza.Data.Entities;

namespace Tomaso_Pizza.Services
{
    public class AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration, PizzaContext context) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly PizzaContext _context = context;

        public async Task<string> GenerateTokenString(LoginUser user)
        {
            //This is for logging in
            IdentityUser? identityUser = await _userManager.FindByEmailAsync(user.Email);
            if (identityUser == null)
            {
                return "Unable to authenticate";
            }

            if(await _userManager.CheckPasswordAsync(identityUser, user.Password) == false)
            {
                return "Unable to authenticate";
            }
            //This is for generating the token
            var roles = await _userManager.GetRolesAsync(identityUser);
            IEnumerable<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role,roles[0]),
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
            

            IdentityResult createResult = await _userManager.CreateAsync(identityUser, registerUser.Password);
            
            if(createResult.Succeeded == false)
                return createResult;

            var addRoleResult = await _userManager.AddToRoleAsync(identityUser, "RegularUser");
            if (addRoleResult.Succeeded == false)
                return addRoleResult; 
            
            Points userPoints = new()
            {
                User = identityUser,
                PointsCount = 0

            };
            await _context.Points.AddAsync(userPoints);
            await _context.SaveChangesAsync();
            return addRoleResult;
        }
    }
}
