using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Storage;
using System;
using KesifUygulamasiTemplate.Models;
using KesifUygulamasiTemplate.Helpers;
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.Services
{
    public class StreetViewService : IStreetViewService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> GetApiKeyAsync()
        {
            var key = await SecureStorageHelper.GetApiKeyAsync();
            if (string.IsNullOrWhiteSpace(key))
                throw new Exception("API Key bulunamadı!");
            return key;
        }

        public async Task<StreetViewPanorama> GetPanoramaAsync(string location)
        {
            // Location'dan latitude ve longitude çıkar
            var parts = location.Split(',');
            if (parts.Length != 2 || !double.TryParse(parts[0], out var lat) || !double.TryParse(parts[1], out var lng))
            {
                throw new ArgumentException("Invalid location format. Expected 'latitude,longitude'");
            }
            return await GetPanorama(lat, lng);
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
                    Copyright = json.TryGetProperty("copyright", out var c) ? c.GetString() ?? "" : "",
                    DateCaptured = json.TryGetProperty("date_captured", out var d) && d.GetString() is string dateStr ? DateTime.Parse(dateStr) : null,
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
                return await Task.FromResult(new StreetViewPanorama
                {
                    Id = panoramaId,
                    Latitude = 40.7128,
                    Longitude = -74.0060,
                    Links = new List<StreetViewLink>()
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Panorama ID ile yüklenemedi: {ex.Message}");
            }
        }
    }
}
