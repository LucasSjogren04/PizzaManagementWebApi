using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tomaso_Pizza.Data.Entities;
namespace Tomaso_Pizza.Data
{
    public class PizzaContext : IdentityDbContext
    {
        public PizzaContext(DbContextOptions options):base(options)
        {
            
        }
        public DbSet<MenuItem> MenuItem { get; set; }
        public DbSet<Category> Category { get; set; }
    }
}
