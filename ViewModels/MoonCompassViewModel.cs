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
    /// Ay pusulas� sayfas� i�in ViewModel
    /// Ay verileri (faz, do�u�, bat��) ve konum y�netimi
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
        /// Ay verileri (faz, do�u�, bat�� saatleri)
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
        /// Formatlanm�� ay do�u� saati (kullan�c� dilinde)
        /// </summary>
        public string FormattedRiseTime => MoonData?.RiseTime != null 
            ? LocalizationService.FormatTime(MoonData.RiseTime) 
            : string.Empty;

        /// <summary>
        /// Formatlanm�� ay bat�� saati (kullan�c� dilinde)
        /// </summary>
        public string FormattedSetTime => MoonData?.SetTime != null 
            ? LocalizationService.FormatTime(MoonData.SetTime) 
            : string.Empty;

        /// <summary>
        /// Formatlanm�� ay faz� y�zdesi
        /// </summary>
        public string FormattedPhase => MoonData != null 
            ? LocalizationService.FormatNumber(MoonData.Phase * 100) + "%" 
            : string.Empty;

        /// <summary>
        /// Formatlanm�� ayd�nlanma y�zdesi
        /// </summary>
        public string FormattedIllumination => MoonData != null 
            ? LocalizationService.FormatNumber(MoonData.Illumination * 100) + "%" 
            : string.Empty;

        /// <summary>
        /// Ay faz�n�n ad�
        /// </summary>
        public string MoonPhaseName => MoonData?.PhaseName ?? string.Empty;

        /// <summary>
        /// Ay faz�n�n emoji temsili
        /// </summary>
        public string MoonPhaseEmoji => MoonData?.PhaseEmoji ?? "??";

        /// <summary>
        /// Formatlanm�� ay mesafesi
        /// </summary>
        public string FormattedDistance => MoonData != null 
            ? $"{LocalizationService.FormatNumber(MoonData.Distance)} km" 
            : string.Empty;

        /// <summary>
        /// Formatlanm�� azimuth a��s�
        /// </summary>
        public string FormattedAzimuth => MoonData != null 
            ? $"{LocalizationService.FormatNumber(MoonData.Azimuth)}�" 
            : string.Empty;

        /// <summary>
        /// Formatlanm�� altitude a��s�
        /// </summary>
        public string FormattedAltitude => MoonData != null 
            ? $"{LocalizationService.FormatNumber(MoonData.Altitude)}�" 
            : string.Empty;

        /// <summary>
        /// Konum bilgisi var m�?
        /// </summary>
        public bool HasLocationData => CurrentLocation != null;

        /// <summary>
        /// Ay verisi var m�?
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
            
            // Ba�lang�� de�erleri
            Title = AppResources.MoonCompass;
            MoonData = new MoonData();
            
            // Commands
            RefreshCommand = new Command(async () => await RefreshAsync(), () => IsNotBusy);
            GetLocationCommand = new Command(async () => await GetCurrentLocationAsync(), () => IsNotBusy);
            ShareCommand = new Command(async () => await ShareMoonDataAsync(), () => HasMoonData && IsNotBusy);
            
            // Dil de�i�ikli�i dinleyicisi
            LocalizationService.Instance.PropertyChanged += OnLocalizationChanged;
            _connectivityService.ConnectivityChanged += OnConnectivityChanged;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Belirli koordinatlar i�in ay verilerini y�kler
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
        /// ViewModel initialize edildi�inde otomatik �a�r�l�r
        /// </summary>
        public override async Task InitializeAsync()
        {
            await RefreshAsync();
        }

        /// <summary>
        /// Kaynaklar� temizler
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
        /// Veri yenileme i�lemi
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
                    // Varsay�lan konum kullan (�stanbul)
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
        /// Mevcut konumu g�venli �ekilde al�r
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
                    
                    // Yeni konum ile ay verilerini g�ncelle
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
        /// Ay verilerini payla�ma
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
        /// Formatlanm�� property'leri g�nceller
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
        /// Command durumlar�n� g�nceller
        /// </summary>
        private void UpdateCommandStates()
        {
            ((Command)RefreshCommand).ChangeCanExecute();
            ((Command)GetLocationCommand).ChangeCanExecute();
            ((Command)ShareCommand).ChangeCanExecute();
        }

        /// <summary>
        /// Dil de�i�ikli�i olay�n� yakalar
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