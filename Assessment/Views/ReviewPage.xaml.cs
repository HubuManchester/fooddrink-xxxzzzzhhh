using Assessment.ViewModels;

namespace Assessment.Views
{
    public partial class ReviewPage : ContentPage
    {
        public ReviewPage() : this(ServiceHelper.GetService<ReviewViewModel>()) { }

        public ReviewPage(ReviewViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
