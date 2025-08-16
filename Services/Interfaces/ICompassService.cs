using System;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.Services.Interfaces
{
    /// <summary>
    /// Pusula i�levleri i�in interface
    /// </summary>
    public interface ICompassService
    {
        /// <summary>
        /// Mevcut y�n (derece olarak)
        /// </summary>
        double CurrentHeading { get; }

        /// <summary>
        /// Pusula okuma ba�lat�r
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Pusula okuma durdurur
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Y�n de�i�ikli�i eventi
        /// </summary>
        event EventHandler<double> HeadingChanged;

        /// <summary>
        /// Pusula kalibrasyonu gerekli mi?
        /// </summary>
        bool IsCalibrationRequired { get; }

        /// <summary>
        /// Pusula sens�r� mevcut mu?
        /// </summary>
        bool IsAvailable { get; }
    }
}