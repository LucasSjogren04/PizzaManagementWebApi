namespace Tomaso_Pizza.Data.DTO
{
    public class OrderRQResult
    {
        public Discounts? Discounts { get; set; }
        public double MoneySaved { get; set; }
        public bool Succeeded { get; set; }
        public string? Error { get; set; }
    }
    public class Discounts
    {
        public bool TwentyPercent { get; set; }
        public bool Used100PointDiscount { get; set; }
    }
    
}
