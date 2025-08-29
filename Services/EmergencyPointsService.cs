using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Networking;
using SQLite;
using KesifUygulamasiTemplate.Models;
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.Services
{
    public class EmergencyPointsService : IEmergencyPointsService
    {
        private readonly HttpClient _httpClient;
        private readonly SQLiteConnection _database;
        private readonly IConnectivity _connectivity;

        public EmergencyPointsService(HttpClient httpClient, SQLiteConnection database, IConnectivity connectivity)
        {
            _httpClient = httpClient;
            _database = database;
            _connectivity = connectivity;
            _database.CreateTable<EmergencyPoint>();
        }

        public Task<IEnumerable<EmergencyPoint>> GetAllEmergencyPointsAsync(int limit = 100)
        {
            var items = _database.Table<EmergencyPoint>().Take(limit).ToList();
            return Task.FromResult<IEnumerable<EmergencyPoint>>(items);
        }

        public async Task<EmergencyPoint?> GetEmergencyPointByIdAsync(int id)
        {
            var item = _database.Table<EmergencyPoint>().FirstOrDefault(x => x.Id == id);
            return await Task.FromResult(item);
        }

        public async Task<IEnumerable<EmergencyPoint>> GetNearbyEmergencyPointsAsync(double latitude, double longitude, double radiusKm = 5, EmergencyPointType type = EmergencyPointType.All)
        {
            var localPoints = _database.Table<EmergencyPoint>()
                .Where(p =>
                    (type == EmergencyPointType.All || p.Type == type) &&
                    CalculateDistance(latitude, longitude, p.Latitude, p.Longitude) <= radiusKm)
                .ToList();

            if (_connectivity.NetworkAccess == NetworkAccess.Internet && (localPoints.Count < 10 || (DateTime.UtcNow - (localPoints.FirstOrDefault()?.LastUpdated ?? DateTime.MinValue)).TotalDays > 7))
            {
                try
                {
                    var url = $"https://api.mapservice.com/pois?lat={latitude}&lng={longitude}&radius={radiusKm}&type={type.ToString().ToLowerInvariant()}";
                    var response = await _httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var onlinePoints = await response.Content.ReadFromJsonAsync<List<EmergencyPoint>>();
                        if (onlinePoints != null)
                        {
                            foreach (var point in onlinePoints)
                            {
                                point.LastUpdated = DateTime.UtcNow;
                                var existing = _database.Table<EmergencyPoint>().FirstOrDefault(p => p.ExternalId == point.ExternalId && point.ExternalId != null);
                                if (existing != null)
                                {
                                    point.Id = existing.Id;
                                    _database.Update(point);
                                }
                                else
                                {
                                    _database.Insert(point);
                                }
                            }

                            return onlinePoints;
                        }
                    }
                }
                catch
                {
                    // ignore and fallback to local
                }
            }

            return localPoints;
        }

        public Task<int> AddEmergencyPointAsync(EmergencyPoint point)
        {
            _database.Insert(point);
            return Task.FromResult(point.Id);
        }

        public Task<bool> UpdateEmergencyPointAsync(EmergencyPoint point)
        {
            var updated = _database.Update(point) > 0;
            return Task.FromResult(updated);
        }

        public Task<bool> RemoveEmergencyPointAsync(int id)
        {
            var removed = _database.Delete<EmergencyPoint>(id) > 0;
            return Task.FromResult(removed);
        }

        public Task<bool> ClearAllEmergencyPointsAsync()
        {
            _database.DeleteAll<EmergencyPoint>();
            return Task.FromResult(true);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula
            const double R = 6371; // km
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double deg) => deg * (Math.PI / 180);
    }
}
