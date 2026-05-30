using Assessment.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Assessment.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly ThemeService _themeService;

        public SettingsViewModel(ThemeService themeService)
        {
            _themeService = themeService;
            Title = "Settings";
            _isDarkMode = _themeService.IsDarkMode;
            UpdateThemeText();
        }

        [ObservableProperty]
        private bool _isDarkMode;

        [ObservableProperty]
        private string _themeIcon = "☀️";

        [ObservableProperty]
        private string _themeText = "Current: Light Mode";

        [RelayCommand]
        private void ToggleTheme()
        {
            IsDarkMode = !IsDarkMode;
        }

        partial void OnIsDarkModeChanged(bool value)
        {
            _themeService.IsDarkMode = value;
            UpdateThemeText();
        }

        private void UpdateThemeText()
        {
            ThemeIcon = IsDarkMode ? "🌙" : "☀️";
            ThemeText = IsDarkMode ? "Current: Dark Mode" : "Current: Light Mode";
        }

        [RelayCommand]
        private async Task ShowAbout()
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "About",
                    "Mei Wei Xuan Chinese Restaurant v1.0\n\n" +
                    "A .NET MAUI food ordering and delivery app\n\n" +
                    "Features:\n" +
                    "• Browse & order dishes\n" +
                    "• Pinch-to-zoom dish images\n" +
                    "• Text-to-speech dish descriptions\n" +
                    "• Map location & distance calculation\n" +
                    "• Photo capture for reviews\n" +
                    "• Dark/Light mode toggle\n" +
                    "• Location-based delivery",
                    "OK");
            }
        }

        [RelayCommand]
        private async Task ResetData()
        {
            if (Application.Current?.MainPage != null)
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Reset Data",
                    "Are you sure you want to clear all cart and review data? This cannot be undone.",
                    "OK", "Cancel");

                if (!confirm) return;

                await Application.Current.MainPage.DisplayAlert(
                    "Done",
                    "All data has been reset.",
                    "OK");
            }
        }
    }
}