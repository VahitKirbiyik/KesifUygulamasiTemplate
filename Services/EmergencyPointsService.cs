using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Models;
using Microsoft.Maui.Networking;
using SQLite;

namespace KesifUygulamasiTemplate.Services
{
    public class EmergencyPointsService : IEmergencyPointsService
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly IConnectivity _connectivity;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public EmergencyPointsService(SQLiteAsyncConnection database, IConnectivity connectivity, HttpClient httpClient)
        {
            _database = database;
            _connectivity = connectivity;
            _httpClient = httpClient;
            _apiBaseUrl = "https://api.example.com/emergency-points"; // Ger�ek API URL'nizi buraya ekleyin
            
            // Veritaban� tablosunu olu�tur
            _database.CreateTableAsync<EmergencyPoint>().Wait();
        }
        
        public async Task<IEnumerable<EmergencyPoint>> GetAllEmergencyPointsAsync(int limit = 100)
        {
            try
            {
                return await _database.Table<EmergencyPoint>()
                    .OrderBy(p => p.Name)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Acil durum noktalar�n� getirme hatas�: {ex.Message}");
                return Enumerable.Empty<EmergencyPoint>();
            }
        }
        
        public async Task<EmergencyPoint> GetEmergencyPointByIdAsync(int id)
        {
            try
            {
                return await _database.GetAsync<EmergencyPoint>(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ID ile acil durum noktas� getirme hatas�: {ex.Message}");
                return null;
            }
        }
        
        public async Task<IEnumerable<EmergencyPoint>> GetNearbyEmergencyPointsAsync(
            double latitude, double longitude, double radiusKm = 5, EmergencyPointType type = EmergencyPointType.All)
        {
            try
            {
                // �nce �evrimd��� veritaban�ndan noktalar� al
                var points = await _database.Table<EmergencyPoint>().ToListAsync();
                
                // T�r filtrelemesi
                if (type != EmergencyPointType.All)
                {
                    points = points.Where(p => p.Type == type).ToList();
                }
                
                // Mesafe hesaplama ve filtreleme
                var nearbyPoints = points
                    .Select(p => new
                    {
                        Point = p,
                        Distance = CalculateDistance(latitude, longitude, p.Latitude, p.Longitude)
                    })
                    .Where(x => x.Distance <= radiusKm)
                    .OrderBy(x => x.Distance)
                    .Select(x => x.Point)
                    .ToList();
                
                // �evrimi�i veri alma (e�er internet varsa ve yetersiz veri varsa)
                if (_connectivity.NetworkAccess == NetworkAccess.Internet && nearbyPoints.Count < 10)
                {
                    await SynchronizeEmergencyPointsAsync(latitude, longitude, radiusKm, type);
                    
                    // Tekrar �evrimd��� veritaban�ndan al (g�ncellenmi�)
                    points = await _database.Table<EmergencyPoint>().ToListAsync();
                    
                    if (type != EmergencyPointType.All)
                    {
                        points = points.Where(p => p.Type == type).ToList();
                    }
                    
                    nearbyPoints = points
                        .Select(p => new
                        {
                            Point = p,
                            Distance = CalculateDistance(latitude, longitude, p.Latitude, p.Longitude)
                        })
                        .Where(x => x.Distance <= radiusKm)
                        .OrderBy(x => x.Distance)
                        .Select(x => x.Point)
                        .ToList();
                }
                
                return nearbyPoints;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Yak�ndaki acil durum noktalar�n� getirme hatas�: {ex.Message}");
                return Enumerable.Empty<EmergencyPoint>();
            }
        }
        
        public async Task<int> AddEmergencyPointAsync(EmergencyPoint point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));
            
            await _semaphore.WaitAsync();
            try
            {
                point.CreatedAt = DateTime.UtcNow;
                return await _database.InsertAsync(point);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Acil durum noktas� ekleme hatas�: {ex.Message}");
                return -1;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        public async Task<bool> UpdateEmergencyPointAsync(EmergencyPoint point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));
                
            if (point.Id <= 0)
                throw new ArgumentException("G�ncellenecek acil durum noktas�n�n ge�erli bir ID'si olmal�d�r.");
            
            await _semaphore.WaitAsync();
            try
            {
                point.UpdatedAt = DateTime.UtcNow;
                int result = await _database.UpdateAsync(point);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Acil durum noktas� g�ncelleme hatas�: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        public async Task<bool> RemoveEmergencyPointAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Ge�ersiz ID.");
            
            await _semaphore.WaitAsync();
            try
            {
                int result = await _database.DeleteAsync<EmergencyPoint>(id);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Acil durum noktas� silme hatas�: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        public async Task<bool> ClearAllEmergencyPointsAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                await _database.DeleteAllAsync<EmergencyPoint>();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"T�m acil durum noktalar�n� temizleme hatas�: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        // Yard�mc� metotlar
        private async Task<bool> SynchronizeEmergencyPointsAsync(
            double latitude, double longitude, double radiusKm, EmergencyPointType type)
        {
            try
            {
                string typeParam = type == EmergencyPointType.All ? "" : $"&type={type.ToString().ToLower()}";
                string url = $"{_apiBaseUrl}?lat={latitude}&lon={longitude}&radius={radiusKm}{typeParam}";
                
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var points = JsonSerializer.Deserialize<List<EmergencyPoint>>(content);
                    
                    if (points != null && points.Count > 0)
                    {
                        await _semaphore.WaitAsync();
                        try
                        {
                            foreach (var point in points)
                            {
                                var existingPoint = await _database.Table<EmergencyPoint>()
                                    .Where(p => p.ExternalId == point.ExternalId)
                                    .FirstOrDefaultAsync();
                                
                                if (existingPoint != null)
                                {
                                    existingPoint.Name = point.Name;
                                    existingPoint.Description = point.Description;
                                    existingPoint.Address = point.Address;
                                    existingPoint.PhoneNumber = point.PhoneNumber;
                                    existingPoint.WebsiteUrl = point.WebsiteUrl;
                                    existingPoint.IsVerified = point.IsVerified;
                                    existingPoint.UpdatedAt = DateTime.UtcNow;
                                    await _database.UpdateAsync(existingPoint);
                                }
                                else
                                {
                                    point.CreatedAt = DateTime.UtcNow;
                                    await _database.InsertAsync(point);
                                }
                            }
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }
                    
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Acil durum noktalar� senkronizasyon hatas�: {ex.Message}");
                return false;
            }
        }
        
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // D�nya yar��ap� (km)
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                    
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        
        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}