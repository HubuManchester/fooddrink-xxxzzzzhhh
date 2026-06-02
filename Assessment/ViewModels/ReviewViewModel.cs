using Assessment.Models;
using Assessment.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Assessment.ViewModels
{
    public partial class ReviewViewModel : BaseViewModel
    {
        private readonly ReviewService _reviewService;
        private readonly DishService _dishService;

        public ReviewViewModel(ReviewService reviewService, DishService dishService)
        {
            _reviewService = reviewService;
            _dishService = dishService;
            Title = "Reviews";
            LoadDishes();
            LoadReviews();
        }

        [ObservableProperty]
        private ObservableCollection<Dish> _dishes = new();

        [ObservableProperty]
        private ObservableCollection<Review> _reviews = new();

        [ObservableProperty]
        private Dish? _selectedDish;

        [ObservableProperty]
        private int _selectedDishIndex = -1;

        [ObservableProperty]
        private string _commentText = string.Empty;

        [ObservableProperty]
        private int _rating = 5;

        [ObservableProperty]
        private string? _photoPath;

        [ObservableProperty]
        private bool _hasPhoto;

        [ObservableProperty]
        private string _photoButtonText = "Take Photo";

        private void LoadDishes()
        {
            Dishes = new ObservableCollection<Dish>(_dishService.GetAllDishes());
        }

        private void LoadReviews()
        {
            Reviews = new ObservableCollection<Review>(_reviewService.GetAllReviews());
        }

        [RelayCommand]
        private void SetRating(string ratingStr)
        {
            if (int.TryParse(ratingStr, out int r) && r >= 1 && r <= 5)
            {
                Rating = r;
            }
        }

        [RelayCommand]
        private async Task TakePhoto()
        {
            try
            {
                ClearError();

                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    SetError("Your device does not support photo capture.");
                    return;
                }

                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                    {
                        SetError("Camera permission is required to take photos. Please allow camera access in settings.");
                        return;
                    }
                }

                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo == null)
                {
                    return;
                }

                var localPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                using (var sourceStream = await photo.OpenReadAsync())
                using (var destStream = File.OpenWrite(localPath))
                {
                    await sourceStream.CopyToAsync(destStream);
                }

                PhotoPath = localPath;
                HasPhoto = true;
                PhotoButtonText = "Retake Photo";
            }
            catch (PermissionException)
            {
                SetError("Camera permission denied. Please allow camera access in settings.");
            }
            catch (Exception ex)
            {
                SetError($"Photo capture failed: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task SubmitReview()
        {
            try
            {
                ClearError();

                if (SelectedDish == null)
                {
                    SetError("Please select a dish to review.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(CommentText))
                {
                    SetError("Please enter your review.");
                    return;
                }

                if (Rating < 1 || Rating > 5)
                {
                    SetError("Please select a rating between 1 and 5 stars.");
                    return;
                }

                var review = new Review
                {
                    DishId = SelectedDish.Id,
                    DishName = SelectedDish.Name,
                    Comment = CommentText,
                    Rating = Rating,
                    PhotoPath = PhotoPath,
                    ReviewDate = DateTime.Now
                };

                _reviewService.AddReview(review);
                LoadReviews();

                CommentText = string.Empty;
                Rating = 5;
                PhotoPath = null;
                HasPhoto = false;
                PhotoButtonText = "Take Photo";
                SelectedDish = null;
                SelectedDishIndex = -1;

                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Review Submitted",
                        $"Thank you for reviewing \"{review.DishName}\"!\nRating: {"★".Repeat(review.Rating)}{"☆".Repeat(5 - review.Rating)}",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                SetError($"Failed to submit review: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

public static class StringExtensions
{
    public static string Repeat(this string s, int count)
    {
        return string.Concat(Enumerable.Repeat(s, count));
    }
}
