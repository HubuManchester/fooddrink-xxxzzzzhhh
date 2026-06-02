using Assessment.ViewModels;

namespace Assessment.Views
{
    public partial class DishDetailPage : ContentPage
    {
        private readonly DishDetailViewModel _viewModel;
        private double _previousPinchScale;

        public DishDetailPage() : this(ServiceHelper.GetService<DishDetailViewModel>()) { }

        public DishDetailPage(DishDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    _previousPinchScale = e.Scale;
                    break;
                case GestureStatus.Running:
                    double delta = e.Scale / _previousPinchScale;
                    _viewModel.CurrentScale = Math.Clamp(_viewModel.CurrentScale * delta, 0.5, 3.0);
                    _previousPinchScale = e.Scale;
                    break;
                case GestureStatus.Completed:
                    break;
            }
        }
    }
}
