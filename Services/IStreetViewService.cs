using System.Threading.Tasks;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services
{
    public interface IStreetViewService
    {
        Task<StreetViewPanorama> GetPanoramaAsync(string location);
    }
}