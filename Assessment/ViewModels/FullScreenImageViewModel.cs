using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Assessment.ViewModels
{
    [QueryProperty(nameof(ImageName), "ImageName")]
    [QueryProperty(nameof(DishName), "DishName")]
    public partial class FullScreenImageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _imageName = string.Empty;

        [ObservableProperty]
        private string _dishName = string.Empty;

        [ObservableProperty]
        private double _currentScale = 1.0;

        [ObservableProperty]
        private double _startScale = 1.0;

        partial void OnImageNameChanged(string value)
        {
            Title = string.IsNullOrEmpty(DishName) ? "Image Preview" : DishName;
        }

        partial void OnDishNameChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
                Title = value;
        }

        [RelayCommand]
        private void ZoomIn()
        {
            CurrentScale = Math.Min(CurrentScale + 0.5, 5.0);
        }

        [RelayCommand]
        private void ZoomOut()
        {
            CurrentScale = Math.Max(CurrentScale - 0.5, 0.5);
        }

        [RelayCommand]
        private void ResetZoom()
        {
            CurrentScale = 1.0;
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
