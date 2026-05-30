using CommunityToolkit.Mvvm.ComponentModel;

namespace Assessment.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        protected void SetError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        protected void ClearError()
        {
            ErrorMessage = string.Empty;
            HasError = false;
        }
    }
}