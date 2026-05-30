using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Assessment.ViewModels
{
    public partial class WelcomeViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _welcomeText = "Mei Wei Xuan";

        [ObservableProperty]
        private string _subtitleText = "Authentic Chinese Cuisine · Fresh Delivery";

        [ObservableProperty]
        private bool _isAnimating;

        public WelcomeViewModel()
        {
            Title = "Welcome";
        }

        [RelayCommand]
        private void EnterApp()
        {
            Application.Current!.MainPage = new AppShell();
        }

        public async Task AutoEnter()
        {
            await Task.Delay(2000);
            EnterApp();
        }
    }
}