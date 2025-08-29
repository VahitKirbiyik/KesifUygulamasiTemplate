using System.Collections.Generic;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services
{
    public class TrafficService : ITrafficService
    {
        // TODO: Gerçek API; stub döndürüyor
        public Task<object> GetTrafficDataAsync(double lat, double lon, double? radiusKm = null)
        {
            if (radiusKm.HasValue)
            {
                return Task.FromResult<object>(new List<TrafficInfo>());
            }
            else
            {
                return Task.FromResult<object>(new TrafficInfo { CongestionLevel = "Low" });
            }
        }

        public double GetTrafficDelay(string start, string end)
        {
            // Placeholder: gerçek trafik algoritması daha sonra eklenecek
            return 0.0;
        }

        public void UpdateTrafficData()
        {
            // Placeholder: trafik verisi güncelleme
        }
    }
}
