using Assessment.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Assessment.ViewModels
{
    public partial class DeliveryViewModel : BaseViewModel
    {
        private readonly LocationService _locationService;
        private readonly CartService _cartService;

        public DeliveryViewModel(LocationService locationService, CartService cartService)
        {
            _locationService = locationService;
            _cartService = cartService;
            Title = "Delivery";
            _cartService.CartChanged += OnCartChanged;
        }

        [ObservableProperty]
        private double _currentLatitude;

        [ObservableProperty]
        private double _currentLongitude;

        [ObservableProperty]
        private string _currentAddress = "Tap the button below to get your location";

        [ObservableProperty]
        private string _deliveryAddress = string.Empty;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private string _note = string.Empty;

        [ObservableProperty]
        private bool _hasLocation;

        [ObservableProperty]
        private decimal _totalPrice;

        [ObservableProperty]
        private int _itemCount;

        [ObservableProperty]
        private bool _hasItems;

        private void OnCartChanged()
        {
            TotalPrice = _cartService.TotalPrice;
            ItemCount = _cartService.ItemCount;
            HasItems = _cartService.Items.Any();
        }

        [RelayCommand]
        private async Task GetLocation()
        {
            try
            {
                IsBusy = true;
                ClearError();

                var result = await _locationService.GetCurrentLocationAsync();
                if (result.HasValue)
                {
                    CurrentLatitude = result.Value.Latitude;
                    CurrentLongitude = result.Value.Longitude;
                    CurrentAddress = result.Value.Address;
                    HasLocation = true;

                    if (string.IsNullOrEmpty(DeliveryAddress))
                    {
                        DeliveryAddress = result.Value.Address;
                    }
                }
                else
                {
                    SetError("Failed to obtain location. Please ensure location services are enabled and try again.");
                }
            }
            catch (Exception ex)
            {
                SetError($"Location error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SubmitDelivery()
        {
            try
            {
                ClearError();

                if (!_cartService.Items.Any())
                {
                    SetError("Your cart is empty. Please add dishes before placing a delivery order.");
                    return;
                }

                if (!HasLocation)
                {
                    SetError("Location not obtained. Please tap the button above to get your current location first.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(DeliveryAddress))
                {
                    SetError("Delivery address is required.\nPlease enter your full delivery address.");
                    return;
                }

                if (DeliveryAddress.Trim().Length < 5)
                {
                    SetError("Delivery address is too short.\nPlease enter a complete address including street name and building/house number.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(PhoneNumber))
                {
                    SetError("Phone number is required.\nPlease enter a contact phone number for the delivery rider.");
                    return;
                }

                var digitsOnly = new string(PhoneNumber.Where(char.IsDigit).ToArray());
                if (digitsOnly.Length < 7 || digitsOnly.Length > 15)
                {
                    SetError("Invalid phone number.\nPlease enter a valid phone number (7-15 digits).");
                    return;
                }

                var message = $"Order Confirmation\n\n" +
                    $"Delivery Address: {DeliveryAddress}\n" +
                    $"Phone: {PhoneNumber}\n" +
                    $"Note: {(string.IsNullOrWhiteSpace(Note) ? "None" : Note)}\n" +
                    $"Items: {ItemCount}\n" +
                    $"Total: ¥{TotalPrice:F2}\n" +
                    $"Estimated Delivery: 30-45 minutes";

                if (Application.Current?.MainPage != null)
                {
                    bool confirm = await Application.Current.MainPage.DisplayAlert(
                        "Confirm Delivery",
                        message,
                        "Place Order", "Cancel");

                    if (confirm)
                    {
                        _cartService.ClearCart();
                        DeliveryAddress = string.Empty;
                        PhoneNumber = string.Empty;
                        Note = string.Empty;

                        await Application.Current.MainPage.DisplayAlert(
                            "Order Placed",
                            "Your order has been submitted!\nThe rider will deliver to you shortly.\nThank you for your order!",
                            "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                SetError($"Failed to submit delivery: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task NavigateToMenu()
        {
            await Shell.Current.GoToAsync("//DishList");
        }

        public void RefreshData()
        {
            TotalPrice = _cartService.TotalPrice;
            ItemCount = _cartService.ItemCount;
            HasItems = _cartService.Items.Any();
        }
    }
}
