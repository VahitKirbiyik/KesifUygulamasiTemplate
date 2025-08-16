using System;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygulamasiTemplate.Services
{
    public class LocationService
    {
        public async Task<Location> GetCurrentLocationAsync()
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                
                Location location = await Geolocation.Default.GetLocationAsync(request);
                
                if (location == null)
                {
                    // Fallback to last known location
                    location = await Geolocation.Default.GetLastKnownLocationAsync();
                }
                
                return location;
            }
            catch (FeatureNotSupportedException)
            {
                // Geolocation kullanılamıyor
                return null;
            }
            catch (FeatureNotEnabledException)
            {
                // Konum servisi kapalı
                return null;
            }
            catch (PermissionException)
            {
                // İzin yok
                return null;
            }
            catch (Exception)
            {
                // Genel hata
                return null;
            }
        }
    }
}
