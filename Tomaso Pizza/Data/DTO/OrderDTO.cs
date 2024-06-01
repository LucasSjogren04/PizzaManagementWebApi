namespace Tomaso_Pizza.Data.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public double Price { get; set; }
        public string? Status { get; set; }
        public List<MenuItemOrderDto> MenuItems { get; set; }
    }

    public class MenuItemOrderDto
    {
        public int MenuItemId { get; set; }
        public MenuItemDto MenuItem { get; set; }
        public int Quantity { get; set; }
    }

    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Category { get; set; }
    }
}
