using Assessment.ViewModels;

namespace Assessment.Views
{
    public partial class FullScreenImagePage : ContentPage
    {
        private readonly FullScreenImageViewModel _viewModel;
        private double _startScale;
        private double _previousPinchScale;

        public FullScreenImagePage() : this(ServiceHelper.GetService<FullScreenImageViewModel>()) { }

        public FullScreenImagePage(FullScreenImageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    _startScale = _viewModel.CurrentScale;
                    _previousPinchScale = e.Scale;
                    break;
                case GestureStatus.Running:
                    double delta = e.Scale / _previousPinchScale;
                    _viewModel.CurrentScale = Math.Clamp(_viewModel.CurrentScale * delta, 0.5, 5.0);
                    _previousPinchScale = e.Scale;
                    break;
                case GestureStatus.Completed:
                    break;
            }
        }

        private void OnZoomInClicked(object? sender, EventArgs e)
        {
            _viewModel.CurrentScale = Math.Min(_viewModel.CurrentScale + 0.5, 5.0);
        }

        private void OnZoomOutClicked(object? sender, EventArgs e)
        {
            _viewModel.CurrentScale = Math.Max(_viewModel.CurrentScale - 0.5, 0.5);
        }

        private void OnResetZoomClicked(object? sender, EventArgs e)
        {
            _viewModel.CurrentScale = 1.0;
        }
    }
}
