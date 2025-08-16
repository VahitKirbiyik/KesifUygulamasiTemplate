using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace KesifUygulamasiTemplate.Services
{
    public enum AppTheme
    {
        System,
        Light,
        Dark
    }
    
    public enum MapType
    {
        Standard,
        Satellite,
        Hybrid,
        Terrain
    }
    
    public class MapPreferences
    {
        public MapType DefaultMapType { get; set; } = MapType.Standard;
        public bool ShowTraffic { get; set; } = true;
        public bool Show3DBuildings { get; set; } = true;
        public bool RotateMapWithMovement { get; set; } = true;
        public bool ShowPointsOfInterest { get; set; } = true;
        public int DefaultZoomLevel { get; set; } = 15;
        public bool FollowUserLocation { get; set; } = true;
        public List<string> VisiblePOICategories { get; set; } = new List<string> 
        { 
            "restaurant", "cafe", "hospital", "pharmacy", "gas_station", "hotel" 
        };
    }
    
    public class NavigationPreferences
    {
        public bool AvoidTolls { get; set; } = false;
        public bool AvoidHighways { get; set; } = false;
        public bool AvoidFerries { get; set; } = false;
        public bool EnableVoiceGuidance { get; set; } = true;
        public string VoiceLanguage { get; set; } = "tr-TR";
        public float VoiceVolume { get; set; } = 1.0f;
        public bool ShowSpeedLimits { get; set; } = true;
        public bool AlertForSpeedCameras { get; set; } = true;
        public bool UseImperialUnits { get; set; } = false;
    }
    
    public interface IPreferencesService
    {
        // Tema tercihleri
        Task<AppTheme> GetThemePreferenceAsync();
        Task SetThemePreferenceAsync(AppTheme theme);
        
        // Dil tercihleri
        Task<string> GetLanguagePreferenceAsync();
        Task SetLanguagePreferenceAsync(string languageCode);
        Task<List<string>> GetAvailableLanguagesAsync();
        
        // Harita tercihleri
        Task<MapPreferences> GetMapPreferencesAsync();
        Task SetMapPreferencesAsync(MapPreferences preferences);
        
        // Navigasyon tercihleri
        Task<NavigationPreferences> GetNavigationPreferencesAsync();
        Task SetNavigationPreferencesAsync(NavigationPreferences preferences);
        
        // Enerji tasarrufu modu
        Task<bool> GetEnergySavingModeAsync();
        Task SetEnergySavingModeAsync(bool enabled);
        
        // Tüm tercihleri sýfýrlama
        Task ResetAllPreferencesAsync();
        
        // Tercih deðiþikliklerini dinleme
        event EventHandler<string> PreferenceChanged;
    }
    
    public class PreferencesService : IPreferencesService
    {
        private readonly IPreferences _preferences;
        
        // Anahtar sabitleri
        private const string KEY_THEME = "app_theme";
        private const string KEY_LANGUAGE = "app_language";
        private const string KEY_MAP_PREFERENCES = "map_preferences";
        private const string KEY_NAVIGATION_PREFERENCES = "navigation_preferences";
        private const string KEY_ENERGY_SAVING_MODE = "energy_saving_mode";
        
        public event EventHandler<string> PreferenceChanged;
        
        public PreferencesService(IPreferences preferences)
        {
            _preferences = preferences;
        }
        
        #region Tema Tercihleri
        
        public Task<AppTheme> GetThemePreferenceAsync()
        {
            // Varsayýlan olarak sistem temasýný kullan
            var themeValue = _preferences.Get(KEY_THEME, (int)AppTheme.System);
            return Task.FromResult((AppTheme)themeValue);
        }
        
        public Task SetThemePreferenceAsync(AppTheme theme)
        {
            _preferences.Set(KEY_THEME, (int)theme);
            OnPreferenceChanged(KEY_THEME);
            return Task.CompletedTask;
        }
        
        #endregion
        
        #region Dil Tercihleri
        
        public Task<string> GetLanguagePreferenceAsync()
        {
            // Varsayýlan olarak sistem dilini kullan
            var defaultLanguage = CultureInfo.CurrentCulture.Name;
            return Task.FromResult(_preferences.Get(KEY_LANGUAGE, defaultLanguage));
        }
        
        public Task SetLanguagePreferenceAsync(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
                throw new ArgumentNullException(nameof(languageCode));
                
            _preferences.Set(KEY_LANGUAGE, languageCode);
            OnPreferenceChanged(KEY_LANGUAGE);
            return Task.CompletedTask;
        }
        
        public Task<List<string>> GetAvailableLanguagesAsync()
        {
            // Uygulamada desteklenen dillerin listesi
            var languages = new List<string>
            {
                "tr-TR", // Türkçe
                "en-US", // Ýngilizce
                "de-DE", // Almanca
                "fr-FR", // Fransýzca
                "es-ES", // Ýspanyolca
                "it-IT", // Ýtalyanca
                "ru-RU", // Rusça
                "ar-SA", // Arapça
                "zh-CN", // Çince (Basitleþtirilmiþ)
                "ja-JP"  // Japonca
            };
            
            return Task.FromResult(languages);
        }
        
        #endregion
        
        #region Harita Tercihleri
        
        public Task<MapPreferences> GetMapPreferencesAsync()
        {
            var json = _preferences.Get(KEY_MAP_PREFERENCES, "");
            
            if (string.IsNullOrEmpty(json))
                return Task.FromResult(new MapPreferences());
                
            try
            {
                var preferences = JsonSerializer.Deserialize<MapPreferences>(json);
                return Task.FromResult(preferences ?? new MapPreferences());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Harita tercihleri okuma hatasý: {ex.Message}");
                return Task.FromResult(new MapPreferences());
            }
        }
        
        public Task SetMapPreferencesAsync(MapPreferences preferences)
        {
            if (preferences == null)
                throw new ArgumentNullException(nameof(preferences));
                
            try
            {
                var json = JsonSerializer.Serialize(preferences);
                _preferences.Set(KEY_MAP_PREFERENCES, json);
                OnPreferenceChanged(KEY_MAP_PREFERENCES);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Harita tercihleri kaydetme hatasý: {ex.Message}");
            }
            
            return Task.CompletedTask;
        }
        
        #endregion
        
        #region Navigasyon Tercihleri
        
        public Task<NavigationPreferences> GetNavigationPreferencesAsync()
        {
            var json = _preferences.Get(KEY_NAVIGATION_PREFERENCES, "");
            
            if (string.IsNullOrEmpty(json))
                return Task.FromResult(new NavigationPreferences());
                
            try
            {
                var preferences = JsonSerializer.Deserialize<NavigationPreferences>(json);
                return Task.FromResult(preferences ?? new NavigationPreferences());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigasyon tercihleri okuma hatasý: {ex.Message}");
                return Task.FromResult(new NavigationPreferences());
            }
        }
        
        public Task SetNavigationPreferencesAsync(NavigationPreferences preferences)
        {
            if (preferences == null)
                throw new ArgumentNullException(nameof(preferences));
                
            try
            {
                var json = JsonSerializer.Serialize(preferences);
                _preferences.Set(KEY_NAVIGATION_PREFERENCES, json);
                OnPreferenceChanged(KEY_NAVIGATION_PREFERENCES);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigasyon tercihleri kaydetme hatasý: {ex.Message}");
            }
            
            return Task.CompletedTask;
        }
        
        #endregion
        
        #region Enerji Tasarrufu Modu
        
        public Task<bool> GetEnergySavingModeAsync()
        {
            return Task.FromResult(_preferences.Get(KEY_ENERGY_SAVING_MODE, false));
        }
        
        public Task SetEnergySavingModeAsync(bool enabled)
        {
            _preferences.Set(KEY_ENERGY_SAVING_MODE, enabled);
            OnPreferenceChanged(KEY_ENERGY_SAVING_MODE);
            return Task.CompletedTask;
        }
        
        #endregion
        
        #region Tüm Tercihleri Sýfýrlama
        
        public Task ResetAllPreferencesAsync()
        {
            // Tüm tercih anahtarlarýný temizle
            _preferences.Remove(KEY_THEME);
            _preferences.Remove(KEY_LANGUAGE);
            _preferences.Remove(KEY_MAP_PREFERENCES);
            _preferences.Remove(KEY_NAVIGATION_PREFERENCES);
            _preferences.Remove(KEY_ENERGY_SAVING_MODE);
            
            // Tüm tercihlerin sýfýrlandýðýný bildir
            OnPreferenceChanged("all");
            
            return Task.CompletedTask;
        }
        
        #endregion
        
        #region Yardýmcý Metotlar
        
        private void OnPreferenceChanged(string key)
        {
            PreferenceChanged?.Invoke(this, key);
        }
        
        #endregion
    }
}