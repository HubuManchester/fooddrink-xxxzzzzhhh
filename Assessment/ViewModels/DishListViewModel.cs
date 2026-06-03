using Assessment.Models;
using Assessment.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Assessment.ViewModels
{
    public partial class DishListViewModel : BaseViewModel
    {
        private readonly DishService _dishService;
        private readonly CartService _cartService;

        private const int PageSize = 10;
        private const double ShakeThreshold = 1.5;
        private const double DeltaThreshold = 0.4;
        private const double ShakeDebounceMs = 1500;
        private DateTime _lastShakeTime = DateTime.MinValue;
        private double _lastMagnitude = 1.0;
        private double _lastX, _lastY, _lastZ;
        private int _warmupCount;

        private int _currentPage;
        private List<Dish> _allDishesCache = new();
        private bool _allLoaded;

        public DishListViewModel(DishService dishService, CartService cartService)
        {
            _dishService = dishService;
            _cartService = cartService;
            _cartService.CartChanged += OnCartChanged;
            Title = "Dishes";
            LoadDishes();
        }

        private void OnCartChanged()
        {
            CartItemCount = _cartService.ItemCount;
        }

        [ObservableProperty]
        private ObservableCollection<Dish> _dishes = new();

        [ObservableProperty]
        private ObservableCollection<string> _categories = new();

        [ObservableProperty]
        private string _selectedCategory = "All";

        [ObservableProperty]
        private int _cartItemCount;

        [ObservableProperty]
        private bool _shakeDetectionOn;

        [ObservableProperty]
        private string _shakeStatusText = "🎲 Shake";

        [ObservableProperty]
        private bool _isLoadingMore;

        [ObservableProperty]
        private bool _hasMoreItems;

        [ObservableProperty]
        private bool _isRefreshing;

        [RelayCommand]
        private void Refresh()
        {
            try
            {
                IsRefreshing = true;

                var allDishes = _dishService.GetAllDishes();
                _allDishesCache = allDishes;

                _currentPage = 0;
                _allLoaded = false;
                var firstPage = _dishService.GetDishesPaged(0, PageSize, SelectedCategory);
                Dishes = new ObservableCollection<Dish>(firstPage);
                _allLoaded = firstPage.Count < PageSize;
                HasMoreItems = !_allLoaded;
            }
            catch (Exception ex)
            {
                SetError($"Refresh failed: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private void LoadDishes()
        {
            try
            {
                IsBusy = true;
                ClearError();

                var allDishes = _dishService.GetAllDishes();
                _allDishesCache = allDishes;

                var cats = _dishService.GetCategories();
                cats.Insert(0, "All");
                Categories = new ObservableCollection<string>(cats);

                CartItemCount = _cartService.ItemCount;

                _currentPage = 0;
                _allLoaded = false;
                var firstPage = _dishService.GetDishesPaged(0, PageSize);
                Dishes = new ObservableCollection<Dish>(firstPage);
                _allLoaded = firstPage.Count < PageSize;
                HasMoreItems = !_allLoaded;
            }
            catch (Exception ex)
            {
                SetError($"Failed to load dishes: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadMore()
        {
            if (_allLoaded || IsLoadingMore || IsBusy)
            {
                return;
            }

            try
            {
                IsLoadingMore = true;

                await Task.Delay(600);

                _currentPage++;

                var nextPage = _dishService.GetDishesPaged(_currentPage, PageSize, SelectedCategory);
                if (nextPage.Count == 0)
                {
                    _allLoaded = true;
                    HasMoreItems = false;
                    return;
                }

                foreach (var dish in nextPage)
                {
                    Dishes.Add(dish);
                }

                _allLoaded = nextPage.Count < PageSize;
                HasMoreItems = !_allLoaded;
            }
            catch (Exception ex)
            {
                SetError($"Failed to load more: {ex.Message}");
            }
            finally
            {
                IsLoadingMore = false;
            }
        }

        [RelayCommand]
        private void FilterByCategory(string category)
        {
            try
            {
                SelectedCategory = category;

                _allDishesCache = category == "All"
                    ? _dishService.GetAllDishes()
                    : _dishService.GetDishesByCategory(category);

                _currentPage = 0;
                _allLoaded = false;

                var firstPage = _dishService.GetDishesPaged(0, PageSize, category);
                Dishes = new ObservableCollection<Dish>(firstPage);
                _allLoaded = firstPage.Count < PageSize;
                HasMoreItems = !_allLoaded;
            }
            catch (Exception ex)
            {
                SetError($"Filter failed: {ex.Message}");
            }
        }

        [RelayCommand]
        private void AddToCart(Dish dish)
        {
            try
            {
                _cartService.AddToCart(dish);
                CartItemCount = _cartService.ItemCount;
            }
            catch (Exception ex)
            {
                SetError($"Failed to add to cart: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task NavigateToDetail(Dish dish)
        {
            if (dish == null)
            {
                return;
            }
            var navigationParameter = new Dictionary<string, object>
            {
                { "Dish", dish }
            };
            await Shell.Current.GoToAsync(nameof(Views.DishDetailPage), navigationParameter);
        }

        [RelayCommand]
        private async Task NavigateToCart()
        {
            await Shell.Current.GoToAsync(nameof(Views.CartPage));
        }

        [RelayCommand]
        private async Task NavigateToMap()
        {
            await Shell.Current.GoToAsync(nameof(Views.MapPage));
        }

        public void RefreshCartCount()
        {
            CartItemCount = _cartService.ItemCount;
        }

        [RelayCommand]
        private void ToggleShake()
        {
            if (ShakeDetectionOn)
            {
                StopShake();
            }
            else
            {
                StartShake();
            }
        }

        private void StartShake()
        {
            if (!Accelerometer.Default.IsSupported)
            {
                SetError("Accelerometer is not supported on this device.");
                return;
            }

            try
            {
                _lastMagnitude = 1.0;
                _lastX = _lastY = _lastZ = 0;
                _warmupCount = 3;
                Accelerometer.Default.ReadingChanged += OnAccelerometerReadingChanged;
                Accelerometer.Default.Start(SensorSpeed.Game);
                ShakeDetectionOn = true;
                ShakeStatusText = "🎲 Shake now...";
            }
            catch (Exception ex)
            {
                SetError($"Failed to start sensor: {ex.Message}");
            }
        }

        private void StopShake()
        {
            Accelerometer.Default.ReadingChanged -= OnAccelerometerReadingChanged;
            Accelerometer.Default.Stop();
            ShakeDetectionOn = false;
            ShakeStatusText = "🎲 Shake";
        }

        private void OnAccelerometerReadingChanged(object? sender, AccelerometerChangedEventArgs e)
        {
            var reading = e.Reading;
            var x = reading.Acceleration.X;
            var y = reading.Acceleration.Y;
            var z = reading.Acceleration.Z;

            var magnitude = Math.Sqrt((x * x) + (y * y) + (z * z));

            if (_warmupCount > 0)
            {
                _warmupCount--;
                _lastMagnitude = magnitude;
                _lastX = x;
                _lastY = y;
                _lastZ = z;
                return;
            }

            bool shakeDetected = magnitude > ShakeThreshold
                || Math.Abs(magnitude - _lastMagnitude) > DeltaThreshold
                || Math.Abs(x - _lastX) > DeltaThreshold
                || Math.Abs(y - _lastY) > DeltaThreshold
                || Math.Abs(z - _lastZ) > DeltaThreshold;

            _lastMagnitude = magnitude;
            _lastX = x;
            _lastY = y;
            _lastZ = z;

            if (!shakeDetected)
            {
                return;
            }

            var now = DateTime.Now;
            if ((now - _lastShakeTime).TotalMilliseconds < ShakeDebounceMs)
            {
                return;
            }

            _lastShakeTime = now;

            MainThread.BeginInvokeOnMainThread(async () => await RecommendRandomDish());
        }

        private async Task RecommendRandomDish()
        {
            try
            {
                var allDishes = _dishService.GetAllDishes();
                if (allDishes.Count == 0)
                {
                    return;
                }

                var random = new Random();
                var recommended = allDishes[random.Next(allDishes.Count)];

                if (Application.Current?.MainPage != null)
                {
                    bool accept = await Application.Current.MainPage.DisplayAlert(
                        "🎲 Shake Recommendation",
                        $"We recommend: {recommended.Name}\n\n{recommended.Description}\n\nPrice: ¥{recommended.Price:F2}\nRating: {recommended.Rating:F1}★",
                        "View",
                        "Cancel");

                    if (accept)
                    {
                        var navParam = new Dictionary<string, object>
                        {
                            { "Dish", recommended }
                        };
                        await Shell.Current.GoToAsync(nameof(Views.DishDetailPage), navParam);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Shake] Recommend failed: {ex.Message}");
            }
        }
    }
}
