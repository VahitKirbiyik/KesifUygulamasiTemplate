using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygamamasiTemplate.Services
{
    public class FavoritePlacesService : IFavoritePlacesService
    {
        private readonly Dictionary<string, (string name, Location location)> _store = new();

        public Task AddFavoriteAsync(string id, Location location, string name)
        {
            _store[id] = (name, location);
            return Task.CompletedTask;
        }

        public Task RemoveFavoriteAsync(string id)
        {
            _store.Remove(id);
            return Task.CompletedTask;
        }

        public Task<List<(string id, string name, Location location)>> GetFavoritesAsync()
        {
            return Task.FromResult(_store.Select(kv => (kv.Key, kv.Value.name, kv.Value.location)).ToList());
        }
    }
}
