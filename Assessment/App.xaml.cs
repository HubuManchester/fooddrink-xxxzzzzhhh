using Assessment.Services;

namespace Assessment
{
    public partial class App : Application
    {
        public App(ThemeService themeService)
        {
            InitializeComponent();
            themeService.Initialize();
            MainPage = new NavigationPage(new Views.WelcomePage());
        }
    }
}