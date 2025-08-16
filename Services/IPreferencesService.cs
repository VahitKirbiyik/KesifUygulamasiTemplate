using System.Threading.Tasks;

namespace KesifUygamamasiTemplate.Services
{
    public interface IPreferencesService
    {
        Task SetThemeAsync(string theme); // "dark"/"light"/"system"
        Task<string> GetThemeAsync();
        Task SetLanguageAsync(string langCode);
        Task<string> GetLanguageAsync();
    }
}
