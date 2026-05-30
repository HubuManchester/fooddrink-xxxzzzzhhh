using Assessment.ViewModels;

namespace Assessment
{
    public partial class MainPage : ContentPage
    {
        public MainPage() : this(ServiceHelper.GetService<DishListViewModel>()) { }

        public MainPage(DishListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}