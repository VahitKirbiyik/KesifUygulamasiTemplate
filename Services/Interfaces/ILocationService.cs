using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygulamasiTemplate.Services.Interfaces
{
    /// <summary>
    /// Konum servisleri i�in interface
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Mevcut konumu al�r
        /// </summary>
        Task<Location?> GetCurrentLocationAsync();

        /// <summary>
        /// Mevcut konum
        /// </summary>
        Location? CurrentLocation { get; }

        /// <summary>
        /// Konum servisinin mevcut olup olmad���
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// S�rekli konum takibi ba�lat�r
        /// </summary>
        Task StartLocationUpdatesAsync();

        /// <summary>
        /// Konum takibini durdurur
        /// </summary>
        Task StopLocationUpdatesAsync();

        /// <summary>
        /// Konum de�i�ikli�i eventi
        /// </summary>
        event System.EventHandler<Location> LocationChanged;
    }
}