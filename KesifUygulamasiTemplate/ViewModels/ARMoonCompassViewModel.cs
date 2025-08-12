using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Devices.Sensors;
using KesifUygulamasiTemplate.Models;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class ARMoonCompassViewModel : BaseViewModel
    {
        private readonly IMoonCompassService _moonCompassService;
        private Location _currentLocation;
        private double _moonAzimuth;
        private double _moonAltitude;
        private DateTime _currentTime;

        public Location CurrentLocation
        {
            get => _currentLocation;
            set => SetProperty(ref _currentLocation, value);
        }
        public double MoonAzimuth
        {
            get => _moonAzimuth;
            set => SetProperty(ref _moonAzimuth, value);
        }
        public double MoonAltitude
        {
            get => _moonAltitude;
            set => SetProperty(ref _moonAltitude, value);
        }
        public DateTime CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        public ICommand RefreshCommand { get; }

        public ARMoonCompassViewModel(IMoonCompassService moonCompassService)
        {
            _moonCompassService = moonCompassService;
            RefreshCommand = new Command(async () => await RefreshAsync());
            CurrentTime = DateTime.Now;
        }

        public async Task RefreshAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Konum al
                var location = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync();
                if (location != null)
                {
                    CurrentLocation = location;
                    // Ay konumu hesapla
                    var moonData = await _moonCompassService.GetMoonDataAsync(location.Latitude, location.Longitude);
                    MoonAzimuth = moonData.Azimuth; // Modelde yoksa eklenmeli
                    MoonAltitude = moonData.Altitude; // Modelde yoksa eklenmeli
                }
            }, "AR verileri güncelleniyor...");
        }
    }
}
