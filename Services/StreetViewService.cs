using System.Net.Http;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services
{
    public class StreetViewService : IStreetViewService
    {
        private readonly HttpClient _httpClient;

        public StreetViewService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StreetViewPanorama> GetPanoramaAsync(string location)
        {
            // Demo response  gerçek API çağrısı buraya gelecek
            var panorama = new StreetViewPanorama
            {
                PanoramaId = "demo123",
                Latitude = 41.0082,
                Longitude = 28.9784,
                Heading = 90,
                Pitch = 0
            };

            return await Task.FromResult(panorama);
        }
    }
}
