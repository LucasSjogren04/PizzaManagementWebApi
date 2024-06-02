using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tomaso_Pizza.Data;
using Tomaso_Pizza.Data.DTO;
using Tomaso_Pizza.Data.Entities;

namespace Tomaso_Pizza.Services
{
    public class CustomerService(PizzaContext context) : ICustomerService
    {
        private readonly PizzaContext _context = context;
        public async Task<OrderRQResult> PremiumOrder(IdentityUser user, OrderRequest orderRequest)
        {
            OrderRQResult discountsAppliedDTO = new();
            Points? points = await _context.Points.Where(p => p.UserId == user.Id).FirstOrDefaultAsync();
            if (points == null)
            {
                discountsAppliedDTO.Succeeded = false;
                discountsAppliedDTO.Error = "Couldn't retrive points";
                return discountsAppliedDTO;
            }
            int itemsOrdered = 0;

            var premiumOrder = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                Price = 0, // Calculate this later
                Status = "Order recieved",
                MenuItem = new List<MenuItemOrder>()
            };

            List<double> prices = [];

            foreach (var itemRequest in orderRequest.Items)
            {
                var menuItem = await _context.MenuItem.FindAsync(itemRequest.FoodItemId);
                if (menuItem == null)
                {
                    discountsAppliedDTO.Succeeded = false;
                    discountsAppliedDTO.Error = "Couldn't find menu item";
                    return discountsAppliedDTO;
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
            discountsAppliedDTO.Discounts = new();
            if (points.PointsCount >= 100)
            {
                double cheapestItemPrice = prices.OrderBy(prices => prices).First();

                premiumOrder.Price -= cheapestItemPrice;

                discountsAppliedDTO.Discounts.Used100PointDiscount = true;
                discountsAppliedDTO.MoneySaved += cheapestItemPrice;
            }

            if (itemsOrdered >= 3)
            {

                premiumOrder.Price *= 0.8;
                discountsAppliedDTO.MoneySaved += premiumOrder.Price * 0.2;
                discountsAppliedDTO.Discounts.TwentyPercent = true;
            }

            points.PointsCount += itemsOrdered * 10;

            _context.Order.Add(premiumOrder);
            await _context.SaveChangesAsync();
            discountsAppliedDTO.Succeeded = true;
            return discountsAppliedDTO;
        }
        public async Task<OrderRQResult> RegularOrder(IdentityUser user, OrderRequest orderRequest)
        {
            OrderRQResult discountsAppliedDTO = new();
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
                    discountsAppliedDTO.Succeeded = false;
                    return discountsAppliedDTO;
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
            discountsAppliedDTO.Succeeded = true;
            return discountsAppliedDTO;
        }
    }
}
