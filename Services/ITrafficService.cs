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
    public interface ITrafficService
    {
        Task<TrafficInfo> GetTrafficInfoAsync(double latitude, double longitude, double radius = 5.0);
        Task<List<TrafficIncident>> GetTrafficIncidentsAsync(double north, double south, double east, double west);
        Task<bool> IsTrafficAvailableAsync();
        Task<Route> GetOptimizedRouteWithTrafficAsync(Location start, Location end, TransportMode mode, RouteOptimizationPreference preference);
    }

    public class TrafficService : ITrafficService
    {
        private readonly HttpClient _httpClient;
        private readonly IConnectivity _connectivity;
        private readonly IGeolocation _geolocation;
        private readonly string _apiKey;
        private readonly Random _random; // Simülasyon için
        private DateTime _lastUpdated;
        private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);
        private Dictionary<string, CachedTrafficData> _trafficCache = new Dictionary<string, CachedTrafficData>();
        
        // Cache verilerini tutan yardýmcý sýnýf
        private class CachedTrafficData
        {
            public TrafficInfo TrafficInfo { get; set; }
            public List<TrafficIncident> Incidents { get; set; }
            public DateTime CacheTime { get; set; }
        }

        public TrafficService(HttpClient httpClient, IConnectivity connectivity, IGeolocation geolocation, IConfiguration config)
        {
            _httpClient = httpClient;
            _connectivity = connectivity;
            _geolocation = geolocation;
            _apiKey = config["MapServices:ApiKey"];
            _random = new Random();
            _lastUpdated = DateTime.UtcNow;
        }

        public async Task<TrafficInfo> GetTrafficInfoAsync(double latitude, double longitude, double radius = 5.0)
        {
            // Ýnternet baðlantýsý kontrolü
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return CreateDefaultTrafficInfo();
                
            try
            {
                // Cache kontrolü
                string cacheKey = $"traffic_{latitude:F3}_{longitude:F3}_{radius:F1}";
                
                await _cacheLock.WaitAsync();
                try
                {
                    // Cache'de varsa ve son 5 dakika içinde güncellenmiþ ise cache'den döndür
                    if (_trafficCache.TryGetValue(cacheKey, out var cachedData) && 
                        (DateTime.UtcNow - cachedData.CacheTime).TotalMinutes < 5)
                    {
                        return cachedData.TrafficInfo;
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }
                
                // Gerçek API çaðrýsý burada olacak
                // Örnek: Google Maps Distance Matrix API veya alternatif bir trafik API'si
                
                // API simulasyonu - gerçek uygulamada burayý gerçek API çaðrýsý ile deðiþtirin
                await Task.Delay(300); // API çaðrýsý simülasyonu
                
                // Günün saatine göre trafik yoðunluðunu deðiþtir (sabah ve akþam yoðun)
                var hour = DateTime.Now.Hour;
                int congestionBase = 20; // Normal zamanlarda temel yoðunluk
                
                // Sabah ve akþam trafik yoðunluðu artar
                if ((hour >= 7 && hour <= 9) || (hour >= 17 && hour <= 19))
                {
                    congestionBase = 70; // Yoðun trafik
                }
                else if ((hour >= 10 && hour <= 16) || (hour >= 20 && hour <= 21))
                {
                    congestionBase = 40; // Orta yoðunlukta trafik
                }
                
                // Random dalgalanmalar ekle
                int congestionLevel = Math.Min(100, Math.Max(0, congestionBase + _random.Next(-15, 16)));
                
                // Trafik bilgisini oluþtur
                var trafficInfo = new TrafficInfo
                {
                    CongestionLevel = congestionLevel,
                    DelayTime = TimeSpan.FromMinutes(congestionLevel / 10.0 * 2), // Her 10 birim yoðunluk için 2 dakika gecikme
                    TypicalTravelTime = TimeSpan.FromMinutes(radius * 2), // Yaklaþýk hesaplama
                    CurrentTravelTime = TimeSpan.FromMinutes(radius * 2 * (1 + congestionLevel / 100.0)), // Trafik yoðunluðuna göre artýrýlmýþ süre
                    LastUpdated = DateTime.UtcNow
                };
                
                // Cache'e ekle
                await _cacheLock.WaitAsync();
                try
                {
                    if (!_trafficCache.ContainsKey(cacheKey))
                    {
                        _trafficCache[cacheKey] = new CachedTrafficData 
                        { 
                            TrafficInfo = trafficInfo,
                            CacheTime = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        _trafficCache[cacheKey].TrafficInfo = trafficInfo;
                        _trafficCache[cacheKey].CacheTime = DateTime.UtcNow;
                    }
                    
                    // Cache boyutunu kontrol et
                    if (_trafficCache.Count > 100)
                    {
                        // En eski 20 öðeyi sil
                        var oldestKeys = _trafficCache
                            .OrderBy(x => x.Value.CacheTime)
                            .Take(20)
                            .Select(x => x.Key)
                            .ToList();
                            
                        foreach (var key in oldestKeys)
                        {
                            _trafficCache.Remove(key);
                        }
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }
                
                return trafficInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Trafik bilgisi alma hatasý: {ex.Message}");
                return CreateDefaultTrafficInfo();
            }
        }

        public async Task<List<TrafficIncident>> GetTrafficIncidentsAsync(double north, double south, double east, double west)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return new List<TrafficIncident>();
                
            try
            {
                // Cache kontrolü için bounding box keyleri
                string cacheKey = $"incidents_{north:F3}_{south:F3}_{east:F3}_{west:F3}";
                
                await _cacheLock.WaitAsync();
                try
                {
                    // Cache'de varsa ve son 5 dakika içinde güncellenmiþ ise cache'den döndür
                    if (_trafficCache.TryGetValue(cacheKey, out var cachedData) && 
                        (DateTime.UtcNow - cachedData.CacheTime).TotalMinutes < 5 &&
                        cachedData.Incidents != null)
                    {
                        return cachedData.Incidents;
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }
                
                // Gerçek API çaðrýsý burada olacak
                // Örnek: HERE, TomTom veya Google Maps trafik olaylarý API'si
                
                // API simulasyonu - gerçek uygulamada burayý gerçek API çaðrýsý ile deðiþtirin
                await Task.Delay(500); // API çaðrýsý simülasyonu
                
                // Simüle edilmiþ trafik olaylarý - gerçek uygulamada API yanýtýný kullanýn
                List<TrafficIncident> incidents = new List<TrafficIncident>();
                
                // Olaylarýn sayýsý için rastgele deðer
                int incidentCount = _random.Next(0, 4);
                
                for (int i = 0; i < incidentCount; i++)
                {
                    // Bounding box içinde rastgele konum oluþtur
                    double lat = south + (north - south) * _random.NextDouble();
                    double lng = west + (east - west) * _random.NextDouble();
                    
                    // Rastgele olay türü seç
                    var types = new[] { "ACCIDENT", "CONSTRUCTION", "ROAD_CLOSED", "LANE_CLOSED", "HEAVY_TRAFFIC" };
                    var type = types[_random.Next(types.Length)];
                    
                    // Baþlangýç zamaný - þimdiden 2 saat öncesine kadar
                    var startTime = DateTime.UtcNow.AddMinutes(-1 * _random.Next(1, 120));
                    
                    // Bitiþ zamaný - þimdiden 3 saat sonrasýna kadar (bazýlarý null olabilir)
                    DateTime? endTime = _random.Next(3) == 0 ? 
                        null : // Bazý olaylarýn bitiþ zamaný belli deðil
                        DateTime.UtcNow.AddMinutes(_random.Next(30, 180));
                    
                    // Ciddiyet seviyesi (1: minor, 4: major)
                    int severity = _random.Next(1, 5);
                    
                    incidents.Add(new TrafficIncident
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = type,
                        Description = GetIncidentDescription(type),
                        Latitude = lat,
                        Longitude = lng,
                        StartTime = startTime,
                        EndTime = endTime,
                        SeverityCode = severity
                    });
                }
                
                // Cache'e ekle
                await _cacheLock.WaitAsync();
                try
                {
                    if (!_trafficCache.ContainsKey(cacheKey))
                    {
                        _trafficCache[cacheKey] = new CachedTrafficData 
                        { 
                            Incidents = incidents,
                            CacheTime = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        _trafficCache[cacheKey].Incidents = incidents;
                        _trafficCache[cacheKey].CacheTime = DateTime.UtcNow;
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }
                
                return incidents;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Trafik olaylarý alma hatasý: {ex.Message}");
                return new List<TrafficIncident>();
            }
        }

        public Task<bool> IsTrafficAvailableAsync()
        {
            // Ýnternet baðlantýsýný ve API anahtarýnýn geçerliliðini kontrol et
            return Task.FromResult(_connectivity.NetworkAccess == NetworkAccess.Internet && !string.IsNullOrEmpty(_apiKey));
        }

        public async Task<Route> GetOptimizedRouteWithTrafficAsync(
            Location start, Location end, TransportMode mode, RouteOptimizationPreference preference)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                throw new InvalidOperationException("Ýnternet baðlantýsý yok. Trafik durumuna göre rota hesaplanamýyor.");
                
            try
            {
                // Bu metod, IRoutingService'in CalculateRouteAsync metoduna benzer, 
                // ancak trafik durumunu dikkate alýr.
                // Gerçek implementasyonda, Google Maps Directions API veya 
                // benzer bir API ile trafik durumunu dikkate alan rota hesaplama yapmalýsýnýz.
                
                // API simulasyonu - gerçek uygulamada burayý gerçek API çaðrýsý ile deðiþtirin
                await Task.Delay(800); // API çaðrýsý simülasyonu
                
                // Haversine formülüyle kuþ uçuþu mesafeyi hesapla (gerçek mesafe daha uzun olacak)
                double distanceKm = CalculateDistance(
                    start.Latitude, start.Longitude, 
                    end.Latitude, end.Longitude);
                    
                // Tahmini sürücü hýzýný hesapla
                double baseSpeedKph = mode switch
                {
                    TransportMode.Walking => 5,  // 5 km/saat yürüme hýzý
                    TransportMode.Bicycling => 15, // 15 km/saat bisiklet hýzý
                    TransportMode.Transit => 25, // 25 km/saat toplu taþýma hýzý
                    _ => 50 // 50 km/saat araba hýzý
                };
                
                // Trafik bilgisi al (yalnýzca araba için)
                double trafficMultiplier = 1.0;
                if (mode == TransportMode.Driving)
                {
                    var trafficInfo = await GetTrafficInfoAsync(
                        (start.Latitude + end.Latitude) / 2, 
                        (start.Longitude + end.Longitude) / 2,
                        distanceKm / 2);
                        
                    // Trafik yoðunluðuna göre hýzý ayarla
                    trafficMultiplier = 1.0 + (trafficInfo.CongestionLevel / 100.0);
                }
                
                // Rota tercihi faktörünü hesapla
                double prefFactor = preference switch
                {
                    RouteOptimizationPreference.Optimistic => 0.8, // Optimistik: trafik daha az yoðun
                    RouteOptimizationPreference.Pessimistic => 1.2, // Pesimistik: trafik daha yoðun
                    _ => 1.0 // En iyi tahmin: normal trafik
                };
                
                // Gerçek yol mesafesini tahmin et (kuþ uçuþu mesafenin 1.3 - 1.6 katý)
                double routeFactor = 1.3 + (_random.NextDouble() * 0.3);
                double actualDistanceKm = distanceKm * routeFactor;
                
                // Seyahat süresini hesapla
                double travelTimeHours = (actualDistanceKm / baseSpeedKph) * trafficMultiplier * prefFactor;
                
                // Rota noktalarýný oluþtur (gerçek API bunlarý döndürür)
                List<Location> path = GenerateSimulatedRoutePath(start, end, 20);
                
                // Adýmlarý oluþtur
                List<RouteStep> steps = GenerateSimulatedRouteSteps(path, actualDistanceKm, TimeSpan.FromHours(travelTimeHours));
                
                // Route nesnesini oluþtur
                var route = new Route
                {
                    RouteId = Guid.NewGuid().ToString(),
                    Start = start,
                    End = end,
                    Path = path,
                    Steps = steps,
                    DistanceKm = actualDistanceKm,
                    Duration = TimeSpan.FromHours(travelTimeHours),
                    TransportMode = mode
                };
                
                return route;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Optimize edilmiþ rota hesaplama hatasý: {ex.Message}");
                throw;
            }
        }

        // Yardýmcý metotlar
        private TrafficInfo CreateDefaultTrafficInfo()
        {
            return new TrafficInfo
            {
                CongestionLevel = 0, // Trafik bilgisi yok
                DelayTime = TimeSpan.Zero,
                TypicalTravelTime = TimeSpan.Zero,
                CurrentTravelTime = TimeSpan.Zero,
                LastUpdated = _lastUpdated
            };
        }
        
        private string GetIncidentDescription(string type)
        {
            return type switch
            {
                "ACCIDENT" => "Trafik kazasý",
                "CONSTRUCTION" => "Yol çalýþmasý",
                "ROAD_CLOSED" => "Yol kapalý",
                "LANE_CLOSED" => "Þerit kapalý",
                "HEAVY_TRAFFIC" => "Yoðun trafik",
                _ => "Trafik olayý"
            };
        }
        
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Dünya yarýçapý (km)
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                    
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        
        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
        
        private List<Location> GenerateSimulatedRoutePath(Location start, Location end, int pointCount)
        {
            var path = new List<Location>
            {
                start // Baþlangýç noktasý
            };
            
            for (int i = 1; i < pointCount - 1; i++)
            {
                // Ara noktalar için doðrusal interpolasyon + küçük rastgele sapmalar
                double ratio = (double)i / (pointCount - 1);
                double lat = start.Latitude + (end.Latitude - start.Latitude) * ratio;
                double lon = start.Longitude + (end.Longitude - start.Longitude) * ratio;
                
                // Rastgele sapmalar (gerçekçi yol eðrisi oluþturmak için)
                double offset = 0.005 * Math.Sin(ratio * Math.PI); // Max 0.005 derece (~500m)
                lat += (_random.NextDouble() - 0.5) * offset;
                lon += (_random.NextDouble() - 0.5) * offset;
                
                path.Add(new Location(lat, lon));
            }
            
            path.Add(end); // Bitiþ noktasý
            return path;
        }
        
        private List<RouteStep> GenerateSimulatedRouteSteps(List<Location> path, double totalDistanceKm, TimeSpan totalDuration)
        {
            var steps = new List<RouteStep>();
            
            // Gerçekçi adýmlar oluþtur
            double distanceSoFar = 0;
            TimeSpan timeSoFar = TimeSpan.Zero;
            
            string[] directions = { "saða", "sola", "düz" };
            string[] streetTypes = { "Cadde", "Sokak", "Bulvar", "Yol" };
            string[] streetNames = { "Atatürk", "Cumhuriyet", "Ýstiklal", "Gazi", "Fatih", "Barýþ", "Lale", "Menekþe" };
            
            for (int i = 0; i < path.Count - 1; i++)
            {
                // Bu adýmýn mesafesini hesapla
                double stepDistance = CalculateDistance(
                    path[i].Latitude, path[i].Longitude,
                    path[i + 1].Latitude, path[i + 1].Longitude);
                    
                // Toplam mesafenin yüzdesi
                double distanceRatio = stepDistance / totalDistanceKm;
                
                // Bu adýmýn süresini hesapla
                TimeSpan stepDuration = TimeSpan.FromTicks((long)(totalDuration.Ticks * distanceRatio));
                
                // Kümülatif deðerleri güncelle
                distanceSoFar += stepDistance;
                timeSoFar += stepDuration;
                
                // Manevra türünü belirle
                string maneuverType = i == path.Count - 2 ? "arrive" : 
                                    i == 0 ? "depart" : 
                                    directions[_random.Next(directions.Length)] == "düz" ? "straight" :
                                    directions[_random.Next(directions.Length)] == "saða" ? "turn-right" : "turn-left";
                
                // Rastgele sokak adý oluþtur
                string streetName = $"{streetNames[_random.Next(streetNames.Length)]} {streetTypes[_random.Next(streetTypes.Length)]}";
                
                // Talimat metni oluþtur
                string instruction = i == 0 ? 
                    $"{streetName} üzerinde yolculuða baþlayýn." :
                    i == path.Count - 2 ?
                    "Hedefinize ulaþtýnýz." :
                    $"{directions[_random.Next(directions.Length)]} dönün ve {streetName} üzerinde {stepDistance:F1} km devam edin.";
                
                steps.Add(new RouteStep
                {
                    Instruction = instruction,
                    StartLocation = path[i],
                    EndLocation = path[i + 1],
                    DistanceKm = stepDistance,
                    Duration = stepDuration,
                    ManeuverType = maneuverType
                });
            }
            
            return steps;
        }
    }
}