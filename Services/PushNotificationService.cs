using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace KesifUygulamasiTemplate.Services
{
    public class PushNotificationService
    {
        private const string PushPrefKey = "PushNotificationsEnabled";
        public bool IsPushEnabled { get; private set; }

        public PushNotificationService()
        {
            IsPushEnabled = Preferences.Get(PushPrefKey, true);
        }

        public void SetPushEnabled(bool enabled)
        {
            IsPushEnabled = enabled;
            Preferences.Set(PushPrefKey, enabled);
            // Platforma �zel enable/disable i�lemleri burada yap�labilir
        }

        public void OnNotificationReceived(string title, string message, string targetPage = null)
        {
            // Bildirim t�klan�nca ilgili sayfaya y�nlendirme
            if (!string.IsNullOrEmpty(targetPage))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync(targetPage);
                });
            }
        }

        public async Task SendLocalNotificationAsync(string title, string message, string targetPage = null)
        {
            // Burada platforma �zel local notification g�sterimi yap�labilir
            // Geli�mi� push i�in platforma �zel kod veya eklenti gerekir
            await Task.CompletedTask;
        }
    }
}
