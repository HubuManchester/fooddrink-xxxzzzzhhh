using Assessment.Models;

namespace Assessment.Services
{
    public class CartService
    {
        public List<CartItem> Items { get; private set; } = new();

        public int ItemCount => Items.Sum(i => i.Quantity);

        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);

        public event Action? CartChanged;

        public void AddToCart(Dish dish, int quantity = 1)
        {
            var existing = Items.FirstOrDefault(i => i.Dish.Id == dish.Id);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem { Dish = dish, Quantity = quantity });
            }
            CartChanged?.Invoke();
        }

        public void RemoveFromCart(Dish dish)
        {
            var item = Items.FirstOrDefault(i => i.Dish.Id == dish.Id);
            if (item != null)
            {
                Items.Remove(item);
                CartChanged?.Invoke();
            }
        }

        public void UpdateQuantity(Dish dish, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.Dish.Id == dish.Id);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                CartChanged?.Invoke();
            }
        }

        public void ClearCart()
        {
            Items.Clear();
            CartChanged?.Invoke();
        }
    }
}
