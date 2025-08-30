using System;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;
using KesifUygulamasiTemplate.ViewModels.Base;
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class NavigationViewModel : BaseViewModel
    {
        private readonly ICompassService _compassService;
        private readonly ILocationService _locationService;

        public Location? CurrentLocation { get; private set; }
        public Location? TargetLocation { get; set; }
        public double BearingToTarget { get; private set; } // Hedefe olan yön
        public double CurrentHeading { get; private set; } // Cihazın mevcut yönü

        public NavigationViewModel(ICompassService compassService, ILocationService locationService)
        {
            _compassService = compassService;
            _locationService = locationService;

            // Compass değişikliklerini dinle
            _compassService.HeadingChanged += OnCompassHeadingChanged;
        }

        private void OnCompassHeadingChanged(double heading)
        {
            CurrentHeading = heading;
            OnPropertyChanged(nameof(CurrentHeading));
        }

        // İki nokta arasındaki yön hesaplama
        private double CalculateBearing(Location start, Location end)
        {
            // Haversine formülü hesaplaması
            var lat1 = start.Latitude * Math.PI / 180;
            var lat2 = end.Latitude * Math.PI / 180;
            var deltaLon = (end.Longitude - start.Longitude) * Math.PI / 180;

            var y = Math.Sin(deltaLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon);

            var bearing = Math.Atan2(y, x) * 180 / Math.PI;
            return (bearing + 360) % 360;
        }

        public async Task UpdateCurrentLocationAsync()
        {
            CurrentLocation = await _locationService.GetCurrentLocationAsync();
            OnPropertyChanged(nameof(CurrentLocation));
        }
    }
}
