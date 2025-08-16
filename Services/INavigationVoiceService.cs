using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Maui.Media;
using Microsoft.Extensions.Preferences;

namespace KesifUygulamasiTemplate.Services
{
    public interface INavigationVoiceService
    {
        Task SpeakNavigationInstructionAsync(string instruction, CancellationToken? cancellationToken = null);
        Task<IEnumerable<Locale>> GetAvailableLocalesAsync();
        Task SetPreferredLocaleAsync(string localeIdentifier);
        void CancelCurrentSpeech();
        Task<bool> IsVoiceGuidanceEnabledAsync();
        Task SetVoiceGuidanceEnabledAsync(bool enabled);
        Task SetVoiceVolumeAsync(float volume); // 0.0f - 1.0f
        Task SetVoicePitchAsync(float pitch);   // 0.0f - 2.0f
    }

    public class NavigationVoiceService : INavigationVoiceService, IDisposable
    {
        private readonly ITextToSpeech _textToSpeech;
        private readonly IPreferences _preferences;
        private CancellationTokenSource _cancelSpeech;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _isDisposed;

        private const string PREF_VOICE_ENABLED = "navigation_voice_enabled";
        private const string PREF_VOICE_LOCALE = "navigation_voice_locale";
        private const string PREF_VOICE_VOLUME = "navigation_voice_volume";
        private const string PREF_VOICE_PITCH = "navigation_voice_pitch";

        public NavigationVoiceService(ITextToSpeech textToSpeech, IPreferences preferences)
        {
            _textToSpeech = textToSpeech;
            _preferences = preferences;
            _cancelSpeech = new CancellationTokenSource();
        }

        public async Task SpeakNavigationInstructionAsync(string instruction, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrEmpty(instruction))
                return;

            // Sesli yönlendirme kapalýysa hiçbir þey yapma
            if (!await IsVoiceGuidanceEnabledAsync())
                return;

