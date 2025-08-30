using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Media;
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.Services
{
    public class NavigationVoiceService : INavigationVoiceService
    {
        public async Task SpeakAsync(string text)
        {
            await TextToSpeech.Default.SpeakAsync(text);
        }

        public async Task<string[]> GetAvailableLocalesAsync()
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            return locales.Select(l => l.Language).ToArray();
        }

        public async Task SetPreferredLocaleAsync(string localeIdentifier)
        {
            // Bu basit implementasyonda sadece tercih edilen locale'ı saklarız
            // Gerçek implementasyonda bu ayarı Preferences'e kaydedebiliriz
            await Task.CompletedTask;
        }
    }
}
