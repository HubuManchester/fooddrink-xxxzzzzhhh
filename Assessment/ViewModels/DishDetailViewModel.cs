using Assessment.Models;
using Assessment.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Assessment.ViewModels
{
    [QueryProperty(nameof(Dish), "Dish")]
    public partial class DishDetailViewModel : BaseViewModel
    {
        private readonly CartService _cartService;

        public DishDetailViewModel(CartService cartService)
        {
            _cartService = cartService;
            Title = "Dish Details";
        }

        [ObservableProperty]
        private Dish? _dish;

        [ObservableProperty]
        private int _quantity = 1;

        [ObservableProperty]
        private bool _isSpeaking;

        [ObservableProperty]
        private string _speechButtonText = "Play Description";

        [ObservableProperty]
        private double _currentScale = 1.0;

        partial void OnDishChanged(Dish? value)
        {
            if (value != null)
                Title = value.Name;
        }

        [RelayCommand]
        private void ZoomIn()
        {
            CurrentScale = Math.Min(CurrentScale + 0.2, 3.0);
        }

        [RelayCommand]
        private void ZoomOut()
        {
            CurrentScale = Math.Max(CurrentScale - 0.2, 0.5);
        }

        [RelayCommand]
        private void ResetZoom()
        {
            CurrentScale = 1.0;
        }

        [RelayCommand]
        private void IncreaseQuantity()
        {
            Quantity++;
        }

        [RelayCommand]
        private void DecreaseQuantity()
        {
            if (Quantity > 1)
                Quantity--;
        }

        [RelayCommand]
        private async Task OpenFullScreenImage()
        {
            if (Dish == null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "ImageName", Dish.ImageName },
                { "DishName", Dish.Name }
            };
            await Shell.Current.GoToAsync(nameof(Views.FullScreenImagePage), navigationParameter);
        }

        [RelayCommand]
        private void AddToCart()
        {
            try
            {
                if (Dish == null) return;
                _cartService.AddToCart(Dish, Quantity);
            }
            catch (Exception ex)
            {
                SetError($"Failed to add to cart: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task SpeakDescription()
        {
            try
            {
                if (Dish == null) return;

                if (IsSpeaking)
                {
                    IsSpeaking = false;
                    SpeechButtonText = "Play Description";
                    return;
                }

                IsSpeaking = true;
                SpeechButtonText = "Stop";

                var options = new SpeechOptions
                {
                    Pitch = 1.0f,
                    Volume = 1.0f
                };

                var text = $"{Dish.Name}. {Dish.Description}. Price: {Dish.Price} yuan. Preparation time: approximately {Dish.PrepTimeMinutes} minutes.";

                await TextToSpeech.Default.SpeakAsync(text, options);

                IsSpeaking = false;
                SpeechButtonText = "Play Description";
            }
            catch (Exception ex)
            {
                IsSpeaking = false;
                SpeechButtonText = "Play Description";
                SetError($"Speech failed: {ex.Message}. Please check if text-to-speech is supported on this device.");
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}