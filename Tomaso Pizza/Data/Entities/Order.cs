using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tomaso_Pizza.Data.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public DateTime OrderDate { get; set; }
        public double Price { get; set; }
        public string? Status { get; set; }
        public ICollection<MenuItemOrder> MenuItem { get; set; }

    }
}
