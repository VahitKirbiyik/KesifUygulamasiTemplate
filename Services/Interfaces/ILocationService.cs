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
        /// Mevcut konumu alýr
        /// </summary>
        Task<Location?> GetCurrentLocationAsync();

        /// <summary>
        /// Mevcut konum
        /// </summary>
        Location? CurrentLocation { get; }

        /// <summary>
        /// Konum servisinin mevcut olup olmadýðý
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Sürekli konum takibi baþlatýr
        /// </summary>
        Task StartLocationUpdatesAsync();

        /// <summary>
        /// Konum takibini durdurur
        /// </summary>
        Task StopLocationUpdatesAsync();

        /// <summary>
        /// Konum deðiþikliði eventi
        /// </summary>
        event System.EventHandler<Location> LocationChanged;
    }
}