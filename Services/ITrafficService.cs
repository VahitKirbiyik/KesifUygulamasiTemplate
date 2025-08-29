using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Extensions.Configuration;
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
        private readonly string? _apiKey;
        private readonly Random _random; // Simülasyon için
        private DateTime _lastUpdated;
        private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);
        private Dictionary<string, CachedTrafficData> _trafficCache = new Dictionary<string, CachedTrafficData>();

        // Cache verilerini tutan yardımcı sınıf
        private class CachedTrafficData
        {
            public TrafficInfo? TrafficInfo { get; set; }
            public List<TrafficIncident>? Incidents { get; set; }
            public DateTime CacheTime { get; set; }
        }

        public TrafficService(HttpClient httpClient, IConnectivity connectivity, IGeolocation geolocation, IConfiguration config)
        {
            _httpClient = httpClient;
            _connectivity = connectivity;
            _geolocation = geolocation;
            _apiKey = config["MapServices:ApiKey"] ?? string.Empty;
            _random = new Random();
            _lastUpdated = DateTime.UtcNow;
        }

        public async Task<TrafficInfo> GetTrafficInfoAsync(double latitude, double longitude, double radius = 5.0)
        {
            // �nternet ba�lant�s� kontrol�
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return CreateDefaultTrafficInfo();

            try
            {
                // Cache kontrolü
                string cacheKey = $"traffic_{latitude:F3}_{longitude:F3}_{radius:F1}";

                await _cacheLock.WaitAsync();
                try
                {
                    // Cache'de varsa ve son 5 dakika içinde güncellenmiş ise cache'den döndür
                    if (_trafficCache.TryGetValue(cacheKey, out var cachedData) &&
                        (DateTime.UtcNow - cachedData.CacheTime).TotalMinutes < 5 &&
                        cachedData.TrafficInfo != null)
                    {
                        return cachedData.TrafficInfo;
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }

                // Gerçek API çağrısı burada olacak
                // Örnek: Google Maps Distance Matrix API veya alternatif bir trafik API'si

                // API simulasyonu - ger�ek uygulamada buray� ger�ek API �a�r�s� ile de�i�tirin
                await Task.Delay(300); // API �a�r�s� sim�lasyonu

                // G�n�n saatine g�re trafik yo�unlu�unu de�i�tir (sabah ve ak�am yo�un)
                var hour = DateTime.Now.Hour;
                int congestionBase = 20; // Normal zamanlarda temel yo�unluk

                // Sabah ve ak�am trafik yo�unlu�u artar
                if ((hour >= 7 && hour <= 9) || (hour >= 17 && hour <= 19))
                {
                    congestionBase = 70; // Yo�un trafik
                }
                else if ((hour >= 10 && hour <= 16) || (hour >= 20 && hour <= 21))
                {
                    congestionBase = 40; // Orta yo�unlukta trafik
                }

                // Random dalgalanmalar ekle
                int congestionLevel = Math.Min(100, Math.Max(0, congestionBase + _random.Next(-15, 16)));

                // Trafik bilgisini olu�tur
                var trafficInfo = new TrafficInfo
                {
                    CongestionLevel = congestionLevel.ToString(),
                    DelayTime = TimeSpan.FromMinutes(congestionLevel / 10.0 * 2), // Her 10 birim yo�unluk i�in 2 dakika gecikme
                    TypicalTravelTime = TimeSpan.FromMinutes(radius * 2), // Yakla��k hesaplama
                    CurrentTravelTime = TimeSpan.FromMinutes(radius * 2 * (1 + congestionLevel / 100.0)), // Trafik yo�unlu�una g�re art�r�lm�� s�re
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
                        // En eski 20 ��eyi sil
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
                Console.WriteLine($"Trafik bilgisi alma hatas�: {ex.Message}");
                return CreateDefaultTrafficInfo();
            }
        }

        public async Task<List<TrafficIncident>> GetTrafficIncidentsAsync(double north, double south, double east, double west)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return new List<TrafficIncident>();

            try
            {
                // Cache kontrol� i�in bounding box keyleri
                string cacheKey = $"incidents_{north:F3}_{south:F3}_{east:F3}_{west:F3}";

                await _cacheLock.WaitAsync();
                try
                {
                    // Cache'de varsa ve son 5 dakika i�inde g�ncellenmi� ise cache'den d�nd�r
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

                // Ger�ek API �a�r�s� burada olacak
                // �rnek: HERE, TomTom veya Google Maps trafik olaylar� API'si

                // API simulasyonu - ger�ek uygulamada buray� ger�ek API �a�r�s� ile de�i�tirin
                await Task.Delay(500); // API �a�r�s� sim�lasyonu

                // Sim�le edilmi� trafik olaylar� - ger�ek uygulamada API yan�t�n� kullan�n
                List<TrafficIncident> incidents = new List<TrafficIncident>();

                // Olaylar�n say�s� i�in rastgele de�er
                int incidentCount = _random.Next(0, 4);

                for (int i = 0; i < incidentCount; i++)
                {
                    // Bounding box i�inde rastgele konum olu�tur
                    double lat = south + (north - south) * _random.NextDouble();
                    double lng = west + (east - west) * _random.NextDouble();

                    // Rastgele olay t�r� se�
                    var types = new[] { "ACCIDENT", "CONSTRUCTION", "ROAD_CLOSED", "LANE_CLOSED", "HEAVY_TRAFFIC" };
                    var type = types[_random.Next(types.Length)];

                    // Ba�lang�� zaman� - �imdiden 2 saat �ncesine kadar
                    var startTime = DateTime.UtcNow.AddMinutes(-1 * _random.Next(1, 120));

                    // Biti� zaman� - �imdiden 3 saat sonras�na kadar (baz�lar� null olabilir)
                    DateTime? endTime = _random.Next(3) == 0 ?
                        null : // Baz� olaylar�n biti� zaman� belli de�il
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
                Console.WriteLine($"Trafik olaylar� alma hatas�: {ex.Message}");
                return new List<TrafficIncident>();
            }
        }

        public Task<bool> IsTrafficAvailableAsync()
        {
            // �nternet ba�lant�s�n� ve API anahtar�n�n ge�erlili�ini kontrol et
            return Task.FromResult(_connectivity.NetworkAccess == NetworkAccess.Internet && !string.IsNullOrEmpty(_apiKey));
        }

        public async Task<Route> GetOptimizedRouteWithTrafficAsync(
            Location start, Location end, TransportMode mode, RouteOptimizationPreference preference)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                throw new InvalidOperationException("�nternet ba�lant�s� yok. Trafik durumuna g�re rota hesaplanam�yor.");

            try
            {
                // Bu metod, IRoutingService'in CalculateRouteAsync metoduna benzer, 
                // ancak trafik durumunu dikkate al�r.
                // Ger�ek implementasyonda, Google Maps Directions API veya 
                // benzer bir API ile trafik durumunu dikkate alan rota hesaplama yapmal�s�n�z.

                // API simulasyonu - ger�ek uygulamada buray� ger�ek API �a�r�s� ile de�i�tirin
                await Task.Delay(800); // API �a�r�s� sim�lasyonu

                // Haversine form�l�yle ku� u�u�u mesafeyi hesapla (ger�ek mesafe daha uzun olacak)
                double distanceKm = CalculateDistance(
                    start.Latitude, start.Longitude,
                    end.Latitude, end.Longitude);

                // Tahmini s�r�c� h�z�n� hesapla
                double baseSpeedKph = mode switch
                {
                    TransportMode.Walking => 5,  // 5 km/saat y�r�me h�z�
                    TransportMode.Bicycling => 15, // 15 km/saat bisiklet h�z�
                    TransportMode.Transit => 25, // 25 km/saat toplu ta��ma h�z�
                    _ => 50 // 50 km/saat araba h�z�
                };

                // Trafik bilgisi al (yaln�zca araba i�in)
                double trafficMultiplier = 1.0;
                if (mode == TransportMode.Driving)
                {
                    var trafficInfo = await GetTrafficInfoAsync(
                        (start.Latitude + end.Latitude) / 2,
                        (start.Longitude + end.Longitude) / 2,
                        distanceKm / 2);

                    // Trafik yo�unlu�una g�re h�z� ayarla
                    trafficMultiplier = 1.0 + (double.Parse(trafficInfo.CongestionLevel) / 100.0);
                }

                // Rota tercihi fakt�r�n� hesapla
                double prefFactor = preference switch
                {
                    RouteOptimizationPreference.Optimistic => 0.8, // Optimistik: trafik daha az yo�un
                    RouteOptimizationPreference.Pessimistic => 1.2, // Pesimistik: trafik daha yo�un
                    _ => 1.0 // En iyi tahmin: normal trafik
                };

                // Ger�ek yol mesafesini tahmin et (ku� u�u�u mesafenin 1.3 - 1.6 kat�)
                double routeFactor = 1.3 + (_random.NextDouble() * 0.3);
                double actualDistanceKm = distanceKm * routeFactor;

                // Seyahat s�resini hesapla
                double travelTimeHours = (actualDistanceKm / baseSpeedKph) * trafficMultiplier * prefFactor;

                // Rota noktalar�n� olu�tur (ger�ek API bunlar� d�nd�r�r)
                List<Location> path = GenerateSimulatedRoutePath(start, end, 20);

                // Ad�mlar� olu�tur
                List<RouteStep> steps = GenerateSimulatedRouteSteps(path, actualDistanceKm, TimeSpan.FromHours(travelTimeHours));

                // Route nesnesini olu�tur
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
                Console.WriteLine($"Optimize edilmiş rota hesaplama hatası: {ex.Message}");
                throw;
            }
        }

        // Yardımcı metotlar
        private TrafficInfo CreateDefaultTrafficInfo()
        {
            return new TrafficInfo
            {
                CongestionLevel = "Low", // Trafik bilgisi yok
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
                "ACCIDENT" => "Trafik kazası",
                "CONSTRUCTION" => "Yol çalışması",
                "ROAD_CLOSED" => "Yol kapalı",
                "LANE_CLOSED" => "Şerit kapalı",
                "HEAVY_TRAFFIC" => "Yoğun trafik",
                _ => "Trafik olayı"
            };
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Dünya yarıçapı (km)
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
                start // Başlangıç noktası
            };

            for (int i = 1; i < pointCount - 1; i++)
            {
                // Ara noktalar için doğrusal interpolasyon + küçük rastgele sapmalar
                double ratio = (double)i / (pointCount - 1);
                double lat = start.Latitude + (end.Latitude - start.Latitude) * ratio;
                double lon = start.Longitude + (end.Longitude - start.Longitude) * ratio;

                // Rastgele sapmalar (gerçekçi yol eğrisi oluşturmak için)
                double offset = 0.005 * Math.Sin(ratio * Math.PI); // Max 0.005 derece (~500m)
                lat += (_random.NextDouble() - 0.5) * offset;
                lon += (_random.NextDouble() - 0.5) * offset;

                path.Add(new Location(lat, lon));
            }

            path.Add(end); // Bitiş noktası
            return path;
        }

        private List<RouteStep> GenerateSimulatedRouteSteps(List<Location> path, double totalDistanceKm, TimeSpan totalDuration)
        {
            var steps = new List<RouteStep>();

            // Gerçekçi adımlar oluştur
            double distanceSoFar = 0;
            TimeSpan timeSoFar = TimeSpan.Zero;

            string[] directions = { "sağa", "sola", "düz" };
            string[] streetTypes = { "Cadde", "Sokak", "Bulvar", "Yol" };
            string[] streetNames = { "Atatürk", "Cumhuriyet", "İstiklal", "Gazi", "Fatih", "Barış", "Lale", "Menekşe" };

            for (int i = 0; i < path.Count - 1; i++)
            {
                // Bu adımın mesafesini hesapla
                double stepDistance = CalculateDistance(
                    path[i].Latitude, path[i].Longitude,
                    path[i + 1].Latitude, path[i + 1].Longitude);

                // Toplam mesafenin yüzdesi
                double distanceRatio = stepDistance / totalDistanceKm;

                // Bu adımın süresini hesapla
                TimeSpan stepDuration = TimeSpan.FromTicks((long)(totalDuration.Ticks * distanceRatio));

                // Kümülatif değerleri güncelle
                distanceSoFar += stepDistance;
                timeSoFar += stepDuration;

                // Manevra türünü belirle
                string maneuverType = i == path.Count - 2 ? "arrive" :
                                    i == 0 ? "depart" :
                                    directions[_random.Next(directions.Length)] == "düz" ? "straight" :
                                    directions[_random.Next(directions.Length)] == "sağa" ? "turn-right" : "turn-left";

                // Rastgele sokak adı oluştur
                string streetName = $"{streetNames[_random.Next(streetNames.Length)]} {streetTypes[_random.Next(streetTypes.Length)]}";

                // Talimat metni oluştur
                string instruction = i == 0 ?
                    $"{streetName} üzerinde yolculuğa başlayın." :
                    i == path.Count - 2 ?
                    "Hedefinize ulaştınız." :
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
