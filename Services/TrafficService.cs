using System.Collections.Generic;
using System.Threading.Tasks;

namespace KesifUygamamasiTemplate.Services
{
    public class TrafficService : ITrafficService
    {
        // TODO: Gerçek API; stub döndürüyor
        public Task<TrafficInfo> GetTrafficInfoAsync(double lat, double lon)
        {
            var t = new TrafficInfo { CongestionLevel = "Low" };
            return Task.FromResult(t);
        }

        public Task<List<TrafficInfo>> GetTrafficIncidentsAsync(double lat, double lon, double radiusKm = 5)
        {
            return Task.FromResult(new List<TrafficInfo>());
        }
    }
}
