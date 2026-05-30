using Assessment.ViewModels;

namespace Assessment.Views
{
    public partial class CartPage : ContentPage
    {
        public CartPage() : this(ServiceHelper.GetService<CartViewModel>()) { }

        public CartPage(CartViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}