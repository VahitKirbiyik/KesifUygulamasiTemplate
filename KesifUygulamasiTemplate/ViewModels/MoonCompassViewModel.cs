using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using KesifUygulamasiTemplate.Models;
using KesifUygulamasiTemplate.Resources.Strings;
using KesifUygulamasiTemplate.Services;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.ViewModels
{
    /// <summary>
    /// Ay pusulasý sayfasý için ViewModel
    /// Ay verileri (faz, doðuþ, batýþ) ve konum yönetimi
    /// </summary>
    public class MoonCompassViewModel : BaseViewModel
    {
        #region Private Fields
        private readonly IMoonCompassService _moonCompassService;
        private readonly LocationService _locationService;
        private readonly ConnectivityService _connectivityService;
        private readonly AppCenterAnalyticsService _analyticsService;
        private MoonData _moonData;
        private Location _currentLocation;
        #endregion

        #region Public Properties
        /// <summary>
        /// Ay verileri (faz, doðuþ, batýþ saatleri)
        /// </summary>
        public MoonData MoonData
        {
            get => _moonData;
            set => SetProperty(ref _moonData, value);
        }
        
        /// <summary>
        /// Mevcut konum bilgisi
        /// </summary>
        public Location CurrentLocation
        {
            get => _currentLocation;
            set => SetProperty(ref _currentLocation, value);
        }

        /// <summary>
        /// Formatlanmýþ ay doðuþ saati (kullanýcý dilinde)
        /// </summary>
        public string FormattedRiseTime => MoonData?.RiseTime != null 
            ? LocalizationService.FormatTime(MoonData.RiseTime) 
            : string.Empty;

        /// <summary>
        /// Formatlanmýþ ay batýþ saati (kullanýcý dilinde)
        /// </summary>
        public string FormattedSetTime => MoonData?.SetTime != null 
            ? LocalizationService.FormatTime(MoonData.SetTime) 
            : string.Empty;

        /// <summary>
        /// Formatlanmýþ ay fazý yüzdesi
        /// </summary>
        public string FormattedPhase => MoonData != null 
            ? LocalizationService.FormatNumber(MoonData.Phase * 100) + "%" 
            : string.Empty;

        /// <summary>
        /// Formatlanmýþ aydýnlanma yüzdesi
        /// </summary>
        public string FormattedIllumination => MoonData != null 
            ? LocalizationService.FormatNumber(MoonData.Illumination * 100) + "%" 
            : string.Empty;

        /// <summary>
        /// Ay fazýnýn adý
        /// </summary>
        public string MoonPhaseName => MoonData?.PhaseName ?? string.Empty;

        /// <summary>
        /// Ay fazýnýn emoji temsili
        /// </summary>
        public string MoonPhaseEmoji => MoonData?.PhaseEmoji ?? "??";

        /// <summary>
        /// Formatlanmýþ ay mesafesi
        /// </summary>
        public string FormattedDistance => MoonData != null 
            ? $"{LocalizationService.FormatNumber(MoonData.Distance)} km" 
            : string.Empty;

        /// <summary>
        /// Formatlanmýþ azimuth açýsý
        /// </summary>
        public string FormattedAzimuth => MoonData != null 
            ? $"{LocalizationService.FormatNumber(MoonData.Azimuth)}°" 
            : string.Empty;

        /// <summary>
        /// Formatlanmýþ altitude açýsý
        /// </summary>
        public string FormattedAltitude => MoonData != null 
            ? $"{LocalizationService.FormatNumber(MoonData.Altitude)}°" 
            : string.Empty;

        /// <summary>
        /// Konum bilgisi var mý?
        /// </summary>
        public bool HasLocationData => CurrentLocation != null;

        /// <summary>
        /// Ay verisi var mý?
        /// </summary>
        public bool HasMoonData => MoonData != null;
        #endregion

        #region Commands
        public ICommand RefreshCommand { get; }
        public ICommand GetLocationCommand { get; }
        public ICommand ShareCommand { get; }
        #endregion

        #region Constructor
        public MoonCompassViewModel(IMoonCompassService moonCompassService, LocationService locationService, ConnectivityService connectivityService, AppCenterAnalyticsService analyticsService)
        {
            _moonCompassService = moonCompassService;
            _locationService = locationService;
            _connectivityService = connectivityService;
            _analyticsService = analyticsService;
            
            // Baþlangýç deðerleri
            Title = AppResources.MoonCompass;
            MoonData = new MoonData();
            
            // Commands
            RefreshCommand = new Command(async () => await RefreshAsync(), () => IsNotBusy);
            GetLocationCommand = new Command(async () => await GetCurrentLocationAsync(), () => IsNotBusy);
            ShareCommand = new Command(async () => await ShareMoonDataAsync(), () => HasMoonData && IsNotBusy);
            
            // Dil deðiþikliði dinleyicisi
            LocalizationService.Instance.PropertyChanged += OnLocalizationChanged;
            _connectivityService.ConnectivityChanged += OnConnectivityChanged;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Belirli koordinatlar için ay verilerini yükler
        /// </summary>
        public async Task LoadMoonDataAsync(double latitude, double longitude)
        {
            _analyticsService.TrackEvent("MoonDataLoadStarted", new System.Collections.Generic.Dictionary<string, string>
            {
                { "Latitude", latitude.ToString() },
                { "Longitude", longitude.ToString() }
            });
            await ExecuteAsync(async () =>
            {
                CurrentLocation = new Location(latitude, longitude);
                MoonData = await _moonCompassService.GetMoonDataAsync(latitude, longitude);
                
                UpdateFormattedProperties();
                UpdateCommandStates();
                
                await ShowSuccessAsync(AppResources.MoonInformation);
                _analyticsService.TrackEvent("MoonDataLoadSucceeded");
            }, AppResources.Loading);
        }

        /// <summary>
        /// ViewModel initialize edildiðinde otomatik çaðrýlýr
        /// </summary>
        public override async Task InitializeAsync()
        {
            await RefreshAsync();
        }

        /// <summary>
        /// Kaynaklarý temizler
        /// </summary>
        public override void Cleanup()
        {
            LocalizationService.Instance.PropertyChanged -= OnLocalizationChanged;
            _connectivityService.ConnectivityChanged -= OnConnectivityChanged;
            base.Cleanup();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Veri yenileme iþlemi
        /// </summary>
        private async Task RefreshAsync()
        {
            _analyticsService.TrackEvent("MoonDataRefreshRequested");
            await ExecuteAsync(async () =>
            {
                // 1. Mevcut konum varsa onu kullan, yoksa yeni konum al
                var location = CurrentLocation ?? await GetLocationSafelyAsync();
                
                if (location == null)
                {
                    // Varsayýlan konum kullan (Ýstanbul)
                    location = new Location(41.0082, 28.9784);
                    await ShowWarningAsync(AppResources.DefaultLocation);
                }
                
                CurrentLocation = location;
                
                // 2. Ay verilerini al
                MoonData = await _moonCompassService.GetMoonDataAsync(location.Latitude, location.Longitude);
                
                UpdateFormattedProperties();
                UpdateCommandStates();
                
                _analyticsService.TrackEvent("MoonDataRefreshCompleted");
            }, "Veriler yenileniyor...");
        }

        /// <summary>
        /// Mevcut konumu güvenli þekilde alýr
        /// </summary>
        private async Task GetCurrentLocationAsync()
        {
            _analyticsService.TrackEvent("GetCurrentLocationRequested");
            await ExecuteAsync(async () =>
            {
                var location = await _locationService.GetCurrentLocationAsync();
                
                if (location != null)
                {
                    CurrentLocation = location;
                    
                    // Yeni konum ile ay verilerini güncelle
                    MoonData = await _moonCompassService.GetMoonDataAsync(location.Latitude, location.Longitude);
                    
                    UpdateFormattedProperties();
                    UpdateCommandStates();
                    
                    await ShowSuccessAsync(AppResources.LocationUpdated);
                    _analyticsService.TrackEvent("GetCurrentLocationSucceeded");
                }
                else
                {
                    await ShowErrorAsync(AppResources.LocationNotAvailable);
                    _analyticsService.TrackEvent("GetCurrentLocationFailed");
                }
            }, AppResources.Loading);
        }

        /// <summary>
        /// Hata yakalamadan konum alma
        /// </summary>
        private async Task<Location> GetLocationSafelyAsync()
        {
            try
            {
                return await _locationService.GetCurrentLocationAsync();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Ay verilerini paylaþma
        /// </summary>
        private async Task ShareMoonDataAsync()
        {
            if (!HasMoonData) return;

            _analyticsService.TrackEvent("ShareMoonDataRequested");
            var shareText = $"?? {AppResources.MoonInformation}\n" +
                           $"{AppResources.MoonRise}: {FormattedRiseTime}\n" +
                           $"{AppResources.MoonSet}: {FormattedSetTime}\n" +
                           $"{AppResources.MoonPhase}: {FormattedPhase}\n" +
                           $"?? {CurrentLocation?.Latitude:0.000}, {CurrentLocation?.Longitude:0.000}\n" +
                           $"?? {AppResources.AppName}";

            await Share.RequestAsync(new ShareTextRequest
            {
                Text = shareText,
                Title = AppResources.ShareMoonData
            });

            await ShowSuccessAsync(AppResources.ShareSuccessful);
            _analyticsService.TrackEvent("ShareMoonDataCompleted");
        }

        /// <summary>
        /// Formatlanmýþ property'leri günceller
        /// </summary>
        private void UpdateFormattedProperties()
        {
            OnPropertyChanged(nameof(FormattedRiseTime));
            OnPropertyChanged(nameof(FormattedSetTime));
            OnPropertyChanged(nameof(FormattedPhase));
            OnPropertyChanged(nameof(FormattedIllumination));
            OnPropertyChanged(nameof(MoonPhaseName));
            OnPropertyChanged(nameof(MoonPhaseEmoji));
            OnPropertyChanged(nameof(FormattedDistance));
            OnPropertyChanged(nameof(FormattedAzimuth));
            OnPropertyChanged(nameof(FormattedAltitude));
            OnPropertyChanged(nameof(HasLocationData));
            OnPropertyChanged(nameof(HasMoonData));
        }

        /// <summary>
        /// Command durumlarýný günceller
        /// </summary>
        private void UpdateCommandStates()
        {
            ((Command)RefreshCommand).ChangeCanExecute();
            ((Command)GetLocationCommand).ChangeCanExecute();
            ((Command)ShareCommand).ChangeCanExecute();
        }

        /// <summary>
        /// Dil deðiþikliði olayýný yakalar
        /// </summary>
        private void OnLocalizationChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(LocalizationService.CurrentCulture))
            {
                Title = Resources.Strings.AppResources.MoonCompass;
                UpdateFormattedProperties();
            }
        }

        private async void OnConnectivityChanged(bool isConnected)
        {
            if (isConnected)
                await ShowInfoAsync(AppResources.InternetConnected);
            else
                await ShowWarningAsync(AppResources.InternetDisconnected);
        }
        #endregion
    }
}