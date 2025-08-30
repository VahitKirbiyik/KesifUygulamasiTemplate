using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygulamasiTemplate.Services.Interfaces
{
    /// <summary>
    /// Konum servisleri için interface
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Mevcut konumu alır
        /// </summary>
        Task<Location?> GetCurrentLocationAsync();

        /// <summary>
        /// Mevcut konum
        /// </summary>
        Location? CurrentLocation { get; }

        /// <summary>
        /// Konum servisinin mevcut olup olmadığı
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Sürekli konum takibi başlatır
        /// </summary>
        Task StartLocationUpdatesAsync();

        /// <summary>
        /// Konum takibini durdurur
        /// </summary>
        Task StopLocationUpdatesAsync();

        /// <summary>
        /// Konum değişikliği eventi
        /// </summary>
        event System.EventHandler<Location> LocationChanged;
    }
}
