public interface IMapDataService
{
    Task<bool> SyncMapDataAsync(double latitude, double longitude, int radiusKm, int maxZoom = 15);
    Task<IEnumerable<MapTile>> GetOfflineTilesAsync(double north, double south, double east, double west, int zoomLevel);
    Task<bool> HasOfflineCoverageAsync(double latitude, double longitude, int zoomLevel);
    Task<int> GetOfflineMapSizeMBAsync();
    Task<bool> ClearExpiredTilesAsync();
    Task<bool> ClearAllTilesAsync();
}
