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
            // Platforma özel enable/disable iþlemleri burada yapýlabilir
        }

        public void OnNotificationReceived(string title, string message, string targetPage = null)
        {
            // Bildirim týklanýnca ilgili sayfaya yönlendirme
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
            // Burada platforma özel local notification gösterimi yapýlabilir
            // Geliþmiþ push için platforma özel kod veya eklenti gerekir
            await Task.CompletedTask;
        }
    }
}
