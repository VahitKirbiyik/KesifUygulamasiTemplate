public class LocationPrivacyService : ILocationPrivacyService
{
    private readonly IPreferencesService _preferencesService;
    
    public LocationPrivacyService(IPreferencesService preferencesService)
    {
        _preferencesService = preferencesService;
    }
    
    public async Task<bool> RequestLocationPermissionWithPrivacyInfoAsync()
    {
        var hasShownInfo = _preferencesService.Get("HasShownLocationPrivacyInfo", false);
        
        if (!hasShownInfo)
        {
            bool userConsent = await Shell.Current.DisplayAlert(
                "Konum Ýzni", 
                "Konumunuz yalnýzca uygulama içinde kullanýlýr ve üçüncü taraflarla paylaþýlmaz. Devam etmek istiyor musunuz?", 
                "Ýzin Ver", "Reddet");
                
            if (!userConsent)
                return false;
                
            _preferencesService.Set("HasShownLocationPrivacyInfo", true);
        }
        
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        return status == PermissionStatus.Granted;
    }
    
    public async Task ForgetAllLocationDataAsync()
    {
        // Kullanýcýnýn tüm konum verilerini sil
    }
}
