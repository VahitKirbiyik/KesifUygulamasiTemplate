public interface IOfflineDataService
{
    Task<bool> StoreDataForOfflineUseAsync<T>(string key, T data);
    Task<T?> GetOfflineDataAsync<T>(string key);
    Task<bool> IsOfflineDataAvailableAsync(string key);
    Task SyncDataWithServerAsync();
}
