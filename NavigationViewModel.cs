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
        
        public NavigationViewModel(ICompassService compassService, ILocationService locationService)
        {
            _compassService = compassService;
            _locationService = locationService;
        }
        
        // Ýki nokta arasýndaki yön hesaplama
        private double CalculateBearing(Location start, Location end) 
        {
            // Haversine formülü hesaplamasý
            var lat1 = start.Latitude * Math.PI / 180;
            var lat2 = end.Latitude * Math.PI / 180;
            var deltaLon = (end.Longitude - start.Longitude) * Math.PI / 180;

            var y = Math.Sin(deltaLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon);

            var bearing = Math.Atan2(y, x) * 180 / Math.PI;
            return (bearing + 360) % 360;
        }
    }
}
