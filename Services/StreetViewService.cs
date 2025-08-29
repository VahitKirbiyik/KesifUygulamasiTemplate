using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Storage;
using System;

namespace KesifUygulamasiTemplate.Services
{
    public class StreetViewService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> GetApiKeyAsync()
        {
            var key = await SecureStorageHelper.GetApiKeyAsync();
            if (string.IsNullOrWhiteSpace(key))
                throw new Exception("API Key bulunamadı!");
            return key;
        }

        public async Task<StreetViewPanorama> GetPanorama(double latitude, double longitude)
        {
            try
            {
                var apiKey = await GetApiKeyAsync();
                var url = $"https://maps.googleapis.com/maps/api/streetview/metadata?location={latitude},{longitude}&key={apiKey}";
                var response = await _httpClient.GetStringAsync(url);
                var json = JsonSerializer.Deserialize<JsonElement>(response);

                return new StreetViewPanorama
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Copyright = json.TryGetProperty("copyright", out var c) ? c.GetString() : "",
                    DateCaptured = json.TryGetProperty("date_captured", out var d) ? DateTime.Parse(d.GetString()) : null,
                    Links = new List<StreetViewLink>() // API’den link ekleme
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Panorama alınamadı: {ex.Message}");
            }
        }

        public async Task<StreetViewPanorama> GetPanoramaByIdAsync(string panoramaId)
        {
            try
            {
                // Örnek olarak sabit veri dönüyoruz
                return new StreetViewPanorama
                {
                    Id = panoramaId,
                    Latitude = 40.7128,
                    Longitude = -74.0060,
                    Links = new List<StreetViewLink>()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Panorama ID ile yüklenemedi: {ex.Message}");
            }
        }
    }
}
