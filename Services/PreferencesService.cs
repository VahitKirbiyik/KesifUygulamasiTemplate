using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace KesifUygamamasiTemplate.Services
{
    public class PreferencesService : IPreferencesService
    {
        private const string ThemeKey = "App.Theme";
        private const string LangKey = "App.Lang";

        public Task SetThemeAsync(string theme)
        {
            Preferences.Default.Set(ThemeKey, theme);
            return Task.CompletedTask;
        }

        public Task<string> GetThemeAsync()
        {
            return Task.FromResult(Preferences.Default.Get(ThemeKey, "system"));
        }

        public Task SetLanguageAsync(string langCode)
        {
            Preferences.Default.Set(LangKey, langCode);
            return Task.CompletedTask;
        }

        public Task<string> GetLanguageAsync()
        {
            return Task.FromResult(Preferences.Default.Get(LangKey, "tr"));
        }
    }
}
