using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services
{
    public class StreetViewService : IStreetViewService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "YOUR_API_KEY"; // TODO: Gerçek API anahtarı ile değiştir
        private const string BaseUrl = "https://maps.googleapis.com/maps/api/streetview/metadata";

        public StreetViewService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StreetViewPanorama> GetPanoramaAsync(string location)
        {
            try
            {
                // Google Street View Metadata API çağrısı
                var url = $"{BaseUrl}?location={Uri.EscapeDataString(location)}&key={ApiKey}";
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                    return null;
                
                // API yanıtını JSON olarak oku
                var metadata = await response.Content.ReadFromJsonAsync<GoogleStreetViewMetadata>();
                
                if (metadata.Status != "OK")
                    return null;

                // API yanıtını StreetViewPanorama nesnesine dönüştür
                return new StreetViewPanorama
                {
                    PanoramaId = metadata.PanoId,
                    Latitude = metadata.Location?.Lat ?? 0,
                    Longitude = metadata.Location?.Lng ?? 0,
                    DateCaptured = metadata.Date,
                    Copyright = metadata.Copyright
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Street View panorama alma hatası: {ex.Message}");
                return null;
            }
        }

        // API yanıtı modeli (iç sınıf)
        private class GoogleStreetViewMetadata
        {
            public string Status { get; set; }
            public string PanoId { get; set; }
            public DateTime Date { get; set; }
            public string Copyright { get; set; }
            public GoogleLocation Location { get; set; }
        }

        private class GoogleLocation
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }
    }
}
