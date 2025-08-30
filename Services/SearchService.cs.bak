using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace KesifUygamamasiTemplate.Services
{
    public class SearchService : ISearchService
    {
        private readonly List<(string id, string name, Location location)> _sample = new();

        public Task<List<(string id, string name, Location location)>> SearchPlacesAsync(string query, int limit = 10)
        {
            // TODO: Gerçek arama -> provider/SQLite FTS
            var r = _sample.Where(x => x.name.Contains(query, System.StringComparison.OrdinalIgnoreCase)).Take(limit).ToList();
            return Task.FromResult(r);
        }

        public Task<List<string>> GetSearchHistoryAsync()
        {
            return Task.FromResult(new List<string>());
        }
    }
}
