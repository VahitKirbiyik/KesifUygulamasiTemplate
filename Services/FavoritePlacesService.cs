using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;
using KesifUygulamasiTemplate.Services.Interfaces;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services
{
    public class FavoritePlacesService : IFavoritePlacesService
    {
        private readonly Dictionary<string, (string name, Location location)> _store = new();

        public Task AddFavoriteAsync(LocationModel place)
        {
            var location = new Location(place.Latitude, place.Longitude);
            _store[place.Id.ToString()] = (place.Title, location);
            return Task.CompletedTask;
        }

        public Task RemoveFavoriteAsync(string id)
        {
            _store.Remove(id);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<LocationModel>> GetFavoritesAsync()
        {
            var favorites = _store.Select(kv =>
            {
                var (name, location) = kv.Value;
                return new LocationModel
                {
                    Id = int.Parse(kv.Key),
                    Title = name,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                };
            });
            return Task.FromResult(favorites);
        }

        public Task<IEnumerable<LocationModel>> GetAllFavoritePlacesAsync()
        {
            return GetFavoritesAsync();
        }
    }
}
