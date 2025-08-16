using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Networking;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services
{
    public class RoutingService : IRoutingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConnectivity _connectivity;
        private readonly ITrafficService _trafficService;
        private readonly string _apiKey;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public RoutingService(HttpClient httpClient, IConnectivity connectivity, ITrafficService trafficService, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _httpClient = httpClient;
            _connectivity = connectivity;
            _trafficService = trafficService;
            _apiKey = config["MapServices:ApiKey"];
        }
        
        public async Task<Route> CalculateRouteAsync(Location start, Location end, TransportMode mode = TransportMode.Driving)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                throw new InvalidOperationException("Ýnternet baðlantýsý yok. Rota hesaplanamýyor.");
                
            try
            {
                var modeStr = GetModeString(mode);
                
                var url = $"https://maps.googleapis.com/maps/api/directions/json" +
                          $"?origin={start.Latitude},{start.Longitude}" +
                          $"&destination={end.Latitude},{end.Longitude}" +
                          $"&mode={modeStr}" +
                          $"&key={_apiKey}";
                          
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var directionsResponse = await response.Content.ReadFromJsonAsync<DirectionsResponse>();
                    
                    if (directionsResponse?.Status == "OK" && directionsResponse.Routes?.Count > 0)
                    {
                        return ConvertToRoute(directionsResponse.Routes[0], mode, start, end);
                    }
                    
                    throw new Exception($"Rota bulunamadý: {directionsResponse?.Status}");
                }
                
                throw new Exception($"Rota hesaplama API hatasý: {response.ReasonPhrase}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API iletiþim hatasý: {ex.Message}");
                throw new Exception("Rota hesaplama servisiyle iletiþim kurulamadý.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rota hesaplama hatasý: {ex.Message}");
                throw;
            }
        }
        
        public async Task<List<Route>> GetAlternativeRoutesAsync(Location start, Location end, TransportMode mode = TransportMode.Driving, int maxAlternatives = 3)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                throw new InvalidOperationException("Ýnternet baðlantýsý yok. Alternatif rotalar hesaplanamýyor.");
            
            try
            {
                var modeStr = GetModeString(mode);
                
                var url = $"https://maps.googleapis.com/maps/api/directions/json" +
                          $"?origin={start.Latitude},{start.Longitude}" +
                          $"&destination={end.Latitude},{end.Longitude}" +
                          $"&mode={modeStr}" +
                          $"&alternatives=true" +
                          $"&key={_apiKey}";
                          
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var directionsResponse = await response.Content.ReadFromJsonAsync<DirectionsResponse>();
                    
                    if (directionsResponse?.Status == "OK" && directionsResponse.Routes?.Count > 0)
                    {
                        var routes = new List<Route>();
                        
                        foreach (var route in directionsResponse.Routes.Take(maxAlternatives))
                        {
                            routes.Add(ConvertToRoute(route, mode, start, end));
                        }
                        
                        return routes;
                    }
                    
                    throw new Exception($"Alternatif rotalar bulunamadý: {directionsResponse?.Status}");
                }
                
                throw new Exception($"Alternatif rota hesaplama API hatasý: {response.ReasonPhrase}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API iletiþim hatasý: {ex.Message}");
                throw new Exception("Rota hesaplama servisiyle iletiþim kurulamadý.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Alternatif rota hesaplama hatasý: {ex.Message}");
                throw;
            }
        }
        
        public async Task<TimeSpan> EstimateTimeAsync(Location start, Location end, TransportMode mode = TransportMode.Driving, bool considerTraffic = true)
        {
            try
            {
                // Önce temel bir rota hesapla
                var route = await CalculateRouteAsync(start, end, mode);
                
                // Trafik durumunu dikkate alacaksak
                if (considerTraffic && _trafficService != null && mode == TransportMode.Driving)
                {
                    try
                    {
                        // Trafik servisinden trafik faktörünü al
                        var trafficInfo = await _trafficService.GetTrafficInfoAsync(
                            (start.Latitude + end.Latitude) / 2,
                            (start.Longitude + end.Longitude) / 2,
                            CalculateDistance(start, end));
                        
                        // Trafik gecikme faktörünü hesapla
                        double delayFactor = 1.0;
                        if (trafficInfo != null)
                        {
                            // Trafik durumuna göre faktör oluþtur (0-100 arasýnda congestion level için)
                            delayFactor = 1.0 + (trafficInfo.CongestionLevel / 100.0);
                        }
                        
                        // Trafik durumuna göre zamaný ayarla
                        return TimeSpan.FromTicks((long)(route.Duration.Ticks * delayFactor));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Trafik bilgisi alýnamadý: {ex.Message}");
                        // Trafik bilgisi alýnamadýysa normal süreyi döndür
                        return route.Duration;
                    }
                }
                
                // Trafik dikkate alýnmayacaksa normal süreyi döndür
                return route.Duration;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Varýþ zamaný tahmini hatasý: {ex.Message}");
                throw;
            }
        }
        
        public async Task<RouteDirections> GetDirectionsAsync(Route route)
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
                
            try
            {
                // Rota adýmlarýný yönlendirme adýmlarýna dönüþtür
                var directions = new RouteDirections
                {
                    RouteId = route.RouteId,
                    TotalDistance = route.DistanceKm,
                    TotalTime = route.Duration,
                    Steps = new List<DirectionStep>()
                };
                
                foreach (var step in route.Steps)
                {
                    directions.Steps.Add(new DirectionStep
                    {
                        Instruction = step.Instruction,
                        Distance = step.DistanceKm,
                        Duration = step.Duration,
                        ManeuverType = step.ManeuverType
                    });
                }
                
                return directions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Yönlendirme adýmlarý oluþturma hatasý: {ex.Message}");
                throw;
            }
        }
        
        // Yardýmcý metotlar
        private string GetModeString(TransportMode mode)
        {
            return mode switch
            {
                TransportMode.Walking => "walking",
                TransportMode.Bicycling => "bicycling",
                TransportMode.Transit => "transit",
                _ => "driving"
            };
        }
        
        private Route ConvertToRoute(DirectionsRoute directionsRoute, TransportMode mode, Location start, Location end)
        {
            // Polyline'ý koordinat listesine çevirme yardýmcý fonksiyonu
            List<Location> DecodePolyline(string encodedPolyline)
            {
                var points = new List<Location>();
                int index = 0, len = encodedPolyline.Length;
                int lat = 0, lng = 0;
                
                while (index < len)
                {
                    int result = 1;
                    int shift = 0;
                    int b;
                    do
                    {
                        b = encodedPolyline[index++] - 63 - 1;
                        result += b << shift;
                        shift += 5;
                    } while (b >= 0x1f);
                    lat += (result & 1) != 0 ? ~(result >> 1) : (result >> 1);
                    
                    result = 1;
                    shift = 0;
                    do
                    {
                        b = encodedPolyline[index++] - 63 - 1;
                        result += b << shift;
                        shift += 5;
                    } while (b >= 0x1f);
                    lng += (result & 1) != 0 ? ~(result >> 1) : (result >> 1);
                    
                    points.Add(new Location(lat * 1e-5, lng * 1e-5));
                }
                
                return points;
            }
            
            // Yeni bir rota oluþtur
            var route = new Route
            {
                RouteId = Guid.NewGuid().ToString(),
                Start = start,
                End = end,
                TransportMode = mode,
                DistanceKm = directionsRoute.Legs.Sum(l => l.Distance.Value) / 1000.0,
                Duration = TimeSpan.FromSeconds(directionsRoute.Legs.Sum(l => l.Duration.Value)),
                Steps = new List<RouteStep>(),
                Path = DecodePolyline(directionsRoute.OverviewPolyline.Points)
            };
            
            // Her bir adýmý ekle
            foreach (var leg in directionsRoute.Legs)
            {
                foreach (var step in leg.Steps)
                {
                    route.Steps.Add(new RouteStep
                    {
                        Instruction = CleanHtmlInstructions(step.HtmlInstructions),
                        StartLocation = new Location(step.StartLocation.Lat, step.StartLocation.Lng),
                        EndLocation = new Location(step.EndLocation.Lat, step.EndLocation.Lng),
                        DistanceKm = step.Distance.Value / 1000.0,
                        Duration = TimeSpan.FromSeconds(step.Duration.Value),
                        ManeuverType = DetermineManeuverType(step.HtmlInstructions)
                    });
                }
            }
            
            return route;
        }
        
        private string CleanHtmlInstructions(string htmlInstructions)
        {
            // HTML etiketlerini temizle
            return htmlInstructions
                .Replace("<b>", "")
                .Replace("</b>", "")
                .Replace("<div>", " ")
                .Replace("</div>", "")
                .Replace("&nbsp;", " ");
        }
        
        private string DetermineManeuverType(string instruction)
        {
            instruction = instruction.ToLowerInvariant();
            
            if (instruction.Contains("saða dön") || instruction.Contains("right"))
                return "turn-right";
                
            if (instruction.Contains("sola dön") || instruction.Contains("left"))
                return "turn-left";
                
            if (instruction.Contains("devam") || instruction.Contains("straight") || instruction.Contains("continue"))
                return "continue";
                
            if (instruction.Contains("u dönüþü") || instruction.Contains("u-turn"))
                return "u-turn";
                
            if (instruction.Contains("kavþak") || instruction.Contains("roundabout"))
                return "roundabout";
                
            if (instruction.Contains("çýkýþ") || instruction.Contains("exit"))
                return "take-exit";
                
            if (instruction.Contains("varýþ") || instruction.Contains("destination") || instruction.Contains("arrive"))
                return "arrive";
                
            return "unknown";
        }
        
        private double CalculateDistance(Location start, Location end)
        {
            const double R = 6371; // Dünya yarýçapý (km)
            var dLat = ToRadians(end.Latitude - start.Latitude);
            var dLon = ToRadians(end.Longitude - start.Longitude);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(start.Latitude)) * Math.Cos(ToRadians(end.Latitude)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                    
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        
        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}