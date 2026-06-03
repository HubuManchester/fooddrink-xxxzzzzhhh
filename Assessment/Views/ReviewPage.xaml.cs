using Assessment.ViewModels;

namespace Assessment.Views
{
    public partial class ReviewPage : ContentPage
    {
        private readonly ReviewViewModel _viewModel;

        public ReviewPage() : this(ServiceHelper.GetService<ReviewViewModel>()) { }

        public ReviewPage(ReviewViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadReviews();
        }
    }
}
