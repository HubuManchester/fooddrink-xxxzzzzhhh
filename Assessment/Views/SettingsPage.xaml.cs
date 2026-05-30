using Assessment.ViewModels;

namespace Assessment.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage() : this(ServiceHelper.GetService<SettingsViewModel>()) { }

        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}