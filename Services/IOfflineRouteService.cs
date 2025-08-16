using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Models;
using Microsoft.Maui.Devices.Sensors;
using SQLite;

namespace KesifUygulamasiTemplate.Services
{
    public interface IOfflineRouteService
    {
        Task<int> SaveRouteAsync(SavedRoute route, List<Location> points);
        Task<SavedRoute> GetRouteByIdAsync(int id);
        Task<List<SavedRoute>> GetAllRoutesAsync();
        Task<bool> DeleteRouteAsync(int id);
        Task<Route> ConvertToRouteAsync(SavedRoute savedRoute);
    }

    public class OfflineRouteService : IOfflineRouteService
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public OfflineRouteService(SQLiteAsyncConnection database)
        {
            _database = database;
            
            // Veritaban� tablolar�n� olu�tur
            _database.CreateTableAsync<SavedRoute>().Wait();
            _database.CreateTableAsync<RoutePoint>().Wait();
        }
        
        public async Task<int> SaveRouteAsync(SavedRoute route, List<Location> points)
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
                
            if (points == null || points.Count < 2)
                throw new ArgumentException("Rota en az iki nokta i�ermelidir.");
            
            await _semaphore.WaitAsync();
            try
            {
                // Rota ba�lang�� ve biti� noktalar�n� ayarla
                route.StartLatitude = points.First().Latitude;
                route.StartLongitude = points.First().Longitude;
                route.EndLatitude = points.Last().Latitude;
                route.EndLongitude = points.Last().Longitude;
                
                // �nce rota bilgisini kaydet
                int routeId;
                if (route.Id == 0)
                {
                    route.CreatedAt = DateTime.UtcNow;
                    routeId = await _database.InsertAsync(route);
                }
                else
                {
                    routeId = route.Id;
                    route.UpdatedAt = DateTime.UtcNow;
                    await _database.UpdateAsync(route);
                    
                    // Mevcut noktalar� sil
                    await _database.Table<RoutePoint>()
                        .Where(p => p.RouteId == routeId)
                        .DeleteAsync();
                }
                
                // Sonra rota noktalar�n� kaydet
                for (int i = 0; i < points.Count; i++)
                {
                    await _database.InsertAsync(new RoutePoint
                    {
                        RouteId = routeId,
                        Latitude = points[i].Latitude,
                        Longitude = points[i].Longitude,
                        Sequence = i
                    });
                }
                
                return routeId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rota kaydetme hatas�: {ex.Message}");
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        public async Task<SavedRoute> GetRouteByIdAsync(int id)
        {
            try
            {
                var route = await _database.GetAsync<SavedRoute>(id);
                if (route != null)
                {
                    // Rota noktalar�n� y�kle
                    var points = await _database.Table<RoutePoint>()
                        .Where(p => p.RouteId == id)
                        .OrderBy(p => p.Sequence)
                        .ToListAsync();
                    
                    route.Points = points.Select(p => new Location(p.Latitude, p.Longitude)).ToList();
                }
                
                return route;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rota getirme hatas� (ID={id}): {ex.Message}");
                return null;
            }
        }
        
        public async Task<List<SavedRoute>> GetAllRoutesAsync()
        {
            try
            {
                var routes = await _database.Table<SavedRoute>()
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
                
                // Her bir rota i�in noktalar� ayr� ayr� y�kle
                foreach (var route in routes)
                {
                    var points = await _database.Table<RoutePoint>()
                        .Where(p => p.RouteId == route.Id)
                        .OrderBy(p => p.Sequence)
                        .ToListAsync();
                    
                    route.Points = points.Select(p => new Location(p.Latitude, p.Longitude)).ToList();
                }
                
                return routes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"T�m rotalar� getirme hatas�: {ex.Message}");
                return new List<SavedRoute>();
            }
        }
        
        public async Task<bool> DeleteRouteAsync(int id)
        {
            await _semaphore.WaitAsync();
            try
            {
                // �nce rota noktalar�n� sil
                await _database.Table<RoutePoint>()
                    .Where(p => p.RouteId == id)
                    .DeleteAsync();
                
                // Sonra rotay� sil
                int result = await _database.DeleteAsync<SavedRoute>(id);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rota silme hatas� (ID={id}): {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        public async Task<Route> ConvertToRouteAsync(SavedRoute savedRoute)
        {
            if (savedRoute == null)
                throw new ArgumentNullException(nameof(savedRoute));
                
            try
            {
                if (savedRoute.Points == null || savedRoute.Points.Count < 2)
                {
                    // Noktalar� y�kle (e�er daha �nce y�klenmemi�se)
                    var points = await _database.Table<RoutePoint>()
                        .Where(p => p.RouteId == savedRoute.Id)
                        .OrderBy(p => p.Sequence)
                        .ToListAsync();
                        
                    savedRoute.Points = points.Select(p => new Location(p.Latitude, p.Longitude)).ToList();
                }
                
                if (savedRoute.Points.Count < 2)
                    throw new InvalidOperationException("Rota en az iki nokta i�ermelidir.");
                    
                var startLocation = new Location(savedRoute.StartLatitude, savedRoute.StartLongitude);
                var endLocation = new Location(savedRoute.EndLatitude, savedRoute.EndLongitude);
                
                var route = new Route
                {
                    RouteId = savedRoute.Id.ToString(),
                    Start = startLocation,
                    End = endLocation,
                    Path = savedRoute.Points,
                    DistanceKm = savedRoute.DistanceKm,
                    Duration = savedRoute.EstimatedDuration,
                    TransportMode = Enum.Parse<TransportMode>(savedRoute.TransportMode)
                };
                
                // Ad�mlar� olu�tur (basit bir yakla��m)
                route.Steps = new List<RouteStep>();
                for (int i = 0; i < savedRoute.Points.Count - 1; i++)
                {
                    double stepDistance = CalculateDistance(
                        savedRoute.Points[i].Latitude, savedRoute.Points[i].Longitude,
                        savedRoute.Points[i + 1].Latitude, savedRoute.Points[i + 1].Longitude);
                        
                    route.Steps.Add(new RouteStep
                    {
                        StartLocation = savedRoute.Points[i],
                        EndLocation = savedRoute.Points[i + 1],
                        Instruction = $"Ad�m {i + 1}",
                        ManeuverType = "straight",
                        DistanceKm = stepDistance,
                        Duration = TimeSpan.FromSeconds(stepDistance * 60) // Yakla��k 60 km/h h�z varsay�m�
                    });
                }
                
                return route;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rota d�n��t�rme hatas�: {ex.Message}");
                throw;
            }
        }
        
        // Yard�mc� metotlar
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // D�nya yar��ap� (km)
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
    }
}