            // Konuþmanýn hâlâ devam edip etmediði kontrol ediliyor ve gerekirse iptal ediliyor
            await _semaphore.WaitAsync();
            try
            {
                // Önceki konuþmayý iptal et
                CancelCurrentSpeech();
                _cancelSpeech = new CancellationTokenSource();

                // Kullanýcý tercihlerini al
                var localeIdentifier = _preferences.Get(PREF_VOICE_LOCALE, "tr-TR");
                var volume = _preferences.Get(PREF_VOICE_VOLUME, 1.0f);
                var pitch = _preferences.Get(PREF_VOICE_PITCH, 1.0f);

                // Ses ayarlarý
                var options = new SpeechOptions
                {
                    Volume = volume,
                    Pitch = pitch
                };

                // Tercih edilen dili bul ve ayarla
                var locales = await _textToSpeech.GetLocalesAsync();
                var preferredLocale = locales.FirstOrDefault(l => 
                    l.Language.Equals(localeIdentifier, StringComparison.OrdinalIgnoreCase) ||
                    l.Name.Equals(localeIdentifier, StringComparison.OrdinalIgnoreCase) ||
                    (l.Language + "-" + l.Country).Equals(localeIdentifier, StringComparison.OrdinalIgnoreCase));

                if (preferredLocale != null)
                {
                    options.Locale = preferredLocale;
                }
                else
                {
                    // Tercih edilen dil bulunamadýysa, dilin ana versiyonunu dene
                    var languageCode = localeIdentifier.Split('-')[0];
                    preferredLocale = locales.FirstOrDefault(l => 
                        l.Language.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
                    
                    if (preferredLocale != null)
                        options.Locale = preferredLocale;
                }

                // Navigasyon talimatýný seslendir
                var effectiveToken = cancellationToken ?? _cancelSpeech.Token;
                
                try
                {
                    // Yönlendirme talimatýný düzenle (daha doðal olmasý için)
                    instruction = FormatNavigationInstruction(instruction);
                    
                    await _textToSpeech.SpeakAsync(instruction, options, effectiveToken);
                }
                catch (OperationCanceledException)
                {
                    // Konuþma iptal edildi, normal akýþ
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Sesli yönlendirme hatasý: {ex.Message}");
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<Locale>> GetAvailableLocalesAsync()
        {
            try
            {
                return await _textToSpeech.GetLocalesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kullanýlabilir dilleri alma hatasý: {ex.Message}");
                return Enumerable.Empty<Locale>();
            }
        }

        public async Task SetPreferredLocaleAsync(string localeIdentifier)
        {
            if (string.IsNullOrEmpty(localeIdentifier))
                throw new ArgumentNullException(nameof(localeIdentifier));

            // Geçerli bir dil kodu mu kontrol et
            var locales = await GetAvailableLocalesAsync();
            var isValidLocale = locales.Any(l => 
                l.Language.Equals(localeIdentifier, StringComparison.OrdinalIgnoreCase) ||
                l.Name.Equals(localeIdentifier, StringComparison.OrdinalIgnoreCase) ||
                (l.Language + "-" + l.Country).Equals(localeIdentifier, StringComparison.OrdinalIgnoreCase));

            if (!isValidLocale)
            {
                // En azýndan dil kodunun geçerli olup olmadýðýný kontrol et
                var languageCode = localeIdentifier.Split('-')[0];
                isValidLocale = locales.Any(l => 
                    l.Language.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
            }

            if (isValidLocale)
            {
                _preferences.Set(PREF_VOICE_LOCALE, localeIdentifier);
            }
            else
            {
                throw new ArgumentException($"Geçersiz dil kodu: {localeIdentifier}");
            }
        }

        public void CancelCurrentSpeech()
        {
            try
            {
                if (_cancelSpeech != null && !_cancelSpeech.IsCancellationRequested)
                {
                    _cancelSpeech.Cancel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Konuþmayý iptal etme hatasý: {ex.Message}");
            }
        }

        public Task<bool> IsVoiceGuidanceEnabledAsync()
        {
            return Task.FromResult(_preferences.Get(PREF_VOICE_ENABLED, true));
        }

        public Task SetVoiceGuidanceEnabledAsync(bool enabled)
        {
            _preferences.Set(PREF_VOICE_ENABLED, enabled);
            return Task.CompletedTask;
        }

        public Task SetVoiceVolumeAsync(float volume)
        {
            if (volume < 0.0f || volume > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(volume), "Ses seviyesi 0.0 ile 1.0 arasýnda olmalýdýr.");

            _preferences.Set(PREF_VOICE_VOLUME, volume);
            return Task.CompletedTask;
        }

        public Task SetVoicePitchAsync(float pitch)
        {
            if (pitch < 0.0f || pitch > 2.0f)
                throw new ArgumentOutOfRangeException(nameof(pitch), "Ses tonu 0.0 ile 2.0 arasýnda olmalýdýr.");

            _preferences.Set(PREF_VOICE_PITCH, pitch);
            return Task.CompletedTask;
        }

        // Yardýmcý metodlar
        private string FormatNavigationInstruction(string instruction)
        {
            // Daha doðal sesli yönlendirme için talimatlarý düzenle
            
            // HTML etiketlerini temizle
            instruction = instruction
                .Replace("<b>", "")
                .Replace("</b>", "")
                .Replace("<div>", ". ")
                .Replace("</div>", "");
            
            // Mesafe bilgisini düzenle
            instruction = System.Text.RegularExpressions.Regex.Replace(
                instruction, 
                @"(\d+(?:\.\d+)?) km", 
                m => $"{m.Groups[1].Value} kilometre");
            
            instruction = System.Text.RegularExpressions.Regex.Replace(
                instruction, 
                @"(\d+(?:\.\d+)?) m", 
                m => $"{m.Groups[1].Value} metre");
            
            // Noktalama iþaretlerini düzenle
            instruction = instruction.Replace("/", " üzerinden ");
            
            return instruction;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                CancelCurrentSpeech();
                _cancelSpeech?.Dispose();
                _semaphore?.Dispose();
            }

            _isDisposed = true;
        }
    }
}