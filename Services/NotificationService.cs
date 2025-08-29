using System;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.Services
{
    /// <summary>
    /// Kullan�c� geri bildirim mekanizmas� (Toast, Snackbar, Alert)
    /// Platform ba��ms�z �ekilde mesaj g�sterimi sa�lar
    /// </summary>
    public class NotificationService
    {
        /// <summary>
        /// K�sa s�reli bilgilendirme mesaj� g�sterir (Toast)
        /// </summary>
        public async Task ShowToastAsync(string message, ToastDuration duration = ToastDuration.Short)
        {
            try
            {
                var toast = Toast.Make(message, duration);
                await toast.Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Toast g�sterim hatas�: {ex.Message}");
            }
        }

        /// <summary>
        /// Ba�ar� mesaj� g�sterir (ye�il renkte)
        /// </summary>
        public async Task ShowSuccessAsync(string message)
        {
            await ShowToastAsync($"? {message}", ToastDuration.Short);
        }

        /// <summary>
        /// Hata mesaj� g�sterir (k�rm�z� renkte)
        /// </summary>
        public async Task ShowErrorAsync(string message)
        {
            await ShowToastAsync($"? {message}", ToastDuration.Long);
        }

        /// <summary>
        /// Uyar� mesaj� g�sterir (sar� renkte)
        /// </summary>
        public async Task ShowWarningAsync(string message)
        {
            await ShowToastAsync($"?? {message}", ToastDuration.Short);
        }

        /// <summary>
        /// Bilgi mesaj� g�sterir (mavi renkte)
        /// </summary>
        public async Task ShowInfoAsync(string message)
        {
            await ShowToastAsync($"?? {message}", ToastDuration.Short);
        }

        /// <summary>
        /// Snackbar g�sterir (action button ile)
        /// </summary>
        public async Task ShowSnackbarAsync(string message, string? actionText = null, Func<Task>? action = null)
        {
            try
            {
                var snackbarOptions = new SnackbarOptions
                {
                    BackgroundColor = Colors.DarkSlateGray,
                    TextColor = Colors.White,
                    ActionButtonTextColor = Colors.Yellow,
                    CornerRadius = new CornerRadius(10),
                    Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                    ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14, FontWeight.Bold)
                };

                Action? actionDelegate = action != null ? () => action() : null;
                var snackbar = Snackbar.Make(message, actionDelegate, actionText ?? "OK", TimeSpan.FromSeconds(3), snackbarOptions);
                await snackbar.Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Snackbar g�sterim hatas�: {ex.Message}");
            }
        }

        /// <summary>
        /// Konfirmasyon dialog'u g�sterir
        /// </summary>
        public async Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Evet", string cancel = "�ptal")
        {
            try
            {
                return Application.Current?.MainPage != null && await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel) == true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Confirmation dialog hatas�: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loading mesaj� g�sterir
        /// </summary>
        public async Task ShowLoadingAsync(string message = "Y�kleniyor...")
        {
            await ShowInfoAsync(message);
        }
    }
}
