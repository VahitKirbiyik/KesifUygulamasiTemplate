using System;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.Services
{
    /// <summary>
    /// Kullanýcý geri bildirim mekanizmasý (Toast, Snackbar, Alert)
    /// Platform baðýmsýz þekilde mesaj gösterimi saðlar
    /// </summary>
    public class NotificationService
    {
        /// <summary>
        /// Kýsa süreli bilgilendirme mesajý gösterir (Toast)
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
                System.Diagnostics.Debug.WriteLine($"Toast gösterim hatasý: {ex.Message}");
            }
        }

        /// <summary>
        /// Baþarý mesajý gösterir (yeþil renkte)
        /// </summary>
        public async Task ShowSuccessAsync(string message)
        {
            await ShowToastAsync($"? {message}", ToastDuration.Short);
        }

        /// <summary>
        /// Hata mesajý gösterir (kýrmýzý renkte)
        /// </summary>
        public async Task ShowErrorAsync(string message)
        {
            await ShowToastAsync($"? {message}", ToastDuration.Long);
        }

        /// <summary>
        /// Uyarý mesajý gösterir (sarý renkte)
        /// </summary>
        public async Task ShowWarningAsync(string message)
        {
            await ShowToastAsync($"?? {message}", ToastDuration.Short);
        }

        /// <summary>
        /// Bilgi mesajý gösterir (mavi renkte)
        /// </summary>
        public async Task ShowInfoAsync(string message)
        {
            await ShowToastAsync($"?? {message}", ToastDuration.Short);
        }

        /// <summary>
        /// Snackbar gösterir (action button ile)
        /// </summary>
        public async Task ShowSnackbarAsync(string message, string actionText = null, Func<Task> action = null)
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

                var snackbar = Snackbar.Make(message, action, actionText, TimeSpan.FromSeconds(3), snackbarOptions);
                await snackbar.Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Snackbar gösterim hatasý: {ex.Message}");
            }
        }

        /// <summary>
        /// Konfirmasyon dialog'u gösterir
        /// </summary>
        public async Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Evet", string cancel = "Ýptal")
        {
            try
            {
                return await Application.Current?.MainPage?.DisplayAlert(title, message, accept, cancel) == true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Confirmation dialog hatasý: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loading mesajý gösterir
        /// </summary>
        public async Task ShowLoadingAsync(string message = "Yükleniyor...")
        {
            await ShowInfoAsync(message);
        }
    }
}