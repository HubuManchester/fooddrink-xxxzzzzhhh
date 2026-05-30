using System.Collections.ObjectModel;
using Assessment.Models;
using Assessment.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Assessment.ViewModels
{
    public partial class CartViewModel : BaseViewModel
    {
        private readonly CartService _cartService;

        public CartViewModel(CartService cartService)
        {
            _cartService = cartService;
            Title = "Cart";
            _cartService.CartChanged += OnCartChanged;
            LoadCart();
        }

        [ObservableProperty]
        private ObservableCollection<CartItem> _cartItems = new();

        [ObservableProperty]
        private decimal _totalPrice;

        [ObservableProperty]
        private bool _isEmpty = true;

        private void OnCartChanged()
        {
            LoadCart();
        }

        private void LoadCart()
        {
            CartItems = new ObservableCollection<CartItem>(_cartService.Items);
            TotalPrice = _cartService.TotalPrice;
            IsEmpty = !_cartService.Items.Any();
        }

        [RelayCommand]
        private void IncreaseQuantity(CartItem item)
        {
            _cartService.UpdateQuantity(item.Dish, item.Quantity + 1);
        }

        [RelayCommand]
        private void DecreaseQuantity(CartItem item)
        {
            if (item.Quantity > 1)
                _cartService.UpdateQuantity(item.Dish, item.Quantity - 1);
        }

        [RelayCommand]
        private void RemoveItem(CartItem item)
        {
            _cartService.RemoveFromCart(item.Dish);
        }

        [RelayCommand]
        private void ClearCart()
        {
            _cartService.ClearCart();
        }

        [RelayCommand]
        private async Task Checkout()
        {
            if (!_cartService.Items.Any())
            {
                SetError("Cart is empty. Please add dishes first.");
                return;
            }

            _cartService.ClearCart();
            LoadCart();

            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Order Confirmed",
                    $"Your order has been placed!\nTotal: ¥{TotalPrice:F2}\nEstimated delivery: 30-45 minutes\nThank you for your order!",
                    "OK");
            }
        }

        [RelayCommand]
        private async Task ContinueShopping()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}