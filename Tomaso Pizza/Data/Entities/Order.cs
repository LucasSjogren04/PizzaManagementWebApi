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

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        public DateTime OrderDate { get; set; }
        public double Price { get; set; }
        public ICollection<MenuItemOrder> MyProperty { get; set; }

    }
}
