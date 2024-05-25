namespace Tomaso_Pizza.Data.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public List<string> Ingredients { get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
