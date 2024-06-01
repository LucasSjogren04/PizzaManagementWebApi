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
    public class CustomerController(UserManager<IdentityUser> userManager, PizzaContext context, IAuthService authService) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly PizzaContext _context = context;
        private readonly IAuthService _authService = authService;

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

            IdentityUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("Invalid Token");

            IList<string> roles = await _userManager.GetRolesAsync(user);
            if (roles[0] != claimedRole)
            {
                return BadRequest("Token out of date");
            }

            if (roles[0] == "PremiumUser")
            {
                Points? points = await _context.Points.Where(p => p.UserId == user.Id).FirstOrDefaultAsync();
                if (points == null)
                    return BadRequest("Unable to proccess request");
                
                int itemsOrdered = 0;
               
                var premiumOrder = new Order
                {
                    UserId = user.Id,
                    OrderDate = DateTime.UtcNow,
                    Price = 0, // Calculate this later
                    Status = "Order recieved",
                    MenuItem = new List<MenuItemOrder>()
                };

                List<double> prices = new();
                foreach (var itemRequest in orderRequest.Items)
                {
                    var menuItem = await _context.MenuItem.FindAsync(itemRequest.FoodItemId);
                    if (menuItem == null)
                    {
                        return NotFound($"MenuItem with ID {itemRequest.FoodItemId} not found.");
                    }
                    prices.Add(menuItem.Price);

                    var menuItemOrder = new MenuItemOrder
                    {
                        MenuItemId = itemRequest.FoodItemId,
                        Quantity = itemRequest.Quantity,
                        Order = premiumOrder
                    };

                    premiumOrder.MenuItem.Add(menuItemOrder);
                    premiumOrder.Price += menuItem.Price * itemRequest.Quantity;
                    itemsOrdered += itemRequest.Quantity;
                    itemsOrdered += itemRequest.Quantity;
                }

                DiscountsAppliedDTO discountsAppliedDTO = new();


                if (points.PointsCount >= 100)
                {
                    double cheapestItemPrice = prices.OrderBy(prices => prices).First();

                    premiumOrder.Price -= cheapestItemPrice;
                    
                    discountsAppliedDTO.Used100PointDiscount = true;
                    discountsAppliedDTO.MoneySaved += cheapestItemPrice;
                }

                if(itemsOrdered >= 3)
                {

                    premiumOrder.Price *= 0.8;
                    discountsAppliedDTO.MoneySaved += premiumOrder.Price * 0.2;
                    discountsAppliedDTO.TwentyPercent = true;
                }

                points.PointsCount += itemsOrdered * 10; 

                _context.Order.Add(premiumOrder);
                await _context.SaveChangesAsync();

                return Ok(discountsAppliedDTO);
            }
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                Price = 0, // Calculate this later
                Status = "Order recieved",
                MenuItem = new List<MenuItemOrder>()
            };

            foreach (var itemRequest in orderRequest.Items)
            {
                var menuItem = await _context.MenuItem.FindAsync(itemRequest.FoodItemId);
                if (menuItem == null)
                {
                    return NotFound($"MenuItem with ID {itemRequest.FoodItemId} not found.");
                }

                var menuItemOrder = new MenuItemOrder
                {
                    MenuItemId = itemRequest.FoodItemId,
                    Quantity = itemRequest.Quantity,
                    Order = order
                };

                order.MenuItem.Add(menuItemOrder);
                order.Price += menuItem.Price * itemRequest.Quantity;
            }

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return Ok("Order created successfully.");
        }

        [HttpPost("CreateFoodItem")]
        public async Task<IActionResult> CreateFoodItem([FromBody] FoodItemDTO food)
        {
            MenuItem item = new()
            {
                Name = food.Name,
                Price = food.Price,
                Description = food.Description,
                Ingredients = food.Ingredients,
                Category = food.Category
            };
            _context.MenuItem.Add(item);
            await _context.SaveChangesAsync();

            return Ok("Order created successfully.");
        }
        
        [HttpGet("GetAllMenuItimes")]
        public async Task<IActionResult> GetAllMenuItimes()
        {
            List<MenuItem> menuItems = [.. _context.MenuItem];

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
