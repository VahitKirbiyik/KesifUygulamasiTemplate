using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Services.Interfaces
{
    public interface IRoutingService
    {
        Task<DirectionsRoute?> GetRouteAsync(LatLng from, LatLng to);
        Task<Route> CalculateRouteAsync(Location start, Location end, TransportMode mode = TransportMode.Driving);
        Task<List<Route>> GetAlternativeRoutesAsync(Location start, Location end, TransportMode mode = TransportMode.Driving);
        Task<TimeSpan> EstimateTimeAsync(Route route, bool considerTraffic = true);
    }

    public interface IOfflineRouteService
    {
        Task<bool> HasOfflineRouteAsync(string id);
        Task<string> SaveRouteAsync(Route route);
        Task<Route> LoadRouteAsync(string routeId);
        Task<List<Route>> GetAllSavedRoutesAsync();
    }

    public interface ILocationSharingService
    {
        Task ShareLocationAsync(LatLng location, string? message = null);
    }

    public interface IBarcodeGeneratorService
    {
        byte[] GenerateBarcode(string data);
    }

    public interface INavigationVoiceService
    {
        Task SpeakAsync(string text);
        Task<string[]> GetAvailableLocalesAsync();
        Task SetPreferredLocaleAsync(string localeIdentifier);
    }

    public interface ILocationPrivacyService
    {
        Task<bool> RequestLocationPermissionWithPrivacyInfoAsync();
    }

    public interface IFavoritePlacesService
    {
        Task AddFavoriteAsync(LocationModel place);
        Task RemoveFavoriteAsync(string id);
        Task<IEnumerable<LocationModel>> GetFavoritesAsync();
        Task<IEnumerable<LocationModel>> GetAllFavoritePlacesAsync();
    }

    public interface ISearchHistoryService
    {
        Task AddSearchAsync(string query);
        Task<IEnumerable<string>> GetRecentSearchesAsync(int limit = 10);
    }

    public interface ISecureDataService
    {
        void Set(string key, string value);
        string? Get(string key);
    }

    public interface ICompassService
    {
        event Action<double> HeadingChanged;
        bool IsMonitoring { get; }
        bool IsSupported { get; }
        void Start();
        void Stop();
    }

    public interface IEmergencyPointsService
    {
        Task<IEnumerable<EmergencyPoint>> GetAllEmergencyPointsAsync(int limit = 100);
        Task<EmergencyPoint?> GetEmergencyPointByIdAsync(int id);
        Task<IEnumerable<EmergencyPoint>> GetNearbyEmergencyPointsAsync(double latitude, double longitude, double radiusKm = 5, EmergencyPointType type = EmergencyPointType.All);
        Task<int> AddEmergencyPointAsync(EmergencyPoint point);
        Task<bool> UpdateEmergencyPointAsync(EmergencyPoint point);
        Task<bool> RemoveEmergencyPointAsync(int id);
        Task<bool> ClearAllEmergencyPointsAsync();
    }

    public interface IStreetViewService
    {
        Task<StreetViewPanorama> GetPanoramaAsync(string location);
    }

    public interface IPreferencesService
    {
        void Set<T>(string key, T value);
        T Get<T>(string key, T defaultValue);
        bool ContainsKey(string key);
        void Remove(string key);
        void Clear();
    }
}
