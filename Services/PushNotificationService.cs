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
            // Platforma özel enable/disable işlemleri burada yapılabilir
        }

        public void OnNotificationReceived(string title, string message, string targetPage = null)
        {
            // Bildirim tıklanınca ilgili sayfaya yönlendirme
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
            // Burada platforma özel local notification gösterimi yapılabilir
            // Gelişmiş push için platforma özel kod veya eklenti gerekir
            await Task.CompletedTask;
        }

        #region Bildirim İçerik Örnekleri

        /// <summary>
        /// Navigasyon bildirimleri için hazır içerik
        /// </summary>
        public async Task SendNavigationNotificationAsync(string destinationName, double distanceKm)
        {
            var title = "🗺️ Navigasyon Rehberi";
            var message = $"{destinationName} hedefinize {distanceKm:F1} km kaldı. Yol tarifi için dokunun.";
            var targetPage = "//route";

            await SendLocalNotificationAsync(title, message, targetPage);
        }

        /// <summary>
        /// Hava durumu bildirimleri
        /// </summary>
        public async Task SendWeatherAlertAsync(string condition, double temperature)
        {
            var title = "🌤️ Hava Durumu Uyarısı";
            var message = $"Mevcut hava durumu: {condition}, Sıcaklık: {temperature:F1}°C";
            var targetPage = "//weather";

            await SendLocalNotificationAsync(title, message, targetPage);
        }

        /// <summary>
        /// Acil durum noktası yakınına geldiğinde
        /// </summary>
        public async Task SendEmergencyPointNotificationAsync(string pointName, string pointType)
        {
            var title = "🚨 Acil Nokta Yakınında";
            var message = $"{pointName} ({pointType}) yakınına geldiniz. Güvenliğiniz için dikkatli olun.";
            var targetPage = "//emergency";

            await SendLocalNotificationAsync(title, message, targetPage);
        }

        /// <summary>
        /// Favori yer hatırlatması
        /// </summary>
        public async Task SendFavoritePlaceReminderAsync(string placeName, double distanceKm)
        {
            var title = "⭐ Favori Yer Hatırlatması";
            var message = $"{placeName} favori yeriniz {distanceKm:F1} km yakınızda!";
            var targetPage = "//favorites";

            await SendLocalNotificationAsync(title, message, targetPage);
        }

        /// <summary>
        /// Ay pusulası hatırlatması
        /// </summary>
        public async Task SendMoonCompassReminderAsync(string moonPhase, double illumination)
        {
            var title = "🌙 Ay Pusulası";
            var message = $"Ay fazı: {moonPhase} (Aydınlanma: %{illumination:F0})";
            var targetPage = "//mooncompass";

            await SendLocalNotificationAsync(title, message, targetPage);
        }

        /// <summary>
        /// Trafik durumu uyarısı
        /// </summary>
        public async Task SendTrafficAlertAsync(string roadName, string condition)
        {
            var title = "🚦 Trafik Uyarısı";
            var message = $"{roadName} yolunda {condition}";
            var targetPage = "//traffic";

            await SendLocalNotificationAsync(title, message, targetPage);
        }

        /// <summary>
        /// Konum paylaşımı hatırlatması
        /// </summary>
        public async Task SendLocationSharingReminderAsync()
        {
            var title = "📍 Konum Paylaşımı";
            var message = "Konumunuzu arkadaşlarınızla paylaşıyorsunuz. Gizlilik ayarlarınızı kontrol edin.";
            var targetPage = "//settings";

            await SendLocalNotificationAsync(title, message, targetPage);
        }

        /// <summary>
        /// Offline harita güncelleme hatırlatması
        /// </summary>
        public async Task SendOfflineMapUpdateReminderAsync()
        {
            var title = "🗺️ Offline Harita";
            var message = "Offline haritalarınız güncel değil. Güncelleme için dokunun.";
            var targetPage = "//offline";

            await SendLocalNotificationAsync(title, message, targetPage);
        }

        /// <summary>
        /// Uygulama güncelleme bildirimi
        /// </summary>
        public async Task SendAppUpdateNotificationAsync(string version)
        {
            var title = "⬆️ Güncelleme Mevcut";
            var message = $"Yeni sürüm {version} mevcut! Güncelleme için App Store/Google Play'e gidin.";
            var targetPage = "//settings";

            await SendLocalNotificationAsync(title, message, targetPage);
        }

        /// <summary>
        /// Güvenlik uyarısı
        /// </summary>
        public async Task SendSecurityAlertAsync(string alertType)
        {
            var title = "🔒 Güvenlik Uyarısı";
            var message = $"{alertType} nedeniyle güvenlik önlemleri aktif edildi.";
            var targetPage = "//security";

            await SendLocalNotificationAsync(title, message, targetPage);
        }

        #endregion
    }
}
