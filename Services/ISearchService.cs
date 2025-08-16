using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygamamasiTemplate.Services
{
    public interface ISearchService
    {
        Task<List<(string id, string name, Location location)>> SearchPlacesAsync(string query, int limit = 10);
        Task<List<string>> GetSearchHistoryAsync();
    }
}
