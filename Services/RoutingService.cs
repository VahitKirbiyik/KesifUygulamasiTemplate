using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Services.Interfaces;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services
{
    public class RoutingService : IRoutingService
    {
        public Task<DirectionsRoute?> GetRouteAsync(LatLng from, LatLng to)
        {
            // Stub implementation
            return Task.FromResult<DirectionsRoute?>(new DirectionsRoute
            {
                Points = new List<LatLng> { from, to },
                Distance = 0,
                Duration = TimeSpan.Zero
            });
        }

        public Task<Route> CalculateRouteAsync(Location start, Location end, TransportMode mode = TransportMode.Driving)
        {
            // Stub implementation
            return Task.FromResult(new Route
            {
                Points = new List<Location> { start, end },
                Distance = 0,
                Duration = TimeSpan.Zero,
                Start = start,
                End = end,
                Steps = new List<RouteStep>()
            });
        }

        public Task<List<Route>> GetAlternativeRoutesAsync(Location start, Location end, TransportMode mode = TransportMode.Driving)
        {
            // Stub implementation
            return Task.FromResult(new List<Route>());
        }

        public Task<TimeSpan> EstimateTimeAsync(Route route, bool considerTraffic = true)
        {
            // Stub implementation
            return Task.FromResult(route.Duration);
        }
    }
}
