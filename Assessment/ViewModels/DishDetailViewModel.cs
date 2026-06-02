using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assessment.Models;
using Assessment.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;

namespace Assessment.ViewModels
{
    [QueryProperty(nameof(Dish), "Dish")]
    public partial class DishDetailViewModel : BaseViewModel
    {
        private static readonly string[] ChunkSeparator = [". "];

        private readonly CartService _cartService;
        private CancellationTokenSource? _cts;
        private int _speechGeneration;

        public DishDetailViewModel(CartService cartService)
        {
            _cartService = cartService;
            Title = "Dish Details";
        }

        [ObservableProperty]
        private Dish? _dish;

        [ObservableProperty]
        private int _quantity = 1;

        [ObservableProperty]
        private bool _isSpeaking;

        [ObservableProperty]
        private string _speechButtonText = "Play Description";

        [ObservableProperty]
        private double _currentScale = 1.0;

        partial void OnDishChanged(Dish? value)
        {
            if (value != null)
            {
                Title = value.Name;
            }
        }

        [RelayCommand]
        private void ZoomIn()
        {
            CurrentScale = Math.Min(CurrentScale + 0.2, 3.0);
        }

        [RelayCommand]
        private void ZoomOut()
        {
            CurrentScale = Math.Max(CurrentScale - 0.2, 0.5);
        }

        [RelayCommand]
        private void ResetZoom()
        {
            CurrentScale = 1.0;
        }

        [RelayCommand]
        private void IncreaseQuantity()
        {
            Quantity++;
        }

        [RelayCommand]
        private void DecreaseQuantity()
        {
            if (Quantity > 1)
            {
                Quantity--;
            }
        }

        [RelayCommand]
        private async Task OpenFullScreenImage()
        {
            if (Dish == null)
            {
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                { "ImageName", Dish.ImageName },
                { "DishName", Dish.Name },
            };

            await Shell.Current.GoToAsync(nameof(Views.FullScreenImagePage), navigationParameter);
        }

        [RelayCommand]
        private void AddToCart()
        {
            try
            {
                if (Dish == null)
                {
                    return;
                }

                _cartService.AddToCart(Dish, Quantity);
            }
            catch (Exception ex)
            {
                SetError($"Failed to add to cart: {ex.Message}");
            }
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task SpeakDescription()
        {
            if (Dish == null)
            {
                return;
            }

            if (IsSpeaking)
            {
                _cts?.Cancel();
                return;
            }

            _speechGeneration++;
            var generation = _speechGeneration;

            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            IsSpeaking = true;
            SpeechButtonText = "Stop";

            try
            {
                var text = $"{Dish.Name}. {Dish.Description}. Price: {Dish.Price} yuan. " +
                           $"Preparation time: approximately {Dish.PrepTimeMinutes} minutes.";

                var chunks = text.Split(ChunkSeparator, StringSplitOptions.RemoveEmptyEntries);

                foreach (var rawChunk in chunks)
                {
                    _cts.Token.ThrowIfCancellationRequested();

                    var chunk = rawChunk.Trim();

                    if (chunk.Length == 0)
                    {
                        continue;
                    }

                    if (!chunk.EndsWith('.'))
                    {
                        chunk += ".";
                    }

                    // 传入 null 作为 SpeechOptions，明确匹配三参数重载
                    // SpeakAsync(string, SpeechOptions?, CancellationToken)
                    // 底层 TTS 引擎收到取消信号后立即停止当前段播放
                    await TextToSpeech.Default.SpeakAsync(chunk, null, _cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // 用户点击了停止，正常退出
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                    SetError($"Speech failed: {ex.Message}"));
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;

                if (generation == _speechGeneration)
                {
                    IsSpeaking = false;
                    SpeechButtonText = "Play Description";
                }
            }
        }

        [RelayCommand]
        private static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}