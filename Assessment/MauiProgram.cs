using Assessment.Services;
using Assessment.ViewModels;
using Assessment.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Assessment
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<DishService>();
            builder.Services.AddSingleton<CartService>();
            builder.Services.AddSingleton<ReviewService>();
            builder.Services.AddSingleton<LocationService>();

            builder.Services.AddTransient<WelcomeViewModel>();
            builder.Services.AddTransient<DishListViewModel>();
            builder.Services.AddTransient<DishDetailViewModel>();
            builder.Services.AddTransient<CartViewModel>();
            builder.Services.AddTransient<MapViewModel>();
            builder.Services.AddTransient<ReviewViewModel>();
            builder.Services.AddTransient<DeliveryViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<FullScreenImageViewModel>();

            builder.Services.AddTransient<WelcomePage>();
            builder.Services.AddTransient<DishListPage>();
            builder.Services.AddTransient<DishDetailPage>();
            builder.Services.AddTransient<CartPage>();
            builder.Services.AddTransient<MapPage>();
            builder.Services.AddTransient<ReviewPage>();
            builder.Services.AddTransient<DeliveryPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<FullScreenImagePage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
