namespace Assessment.Services
{
    public class ThemeService
    {
        private const string ThemeKey = "AppTheme";

        public bool IsDarkMode
        {
            get => Preferences.Get(ThemeKey, false);
            set
            {
                Preferences.Set(ThemeKey, value);
                ApplyTheme();
            }
        }

        public void ApplyTheme()
        {
            Application.Current!.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;
        }

        public void Initialize()
        {
            ApplyTheme();
        }
    }
}