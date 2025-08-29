using System.Threading.Tasks;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services
{
    public interface IMoonCompassService
    {
        Task<MoonData> GetMoonDataAsync(double latitude, double longitude);
    }
}
