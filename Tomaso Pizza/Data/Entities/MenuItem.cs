namespace Tomaso_Pizza.Data.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Category { get; set; }
        public ICollection<MenuItemOrder> Orders { get; set; }
    }
}
