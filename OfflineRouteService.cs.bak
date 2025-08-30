using SQLite;
using KesifUygulamasiTemplate.Models;
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.Services
{
    // Services/OfflineRouteService.cs
    public class OfflineRouteService : IOfflineRouteService
    {
        public OfflineRouteService(SQLiteConnection database)
        {
            // Stub implementation
        }
        
        public async Task<bool> HasOfflineRouteAsync(string id)
        {
            // Stub implementation
            return await Task.FromResult(false);
        }
        
        public async Task<string> SaveRouteAsync(Route route)
        {
            // Stub implementation
            return await Task.FromResult("1");
        }
        
        public async Task<Route> LoadRouteAsync(string routeId)
        {
            // Stub implementation
            await Task.CompletedTask;
            throw new KeyNotFoundException($"Route with ID {routeId} not found");
        }
        
        public async Task<List<Route>> GetAllSavedRoutesAsync()
        {
            // Stub implementation
            return await Task.FromResult(new List<Route>());
        }
    }
}