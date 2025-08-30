using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.Services
{
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
                    "Konum �zni",
                    "Konumunuz yaln�zca uygulama i�inde kullan�l�r ve ���nc� taraflarla payla��lmaz. Devam etmek istiyor musunuz?",
                    "�zin Ver", "Reddet");

                if (!userConsent)
                    return false;

                _preferencesService.Set("HasShownLocationPrivacyInfo", true);
            }

            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            return status == PermissionStatus.Granted;
        }

        public async Task ForgetAllLocationDataAsync()
        {
            // Kullan�c�n�n t�m konum verilerini sil
            await Task.CompletedTask;
        }
    }
}
