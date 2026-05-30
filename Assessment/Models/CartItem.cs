namespace Assessment.Models
{
    public class CartItem
    {
        public Dish Dish { get; set; } = null!;
        public int Quantity { get; set; } = 1;
        public decimal TotalPrice => Dish.Price * Quantity;
    }
}