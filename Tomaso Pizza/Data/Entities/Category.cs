namespace Tomaso_Pizza.Data.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; }
    }
}
