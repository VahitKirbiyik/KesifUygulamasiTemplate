using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using KesifUygulamasiTemplate.Models;
using KesifUygulamasiTemplate.Services;
using KesifUygulamasiTemplate.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KesifUygulamasiTemplate.Services
{
    /// <summary>
    /// İki konum arası rota bilgisini sağlayan servis implementasyonu
    /// Google Maps Directions API ve Mapbox Directions API desteği
    /// </summary>
    public class RouteService : IRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly ConnectivityService _connectivityService;
        private readonly string _googleMapsApiKey;
        private readonly string _mapboxApiKey;
        private const string GOOGLE_MAPS_API_URL = "https://maps.googleapis.com/maps/api/directions/json";
        private const string MAPBOX_API_URL = "https://api.mapbox.com/directions/v5/mapbox/driving";

        public RouteService(HttpClient httpClient, ConnectivityService connectivityService)
        {
            _httpClient = httpClient;
            _connectivityService = connectivityService;

            // API anahtarları - gerçek uygulamada secure storage'dan alınmalı
            _googleMapsApiKey = "YOUR_GOOGLE_MAPS_API_KEY"; // Environment variable'dan alınmalı
            _mapboxApiKey = "YOUR_MAPBOX_API_KEY"; // Environment variable'dan alınmalı
        }

        /// <summary>
        /// Gerçek rota API'leri kullanarak rota hesaplar
        /// </summary>
        public async Task<List<LocationModel>> GetRouteAsync(LocationModel start, LocationModel end)
        {
            // Offline kontrolü
            if (!_connectivityService.IsConnected)
            {
                return await GetOfflineRouteAsync(start, end);
            }

            try
            {
                // Önce Google Maps API dene
                var route = await GetGoogleMapsRouteAsync(start, end);
                if (route != null && route.Count > 0)
                    return route;

                // Google Maps başarısız olursa Mapbox dene
                route = await GetMapboxRouteAsync(start, end);
                if (route != null && route.Count > 0)
                    return route;

                // Her iki API de başarısız olursa basit rota döndür
                return await GetSimpleRouteAsync(start, end);
            }
            catch (Exception ex)
            {
                // Hata durumunda basit rota
                System.Diagnostics.Debug.WriteLine($"Rota hesaplanırken hata: {ex.Message}");
                return await GetSimpleRouteAsync(start, end);
            }
        }

        /// <summary>
        /// Google Maps Directions API ile rota hesaplar
        /// </summary>
        private async Task<List<LocationModel>> GetGoogleMapsRouteAsync(LocationModel start, LocationModel end)
        {
            if (string.IsNullOrEmpty(_googleMapsApiKey) || _googleMapsApiKey.Contains("YOUR_"))
                return null;

            var url = $"{GOOGLE_MAPS_API_URL}?origin={start.Latitude},{start.Longitude}&destination={end.Latitude},{end.Longitude}&key={_googleMapsApiKey}&mode=driving";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var googleResponse = JsonSerializer.Deserialize<GoogleMapsResponse>(content);

            if (googleResponse?.Status != "OK" || googleResponse.Routes.Length == 0)
                return null;

            var route = new List<LocationModel>();
            var overviewPath = googleResponse.Routes[0].OverviewPolyline.Points;

            // Polyline decode işlemi burada yapılmalı
            // Şimdilik basit noktalar döndürüyoruz
            route.Add(start);
            route.Add(end);

            return route;
        }

        /// <summary>
        /// Mapbox Directions API ile rota hesaplar
        /// </summary>
        private async Task<List<LocationModel>> GetMapboxRouteAsync(LocationModel start, LocationModel end)
        {
            if (string.IsNullOrEmpty(_mapboxApiKey) || _mapboxApiKey.Contains("YOUR_"))
                return null;

            var url = $"{MAPBOX_API_URL}/{start.Longitude},{start.Latitude};{end.Longitude},{end.Latitude}?access_token={_mapboxApiKey}&geometries=geojson";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var mapboxResponse = JsonSerializer.Deserialize<MapboxResponse>(content);

            if (mapboxResponse?.Routes.Length == 0)
                return null;

            var route = new List<LocationModel>();
            var coordinates = mapboxResponse.Routes[0].Geometry.Coordinates;

            foreach (var coord in coordinates)
            {
                route.Add(new LocationModel
                {
                    Latitude = coord[1],
                    Longitude = coord[0]
                });
            }

            return route;
        }

        /// <summary>
        /// Offline durumda basit rota hesaplar
        /// </summary>
        private async Task<List<LocationModel>> GetOfflineRouteAsync(LocationModel start, LocationModel end)
        {
            // Offline önbellekten rota kontrolü
            // Gerçek implementasyonda veritabanından rota çekilmeli
            return await GetSimpleRouteAsync(start, end);
        }

        /// <summary>
        /// Basit doğrusal rota hesaplar (fallback)
        /// </summary>
        private async Task<List<LocationModel>> GetSimpleRouteAsync(LocationModel start, LocationModel end)
        {
            await Task.Delay(100); // Simüle edilmiş ağ gecikmesi

            var route = new List<LocationModel>();

            // Başlangıç noktasını ekleyin
            route.Add(start);

            // İki nokta arasında birkaç ara nokta oluşturun
            int steps = 10;
            for (int i = 1; i < steps; i++)
            {
                double factor = (double)i / steps;
                double latitude = start.Latitude + (end.Latitude - start.Latitude) * factor;
                double longitude = start.Longitude + (end.Longitude - start.Longitude) * factor;
                route.Add(new LocationModel { Latitude = latitude, Longitude = longitude });
            }

            // Varış noktasını ekleyin
            route.Add(end);

            return route;
        }
    }

    // API Response modelleri
    public class GoogleMapsResponse
    {
        public string Status { get; set; }
        public GoogleRoute[] Routes { get; set; }
    }

    public class GoogleRoute
    {
        public GooglePolyline OverviewPolyline { get; set; }
    }

    public class GooglePolyline
    {
        public string Points { get; set; }
    }

    public class MapboxResponse
    {
        public MapboxRoute[] Routes { get; set; }
    }

    public class MapboxRoute
    {
        public MapboxGeometry Geometry { get; set; }
    }

    public class MapboxGeometry
    {
        public double[][] Coordinates { get; set; }
    }
}
