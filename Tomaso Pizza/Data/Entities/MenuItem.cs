using System.ComponentModel.DataAnnotations;

namespace Tomaso_Pizza.Data.Entities
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Category { get; set; }
        public ICollection<MenuItemOrder> Order { get; set; }
    }
}
