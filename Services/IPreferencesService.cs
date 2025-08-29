using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.Services
{
    public interface IPreferencesService
    {
        Task SetThemeAsync(string theme); // "dark"/"light"/"system"
        Task<string> GetThemeAsync();
        Task SetLanguageAsync(string langCode);
        Task<string> GetLanguageAsync();
        void Set(string key, string value);
        string? Get(string key);
        void Set(string key, bool value);
        bool Get(string key, bool defaultValue);
    }
}
