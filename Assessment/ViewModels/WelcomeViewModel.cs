using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Assessment.ViewModels
{
    public partial class WelcomeViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string _welcomeText = "Mei Wei Xuan";

        [ObservableProperty]
        private string _subtitleText = "Authentic Chinese Cuisine · Fresh Delivery";

        [ObservableProperty]
        private bool _isAnimating;

        public WelcomeViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Title = "Welcome";
        }

        [RelayCommand]
        private void EnterApp()
        {
            Application.Current!.MainPage = _serviceProvider.GetRequiredService<AppShell>();
        }

        public async Task AutoEnter()
        {
            await Task.Delay(2000);
            EnterApp();
        }
    }
}