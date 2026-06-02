using Assessment.ViewModels;

namespace Assessment.Views
{
    public partial class DeliveryPage : ContentPage
    {
        private readonly DeliveryViewModel _viewModel;

        public DeliveryPage() : this(ServiceHelper.GetService<DeliveryViewModel>()) { }

        public DeliveryPage(DeliveryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.RefreshData();
        }
    }
}
