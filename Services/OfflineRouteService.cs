using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.Services
{
    // Basit in-memory implementasyon; ileride SQLite/MBTiles eklenecek
    public class OfflineRouteService : IOfflineRouteService
    {
        private readonly Dictionary<string, Route> _store = new();

        public Task<string> SaveRouteAsync(Route route)
        {
            var id = Guid.NewGuid().ToString();
            _store[id] = route;
            return Task.FromResult(id);
        }

        public Task<Route> LoadRouteAsync(string routeId)
        {
            _store.TryGetValue(routeId, out var r);
            return Task.FromResult(r);
        }

        public Task<List<Route>> GetAllSavedRoutesAsync()
        {
            return Task.FromResult(new List<Route>(_store.Values));
        }

        public Task<bool> HasOfflineRouteAsync(string id)
        {
            return Task.FromResult(_store.ContainsKey(id));
        }
    }
}
