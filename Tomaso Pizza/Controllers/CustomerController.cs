using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tomaso_Pizza.Data;
using Tomaso_Pizza.Data.DTO;
using Tomaso_Pizza.Data.Entities;
using Tomaso_Pizza.Services;

namespace Tomaso_Pizza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(UserManager<IdentityUser> userManager, PizzaContext context, IAuthService authService, ICustomerService customerService) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly PizzaContext _context = context;
        private readonly IAuthService _authService = authService;
        private readonly ICustomerService _customerService = customerService;

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
        {
            if (orderRequest?.Items == null || orderRequest.Items.Count == 0)
            {
                return BadRequest("Order must contain at least one menu item.");
            }

            string? claimedRole = User.FindFirstValue(ClaimTypes.Role);
            string? email = User.FindFirstValue(ClaimTypes.Email);
            
            if (claimedRole == null || email == null)
                return BadRequest("Invalid Token");

            (IdentityUser? user, bool tokenAuthenticated) = await _authService.AuthenticateUserClaim(claimedRole, email);
            if(tokenAuthenticated == false || user == null)
                return BadRequest("Invalid Token");

            if (claimedRole == "PremiumUser")
            {
                OrderRQResult premOrderRQresult =  await _customerService.PremiumOrder(user, orderRequest);
                if(premOrderRQresult.Succeeded == true)
                {
                    return Ok(premOrderRQresult);
                }
                return BadRequest(premOrderRQresult);
            }
            OrderRQResult regOrderRQresult = await _customerService.RegularOrder(user, orderRequest);

            return Ok(regOrderRQresult);
        }
        
        
        [HttpGet("GetAllMenuItimes")]
        public async Task<IActionResult> GetAllMenuItimes()
        {
            List<MenuItem> menuItems = await _context.MenuItem.ToListAsync();

            return Ok(menuItems);
        }

        [Authorize]
        [HttpGet("GetMyOrders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Email claim not found.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var orders = await _context.Order
                                       .Include(o => o.MenuItem)
                                       .ThenInclude(mio => mio.MenuItem)
                                       .Where(o => o.UserId == user.Id)
                                       .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the specified user.");
            }

            // Map entities to DTOs
            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Price = o.Price,
                Status = o.Status, 
                
                MenuItems = o.MenuItem.Select(mio => new MenuItemOrderDto
                {
                    MenuItemId = mio.MenuItemId,
                    Quantity = mio.Quantity,
                    MenuItem = new MenuItemDto
                    {
                        Id = mio.MenuItem.Id,
                        Name = mio.MenuItem.Name,
                        Price = mio.MenuItem.Price,
                        Description = mio.MenuItem.Description,
                        Ingredients = mio.MenuItem.Ingredients,
                        Category = mio.MenuItem.Category
                    }
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }

        [Authorize]
        [HttpGet("SeePoints")]
        public async Task<IActionResult> GetMyPoints()
        {
            string? email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return BadRequest("Invalid credentials.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("Invalid credentials.");

            Points? points = await _context.Points.SingleOrDefaultAsync(p => p.UserId == user.Id);
            if (points == null)
                return StatusCode(500, "Something is broken :(");
            
            return Ok(points.PointsCount);
        }


    }
}
