using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.Services
{
    /// <summary>
    /// �ki konum aras� rota bilgisini sa�layan servis implementasyonu
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
            // Ger�ek bir uygulamada burada bir rota API'si kullan�labilir
            // Bu �rnek i�in basit bir d�z �izgi rotas� olu�turuyoruz
            await Task.Delay(100); // Sim�le edilmi� a� gecikmesi
            
            var route = new List<Location>();
            
            // Ba�lang�� noktas�n� ekleyin
            route.Add(start);
            
            // �ki nokta aras�nda birka� ara nokta olu�turun (bu basit bir do�rusal interpolasyon)
            int steps = 5;
            for (int i = 1; i < steps; i++)
            {
                double factor = (double)i / steps;
                double latitude = start.Latitude + (end.Latitude - start.Latitude) * factor;
                double longitude = start.Longitude + (end.Longitude - start.Longitude) * factor;
                route.Add(new Location(latitude, longitude));
            }
            
            // Var�� noktas�n� ekleyin
            route.Add(end);
            
            return route;
        }
    }
}
