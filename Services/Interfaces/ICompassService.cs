using System;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.Services.Interfaces
{
    /// <summary>
    /// Pusula iþlevleri için interface
    /// </summary>
    public interface ICompassService
    {
        /// <summary>
        /// Mevcut yön (derece olarak)
        /// </summary>
        double CurrentHeading { get; }

        /// <summary>
        /// Pusula okuma baþlatýr
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Pusula okuma durdurur
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Yön deðiþikliði eventi
        /// </summary>
        event EventHandler<double> HeadingChanged;

        /// <summary>
        /// Pusula kalibrasyonu gerekli mi?
        /// </summary>
        bool IsCalibrationRequired { get; }

        /// <summary>
        /// Pusula sensörü mevcut mu?
        /// </summary>
        bool IsAvailable { get; }
    }
}