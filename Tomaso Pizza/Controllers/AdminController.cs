using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tomaso_Pizza.Data;
using Tomaso_Pizza.Data.DTO;
using Tomaso_Pizza.Data.Entities;

namespace Tomaso_Pizza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController(PizzaContext context, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly PizzaContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpDelete("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            Order? order = await _context.Order.SingleOrDefaultAsync(o => o.Id == id);
            if(order == null)
            {
                return BadRequest("Order not found");
            }
            _context.Order.Remove(order);
            var result = await _context.SaveChangesAsync();
            return Ok(result);
        }
        
        [HttpPut("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrder(int id, string newStatus)
        {
            Order? order = await _context.Order.SingleOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return BadRequest("Order not found");
            }
            order.Status = newStatus;
            var result = await _context.SaveChangesAsync();
            return Ok(result);


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

            return Ok("Menu item created successfully.");
        }
        [HttpPut("EditFoodItem")]
        public async Task<IActionResult> EditFoodItem(int id, string newIngredients)
        {
            MenuItem? menuItem = await _context.MenuItem.Where(mi => mi.Id == id).FirstOrDefaultAsync();
            if (menuItem == null)
                return BadRequest("Menu Item doesn't exist");

            menuItem.Ingredients = newIngredients;
            await _context.SaveChangesAsync();

            return Ok("Menu item created successfully.");
        }
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            List<Order> orders = await _context.Order
                .Include(o => o.MenuItem)
                .ThenInclude(mio => mio.MenuItem)
                .ToListAsync();
            List<OrderDto> orderDtos = orders.Select(o => new OrderDto
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

        [HttpPut("SetPremiumUser")]
        public async Task<IActionResult> SetPremiumUser(string email)
        {
            //Check if user exists
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return BadRequest("User not found");
            }
            //Check if user has the PremiumUser role
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("PremiumUser"))
            {
                return BadRequest("This User already already has the PremiumUser role");
            }
            //Check if the user has any roles at all
            if(roles.Count > 0)
            {
                //If so, remove them
                foreach(var role in roles)
                {
                    var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, roles[0]);
                    if (removeRoleResult.Succeeded == false)
                        return BadRequest(removeRoleResult.Errors);
                }
            }
            //Add the PremiumUser role
            var addRoleResult = await _userManager.AddToRoleAsync(user, "PremiumUser");
            if(addRoleResult.Succeeded == false)
            {
                return BadRequest(addRoleResult.Errors);
            }
            return Ok(addRoleResult);
        }
        [HttpPut("SetRegularUser")]
        public async Task<IActionResult> SetRegularUser(string email)
        {
            //Check if user exists
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            //Check if user has the RegularUser role
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("RegularUser"))
            {
                return BadRequest("This User already has the RegularUser role");
            }
            //Check if the user has any roles at all
            if (roles.Count > 0)
            {
                //If so, remove them
                foreach (var role in roles)
                {
                    var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, roles[0]);
                    if (removeRoleResult.Succeeded == false)
                        return BadRequest(removeRoleResult.Errors);
                }
            }
            //Add the PremiumUser role
            var addRoleResult = await _userManager.AddToRoleAsync(user, "RegularUser");
            if (addRoleResult.Succeeded == false)
            {
                return BadRequest(addRoleResult.Errors);
            }
            return Ok(addRoleResult);
        }
    }
}
