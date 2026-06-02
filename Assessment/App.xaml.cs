using Assessment.Services;

namespace Assessment
{
    public partial class App : Application
    {
        public App(ThemeService themeService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            themeService.Initialize();
            MainPage = new NavigationPage(serviceProvider.GetRequiredService<Views.WelcomePage>());
        }
    }
}
