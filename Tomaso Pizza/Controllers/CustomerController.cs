using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tomaso_Pizza.Data;
using Tomaso_Pizza.Data.DTO;
using Tomaso_Pizza.Data.Entities;

namespace Tomaso_Pizza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(UserManager<IdentityUser> userManager, PizzaContext context) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly PizzaContext _context = context;

        [Authorize]
        [HttpPost]
        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
        {
            if (orderRequest?.Items == null || orderRequest.Items.Count == 0)
            {
                return BadRequest("Order must contain at least one menu item.");
            }

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

            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                Price = 0, // Calculate this later
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

        [HttpPost]
        [Route("CreateFoodItem")]
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
        
        [HttpGet]
        [Route("GetAllMenuItimes")]
        public async Task<IActionResult> GetAllMenuItimes()
        {
            List<MenuItem> menuItems = [.. _context.MenuItem];

            return Ok(menuItems);
        }

        [Authorize]
        [HttpGet]
        [Route("GetMyOrders")]
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
                                       .ThenInclude(m => m.MenuItem)
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
                MenuItems = o.MenuItem.Select(mi => new MenuItemOrderDto
                {
                    MenuItemId = mi.MenuItemId,
                    Quantity = mi.Quantity,
                    MenuItem = new MenuItemDto
                    {
                        Id = mi.MenuItem.Id,
                        Name = mi.MenuItem.Name,
                        Price = mi.MenuItem.Price,
                        Description = mi.MenuItem.Description,
                        Ingredients = mi.MenuItem.Ingredients,
                        Category = mi.MenuItem.Category
                    }
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }
    }
}
