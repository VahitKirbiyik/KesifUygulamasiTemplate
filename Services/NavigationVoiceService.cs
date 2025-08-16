using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Media;

namespace KesifUygamamasiTemplate.Services
{
    public class NavigationVoiceService : INavigationVoiceService
    {
        public Task<string[]> GetAvailableLocalesAsync()
        {
            // Gerçek TTS engine sorgusu ileride eklenecek
            return Task.FromResult(new[] { "tr-TR", "en-US" });
        }

        public async Task SpeakAsync(string text, string locale = "tr-TR")
        {
            var options = new SpeechOptions
            {
                Locale = new Locale(locale),
                Volume = 1.0f,
                Pitch = 1.0f
            };

            await TextToSpeech.Default.SpeakAsync(text, options);
        }
    }
}
