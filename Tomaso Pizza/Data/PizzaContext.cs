using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tomaso_Pizza.Data.Entities;
namespace Tomaso_Pizza.Data
{
    public class PizzaContext : IdentityDbContext
    {
        public PizzaContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<MenuItem> MenuItem { get; set; }
        public DbSet<Order> Order { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MenuItemOrder>()
                .HasKey(mio => new { mio.MenuItemId, mio.OrderId });
            builder.Entity<MenuItemOrder>()
                .HasOne(mio => mio.MenuItem)
                .WithMany();
        }
    }
}
