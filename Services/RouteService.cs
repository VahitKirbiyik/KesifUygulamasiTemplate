using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.Services
{
    /// <summary>
    /// Ýki konum arasý rota bilgisini saðlayan servis implementasyonu
    /// </summary>
    public class RouteService : IRouteService
    {
        /// <summary>
        /// Gets route points between two locations
        /// </summary>
        /// <param name="start">Starting location</param>
        /// <param name="end">Ending location</param>
        /// <returns>List of location points that make up the route</returns>
        public async Task<List<Location>> GetRouteAsync(Location start, Location end)
        {
            // Gerçek bir uygulamada burada bir rota API'si kullanýlabilir
            // Bu örnek için basit bir düz çizgi rotasý oluþturuyoruz
            await Task.Delay(100); // Simüle edilmiþ að gecikmesi
            
            var route = new List<Location>();
            
            // Baþlangýç noktasýný ekleyin
            route.Add(start);
            
            // Ýki nokta arasýnda birkaç ara nokta oluþturun (bu basit bir doðrusal interpolasyon)
            int steps = 5;
            for (int i = 1; i < steps; i++)
            {
                double factor = (double)i / steps;
                double latitude = start.Latitude + (end.Latitude - start.Latitude) * factor;
                double longitude = start.Longitude + (end.Longitude - start.Longitude) * factor;
                route.Add(new Location(latitude, longitude));
            }
            
            // Varýþ noktasýný ekleyin
            route.Add(end);
            
            return route;
        }
    }
}
