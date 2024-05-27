using Tomaso_Pizza.Data.Entities;

namespace Tomaso_Pizza.Data.DTO
{
    public class OrderRequest
    {
        public List<ItemDTO> Items { get; set; }
    }
    public class ItemDTO
    {

        public int FoodItemId { get; set; }
        public int Quantity { get; set; }

    }
}
