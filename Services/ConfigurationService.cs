using System;
using System.IO;
using System.Text.Json;

namespace KesifUygulamasiTemplate.Services
{
    public class ConfigurationService
    {
        private readonly string _configFilePath = "appsettings.json";

        // Google Maps API
        public string GoogleMapsApiKey => GetApiKey("GOOGLE_MAPS_API_KEY");
        public string GoogleMapsDirectionsUrl => "https://maps.googleapis.com/maps/api/directions/json";
        public string GoogleMapsPlacesUrl => "https://maps.googleapis.com/maps/api/place";

        // Mapbox API
        public string MapboxApiKey => GetApiKey("MAPBOX_API_KEY");
        public string MapboxDirectionsUrl => "https://api.mapbox.com/directions/v5/mapbox";
        public string MapboxGeocodingUrl => "https://api.mapbox.com/geocoding/v5/mapbox.places";

        // OpenWeatherMap API
        public string OpenWeatherMapApiKey => GetApiKey("OPENWEATHERMAP_API_KEY");
        public string OpenWeatherMapBaseUrl => "https://api.openweathermap.org/data";

        // App Center
        public string AppCenterAndroidSecret => GetApiKey("APPCENTER_ANDROID_SECRET");
        public string AppCenterIOSSecret => GetApiKey("APPCENTER_IOS_SECRET");
        public bool AppCenterAnalyticsEnabled => true;
        public bool AppCenterCrashesEnabled => true;

        // Map Settings
        public int DefaultZoomLevel => 15;
        public int MaxZoomLevel => 20;
        public int MinZoomLevel => 3;
        public string TileServerUrl => "https://tile.openstreetmap.org";
        public int CacheSizeMB => 500;
        public int CacheExpiryDays => 30;

        // Notification Settings
        public bool NavigationNotificationsEnabled => true;
        public bool WeatherAlertsEnabled => true;
        public bool EmergencyPointsEnabled => true;
        public bool FavoritePlacesEnabled => true;
        public bool MoonCompassEnabled => true;

        // Privacy Settings
        public bool LocationTrackingEnabled => true;
        public bool AnalyticsEnabled => true;
        public bool CrashReportingEnabled => true;
        public bool DataSharingEnabled => false;

        // Donation Settings
        public string PayPalButtonId => GetApiKey("PAYPAL_BUTTON_ID");
        public string PayPalCurrency => "USD";
        public string PayPalLanguage => "en_US";

        public string StripePublishableKey => GetApiKey("STRIPE_PUBLISHABLE_KEY");
        public string StripePriceId => GetApiKey("STRIPE_PRICE_ID");
        public string StripeCurrency => "USD";

        private string GetApiKey(string keyName)
        {
            try
            {
                // Environment variable'dan oku
                var envValue = Environment.GetEnvironmentVariable(keyName);
                if (!string.IsNullOrEmpty(envValue))
                    return envValue;

                // appsettings.json'dan oku
                if (File.Exists(_configFilePath))
                {
                    var json = File.ReadAllText(_configFilePath);
                    var config = JsonSerializer.Deserialize<JsonElement>(json);

                    if (config.TryGetProperty("ApiKeys", out var apiKeys))
                    {
                        // Basitleştirilmiş anahtar eşleme
                        var simpleKey = keyName.Replace("_", "").Replace("APIKEY", "").Replace("SECRET", "").ToLower();
                        if (apiKeys.TryGetProperty(simpleKey, out var value))
                        {
                            return value.GetString() ?? string.Empty;
                        }
                    }
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool IsApiKeyConfigured(string key)
        {
            var value = GetApiKey(key);
            return !string.IsNullOrEmpty(value) && !value.Contains("YOUR_");
        }
    }
}
