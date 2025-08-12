using System;
using System.Threading;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.Services
{
    /// <summary>
    /// Arka planda veri senkronizasyonu yapan servis
    /// </summary>
    public class OfflineSyncService
    {
        private readonly MoonCompassService _moonCompassService;
        private readonly ConnectivityService _connectivityService;
        private readonly PushNotificationService _pushService;
        private Timer _timer;

        public OfflineSyncService(MoonCompassService moonCompassService, ConnectivityService connectivityService, PushNotificationService pushService)
        {
            _moonCompassService = moonCompassService;
            _connectivityService = connectivityService;
            _pushService = pushService;
        }

        public void Start()
        {
            _timer = new Timer(async _ => await SyncAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        }

        public void Stop()
        {
            _timer?.Dispose();
        }

        public async Task SyncAsync()
        {
            if (_connectivityService.IsConnected)
            {
                // Örnek: Dolunay kontrolü
                var moonData = await _moonCompassService.GetMoonDataAsync(40.0, 30.0); // Örnek konum
                if (moonData != null && Math.Abs(moonData.Phase - 1.0) < 0.05)
                {
                    await _pushService.SendLocalNotificationAsync("Dolunay!", "Bu gece dolunay var.", "/MoonCompassPage");
                }
                // Diğer önemli olaylar için benzer kontroller eklenebilir
            }
        }
    }
}
