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

    public class PreferencesService : KesifUygulamasiTemplate.Services.Interfaces.IPreferencesService
    {
        private readonly IPreferences _preferences;

        // Anahtar sabitleri
        private const string KEY_THEME = "app_theme";
        private const string KEY_LANGUAGE = "app_language";
        private const string KEY_MAP_PREFERENCES = "map_preferences";
        private const string KEY_NAVIGATION_PREFERENCES = "navigation_preferences";
        private const string KEY_ENERGY_SAVING_MODE = "energy_saving_mode";

        public event EventHandler<string>? PreferenceChanged;

        public PreferencesService(IPreferences preferences)
        {
            _preferences = preferences;
        }

        #region Tema Tercihleri

        public Task<AppTheme> GetThemePreferenceAsync()
        {
            // Varsay�lan olarak sistem temas�n� kullan
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
            // Varsay�lan olarak sistem dilini kullan
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
                "tr-TR", // T�rk�e
                "en-US", // �ngilizce
                "de-DE", // Almanca
                "fr-FR", // Frans�zca
                "es-ES", // �spanyolca
                "it-IT", // �talyanca
                "ru-RU", // Rus�a
                "ar-SA", // Arap�a
                "zh-CN", // �ince (Basitle�tirilmi�)
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
                Console.WriteLine($"Harita tercihleri okuma hatas�: {ex.Message}");
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
                Console.WriteLine($"Harita tercihleri kaydetme hatas�: {ex.Message}");
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
                Console.WriteLine($"Navigasyon tercihleri okuma hatas�: {ex.Message}");
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
                Console.WriteLine($"Navigasyon tercihleri kaydetme hatas�: {ex.Message}");
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

        #region T�m Tercihleri S�f�rlama

        public Task ResetAllPreferencesAsync()
        {
            // T�m tercih anahtarlar�n� temizle
            _preferences.Remove(KEY_THEME);
            _preferences.Remove(KEY_LANGUAGE);
            _preferences.Remove(KEY_MAP_PREFERENCES);
            _preferences.Remove(KEY_NAVIGATION_PREFERENCES);
            _preferences.Remove(KEY_ENERGY_SAVING_MODE);

            // T�m tercihlerin s�f�rland���n� bildir
            OnPreferenceChanged("all");

            return Task.CompletedTask;
        }

        #endregion

        #region Yard�mc� Metotlar

        private void OnPreferenceChanged(string key)
        {
            PreferenceChanged?.Invoke(this, key);
        }

        #endregion

        #region Sync Methods

        public void Set(string key, string value)
        {
            _preferences.Set(key, value);
            OnPreferenceChanged(key);
        }

        public string? Get(string key)
        {
            return _preferences.Get<string?>(key, default);
        }

        public void Set(string key, bool value)
        {
            _preferences.Set(key, value);
            OnPreferenceChanged(key);
        }

        public bool Get(string key, bool defaultValue)
        {
            return _preferences.Get(key, defaultValue);
        }

        #endregion

        #region Generic IPreferencesService Implementation

        public void Set<T>(string key, T value)
        {
            if (value == null)
            {
                _preferences.Remove(key);
            }
            else if (value is string str)
            {
                _preferences.Set(key, str);
            }
            else if (value is bool b)
            {
                _preferences.Set(key, b);
            }
            else if (value is int i)
            {
                _preferences.Set(key, i);
            }
            else if (value is double d)
            {
                _preferences.Set(key, d);
            }
            else if (value is float f)
            {
                _preferences.Set(key, f.ToString());
            }
            else
            {
                _preferences.Set(key, value.ToString() ?? "");
            }
            OnPreferenceChanged(key);
        }

        public T Get<T>(string key, T defaultValue)
        {
            try
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)_preferences.Get(key, (string)(object)defaultValue!);
                }
                else if (typeof(T) == typeof(bool))
                {
                    return (T)(object)_preferences.Get(key, (bool)(object)defaultValue!);
                }
                else if (typeof(T) == typeof(int))
                {
                    return (T)(object)_preferences.Get(key, (int)(object)defaultValue!);
                }
                else if (typeof(T) == typeof(double))
                {
                    return (T)(object)_preferences.Get(key, (double)(object)defaultValue!);
                }
                else if (typeof(T) == typeof(float))
                {
                    var str = _preferences.Get(key, "");
                    if (string.IsNullOrEmpty(str))
                        return defaultValue;
                    return (T)(object)float.Parse(str);
                }
                else
                {
                    var str = _preferences.Get(key, "");
                    if (string.IsNullOrEmpty(str))
                        return defaultValue;
                    return (T)(object)str;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        public bool ContainsKey(string key)
        {
            return _preferences.ContainsKey(key);
        }

        public void Remove(string key)
        {
            _preferences.Remove(key);
            OnPreferenceChanged(key);
        }

        public void Clear()
        {
            _preferences.Clear();
            OnPreferenceChanged("all");
        }

        #endregion
    }
}
