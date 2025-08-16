using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygulamasiTemplate.Services
{
    public interface IRoutingService
    {
        Task<Route> CalculateRouteAsync(Location start, Location end, TransportMode mode = TransportMode.Driving);
        Task<List<Route>> GetAlternativeRoutesAsync(Location start, Location end, TransportMode mode = TransportMode.Driving, int maxAlternatives = 3);
        Task<TimeSpan> EstimateTimeAsync(Location start, Location end, TransportMode mode = TransportMode.Driving, bool considerTraffic = true);
        Task<RouteDirections> GetDirectionsAsync(Route route);
    }
}