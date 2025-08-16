using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using KesifUygulamasiTemplate.Resources.Strings;
using KesifUygulamasiTemplate.Services;
using KesifUygulamasiTemplate.Models;
using System.Globalization;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly SettingsService _settingsService;
        private readonly ThemeService _themeService;
        private readonly PushNotificationService _pushService;
        private bool _isOfflineMode;
        private string _appVersion;
        private Models.AppTheme _selectedTheme;
        private bool _isPushEnabled;

        public bool IsOfflineMode
        {
            get => _isOfflineMode;
            set => SetProperty(ref _isOfflineMode, value);
        }

        public string AppVersion
        {
            get => _appVersion;
            set => SetProperty(ref _appVersion, value);
        }

        public Models.AppTheme SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (SetProperty(ref _selectedTheme, value))
                {
                    _themeService.SetTheme(value);
                }
            }
        }

        public bool IsPushEnabled
        {
            get => _isPushEnabled;
            set
            {
                if (SetProperty(ref _isPushEnabled, value))
                {
                    _pushService.SetPushEnabled(value);
                }
            }
        }

        public bool IsTurkishSelected => LocalizationService.CurrentCulture.TwoLetterISOLanguageName == "tr";
        public bool IsEnglishSelected => LocalizationService.CurrentCulture.TwoLetterISOLanguageName == "en";
        public IEnumerable<Models.AppTheme> ThemeOptions => Enum.GetValues(typeof(Models.AppTheme)).Cast<Models.AppTheme>();

        public SettingsViewModel(SettingsService settingsService, ThemeService themeService, PushNotificationService pushService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
            _pushService = pushService ?? throw new ArgumentNullException(nameof(pushService));

            LoadSettings();
            LoadAppVersion();
            _selectedTheme = _themeService.CurrentTheme;
            _isPushEnabled = _pushService.IsPushEnabled;
        }

        private void LoadSettings()
        {
            IsOfflineMode = _settingsService.GetSetting<bool>("OfflineMode", false);
        }
        
        private void LoadAppVersion()
        {
            AppVersion = AppInfo.VersionString;
        }
        
        public void SetOfflineMode(bool value)
        {
            IsOfflineMode = value;
            _settingsService.SaveSetting("OfflineMode", value);
        }
        
        public async Task<string> CheckLocationPermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                return status == PermissionStatus.Granted 
                    ? AppResources.PermissionGranted 
                    : AppResources.PermissionDenied;
            }
            catch
            {
                return AppResources.Error;
            }
        }
        
        public async Task RequestLocationPermissionAsync()
        {
            try
            {
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}