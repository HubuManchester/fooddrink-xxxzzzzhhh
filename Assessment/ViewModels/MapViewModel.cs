using Assessment.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Assessment.ViewModels
{
    public partial class MapViewModel : BaseViewModel
    {
        private readonly LocationService _locationService;

        private const double ShopLatitude = 39.9150;
        private const double ShopLongitude = 116.4040;
        private const string ShopName = "Mei Wei Xuan Chinese Restaurant";

        public event Action<double, double, string>? LocationUpdated;

        public MapViewModel(LocationService locationService)
        {
            _locationService = locationService;
            Title = "Store Location";
        }

        [ObservableProperty]
        private double _currentLatitude;

        [ObservableProperty]
        private double _currentLongitude;

        [ObservableProperty]
        private string _currentAddress = "Getting location...";

        [ObservableProperty]
        private double _shopLatitudeValue = ShopLatitude;

        [ObservableProperty]
        private double _shopLongitudeValue = ShopLongitude;

        [ObservableProperty]
        private string _shopNameDisplay = ShopName;

        [ObservableProperty]
        private string _distanceText = string.Empty;

        [ObservableProperty]
        private bool _hasLocation;

        [ObservableProperty]
        private bool _locationError;

        [ObservableProperty]
        private string _locationErrorMessage = string.Empty;

        [RelayCommand]
        private async Task GetCurrentLocation()
        {
            try
            {
                IsBusy = true;
                ClearError();
                LocationError = false;

                var result = await _locationService.GetCurrentLocationAsync();
                if (result.HasValue)
                {
                    CurrentLatitude = result.Value.Latitude;
                    CurrentLongitude = result.Value.Longitude;
                    CurrentAddress = result.Value.Address;
                    HasLocation = true;

                    var distance = LocationService.CalculateDistance(
                        CurrentLatitude, CurrentLongitude,
                        ShopLatitude, ShopLongitude);

                    DistanceText = distance < 1
                        ? $"About {distance * 1000:F0}m from store"
                        : $"About {distance:F1}km from store";

                    LocationUpdated?.Invoke(CurrentLatitude, CurrentLongitude, CurrentAddress);
                }
            }
            catch (Exception ex)
            {
                LocationError = true;
                LocationErrorMessage = ex.Message;
                SetError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CopyLocation()
        {
            if (!HasLocation)
            {
                return;
            }

            var text = $"Longitude: {CurrentLongitude:F6}, Latitude: {CurrentLatitude:F6}";
            await Clipboard.Default.SetTextAsync(text);

            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Copied", $"Current location coordinates copied to clipboard:\n{text}", "OK");
            }
        }

        [RelayCommand]
        private async Task CopyShopLocation()
        {
            var text = $"Store location - Longitude: {ShopLongitude:F6}, Latitude: {ShopLatitude:F6}";
            await Clipboard.Default.SetTextAsync(text);

            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Copied", $"Store coordinates copied to clipboard:\n{text}", "OK");
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
