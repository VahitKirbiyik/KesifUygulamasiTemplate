using Microsoft.Maui.Devices.Sensors;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.Services;

public class LocationService
{
    public async Task<Location> GetCurrentLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best);
            var location = await Geolocation.Default.GetLocationAsync(request);
            return location;
        }
        catch
        {
            return null;
        }
    }
}
