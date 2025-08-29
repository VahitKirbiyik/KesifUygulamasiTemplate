using System;
using System.Threading.Tasks;
using System.Windows.Input;
using KesifUygulamasiTemplate.Services.Interfaces;
using KesifUygulamasiTemplate.ViewModels.Base;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IPreferencesService _preferencesService;
        private readonly ILocationPrivacyService _locationPrivacyService;

        private bool _isDarkMode;
        private string _language = "en";
        private bool _isOfflineMode;
        private AppTheme _selectedTheme;
        private bool _isPushEnabled;

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set => SetProperty(ref _isDarkMode, value);
        }

        public string Language
        {
            get => _language;
            set => SetProperty(ref _language, value);
        }

        public bool IsOfflineMode
        {
            get => _isOfflineMode;
            set => SetProperty(ref _isOfflineMode, value);
        }

        public AppTheme SelectedTheme
        {
            get => _selectedTheme;
            set => SetProperty(ref _selectedTheme, value);
        }

        public bool IsPushEnabled
        {
            get => _isPushEnabled;
            set => SetProperty(ref _isPushEnabled, value);
        }

        public AppTheme[] ThemeOptions => new[] { AppTheme.Light, AppTheme.Dark };

        public bool IsTurkishSelected => Language == "tr";
        public bool IsEnglishSelected => Language == "en";

        public ICommand SetTurkishCommand { get; }
        public ICommand SetEnglishCommand { get; }
        public ICommand SetOfflineModeCommand { get; }
        public ICommand RequestLocationPermissionCommand { get; }
        public ICommand CheckLocationPermissionCommand { get; }
        public ICommand ToggleOfflineModeCommand { get; }
        public ICommand PayPalDonationCommand { get; }
        public ICommand StripeDonationCommand { get; }

        public SettingsViewModel(IPreferencesService preferencesService, ILocationPrivacyService locationPrivacyService)
        {
            _preferencesService = preferencesService ?? throw new ArgumentNullException(nameof(preferencesService));
            _locationPrivacyService = locationPrivacyService ?? throw new ArgumentNullException(nameof(locationPrivacyService));

            SetTurkishCommand = new Command(() => SetLanguage("tr"));
            SetEnglishCommand = new Command(() => SetLanguage("en"));
            SetOfflineModeCommand = new Command<bool>(SetOfflineMode);
            RequestLocationPermissionCommand = new Command(async () => await RequestLocationPermissionAsync());
            CheckLocationPermissionCommand = new Command(async () =>
            {
                var status = await CheckLocationPermissionAsync();
                // Handle the status if needed
            });
            ToggleOfflineModeCommand = new Command(ToggleOfflineMode);
            PayPalDonationCommand = new Command(OpenPayPalDonation);
            StripeDonationCommand = new Command(OpenStripeDonation);

            SetTurkishCommand = new Command(() => SetLanguage("tr"));
            SetEnglishCommand = new Command(() => SetLanguage("en"));
            SetOfflineModeCommand = new Command<bool>(SetOfflineMode);
            RequestLocationPermissionCommand = new Command(async () => await RequestLocationPermissionAsync());
            CheckLocationPermissionCommand = new Command(async () => 
            {
                var status = await CheckLocationPermissionAsync();
                // Handle the status if needed
            });
        }

        private void SetLanguage(string language)
        {
            Language = language;
            OnPropertyChanged(nameof(IsTurkishSelected));
            OnPropertyChanged(nameof(IsEnglishSelected));
        }

        public void SetOfflineMode(bool isOffline)
        {
            IsOfflineMode = isOffline;
        }

        public async Task RequestLocationPermissionAsync()
        {
            try
            {
                var result = await _locationPrivacyService.RequestLocationPermissionWithPrivacyInfoAsync();
                if (!result)
                {
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Permission Denied", "Location permission is required for this feature.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to request location permission: {ex.Message}", "OK");
                }
            }
        }

        public async Task<string> CheckLocationPermissionAsync()
        {
            try
            {
                // Implementation for checking location permission
                // For now, return a mock status
                return "Permission status checked";
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to check location permission: {ex.Message}", "OK");
                }
                return "Error checking permission";
            }
        }

        private void ToggleOfflineMode()
        {
            IsOfflineMode = !IsOfflineMode;
        }

        private void OpenPayPalDonation()
        {
            // Open PayPal donation link
            if (Application.Current?.MainPage != null)
            {
                Application.Current.MainPage.DisplayAlert("PayPal Donation", "Opening PayPal donation page...", "OK");
            }
        }

        private void OpenStripeDonation()
        {
            // Open Stripe donation link
            if (Application.Current?.MainPage != null)
            {
                Application.Current.MainPage.DisplayAlert("Stripe Donation", "Opening Stripe donation page...", "OK");
            }
        }
    }
}
