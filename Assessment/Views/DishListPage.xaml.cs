using Assessment.ViewModels;

namespace Assessment.Views
{
    public partial class DishListPage : ContentPage
    {
        private readonly DishListViewModel _viewModel;

        public DishListPage() : this(ServiceHelper.GetService<DishListViewModel>()) { }

        public DishListPage(DishListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.RefreshCartCount();
        }
    }
}
