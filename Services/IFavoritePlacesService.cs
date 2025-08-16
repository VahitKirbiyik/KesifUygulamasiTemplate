using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygamamasiTemplate.Services
{
    public interface IFavoritePlacesService
    {
        Task AddFavoriteAsync(string id, Location location, string name);
        Task RemoveFavoriteAsync(string id);
        Task<List<(string id, string name, Location location)>> GetFavoritesAsync();
    }
}
