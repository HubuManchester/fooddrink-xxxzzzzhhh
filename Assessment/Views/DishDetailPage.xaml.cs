using Assessment.ViewModels;

namespace Assessment.Views
{
    public partial class DishDetailPage : ContentPage
    {
        private readonly DishDetailViewModel _viewModel;

        public DishDetailPage() : this(ServiceHelper.GetService<DishDetailViewModel>()) { }

        public DishDetailPage(DishDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private double _startScale = 1;
        private double _currentScale = 1;

        private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
        {
            if (sender is not Image image) return;

            switch (e.Status)
            {
                case GestureStatus.Started:
                    _startScale = _viewModel.CurrentScale;
                    break;
                case GestureStatus.Running:
                    _currentScale = Math.Clamp(_startScale * e.Scale, 0.5, 3.0);
                    image.Scale = _currentScale;
                    break;
                case GestureStatus.Completed:
                    _viewModel.CurrentScale = _currentScale;
                    image.Scale = _currentScale;
                    break;
            }
        }
    }
}