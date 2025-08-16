// Services/NavigationVoiceService.cs
public class NavigationVoiceService : INavigationVoiceService
{
    private readonly ITextToSpeech _textToSpeech;
    private readonly IPreferences _preferences;
    private CancellationTokenSource _cancelSpeech;
    
    public NavigationVoiceService(ITextToSpeech textToSpeech, IPreferences preferences)
    {
        _textToSpeech = textToSpeech;
        _preferences = preferences;
        _cancelSpeech = new CancellationTokenSource();
    }
    
    public async Task SpeakNavigationInstructionAsync(string instruction)
    {
        if (!_preferences.Get("voice_guidance_enabled", true))
            return;
            
        // �nceki konu�may� iptal et
        _cancelSpeech.Cancel();
        _cancelSpeech = new CancellationTokenSource();
        
        // Kullan�c� dil tercihini al
        var locale = _preferences.Get("voice_language", "tr-TR");
        
        var options = new SpeechOptions
        {
            Volume = _preferences.Get("voice_volume", 1.0f),
            Pitch = 1.0f
        };
        
        // E�er tercih edilen dil kullan�labilir ise, onu kullan
        var locales = await _textToSpeech.GetLocalesAsync();
        var preferredLocale = locales.FirstOrDefault(l => l.Language.StartsWith(locale.Split('-')[0]));
        
        if (preferredLocale != null)
            options.Locale = preferredLocale;
        
        try
        {
            await _textToSpeech.SpeakAsync(instruction, options, _cancelSpeech.Token);
        }
        catch (OperationCanceledException)
        {
            // Konu�ma iptal edildi, normal ak��
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sesli y�nlendirme hatas�: {ex.Message}");
        }
    }
    
    public async Task<IEnumerable<Locale>> GetAvailableLocalesAsync()
    {
        return await _textToSpeech.GetLocalesAsync();
    }
    
    public void CancelCurrentSpeech()
    {
        _cancelSpeech.Cancel();
    }
}