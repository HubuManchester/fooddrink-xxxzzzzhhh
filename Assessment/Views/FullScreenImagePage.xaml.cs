using Assessment.ViewModels;

namespace Assessment.Views
{
    public partial class FullScreenImagePage : ContentPage
    {
        private readonly FullScreenImageViewModel _viewModel;

        public FullScreenImagePage() : this(ServiceHelper.GetService<FullScreenImageViewModel>()) { }

        public FullScreenImagePage(FullScreenImageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private double _pinchStartScale = 1;
        private double _pinchCurrentScale = 1;

        private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    _pinchStartScale = FullImage.Scale;
                    break;
                case GestureStatus.Running:
                    _pinchCurrentScale = Math.Clamp(_pinchStartScale * e.Scale, 0.5, 5.0);
                    FullImage.Scale = _pinchCurrentScale;
                    break;
                case GestureStatus.Completed:
                    _viewModel.CurrentScale = FullImage.Scale;
                    break;
            }
        }
    }
}