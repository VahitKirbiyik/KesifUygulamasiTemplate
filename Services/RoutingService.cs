using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygamamasiTemplate.Services
{
    // Basit in-memory / provider-agnostic stub
    public class RoutingService : IRoutingService
    {
        public Task<Route> CalculateRouteAsync(Location start, Location end, TransportMode mode = TransportMode.Driving)
        {
            var r = new Route
            {
                Start = start,
                End = end,
                DistanceKm = 1.0, // stub değer
                Duration = TimeSpan.FromMinutes(5)
            };
            r.Path.Add(start);
            r.Path.Add(end);
            return Task.FromResult(r);
        }

        public Task<List<Route>> GetAlternativeRoutesAsync(Location start, Location end, TransportMode mode = TransportMode.Driving)
        {
            var list = new List<Route> { new Route { Start = start, End = end, DistanceKm = 1.0, Duration = TimeSpan.FromMinutes(5) } };
            return Task.FromResult(list);
        }

        public Task<TimeSpan> EstimateTimeAsync(Route route, bool considerTraffic = true)
        {
            return Task.FromResult(route.Duration);
        }
    }
}
