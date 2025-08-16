// Services/EmergencyPointsService.cs
public class EmergencyPointsService : IEmergencyPointsService
{
    private readonly HttpClient _httpClient;
    private readonly SQLiteConnection _database;
    private readonly IConnectivity _connectivity;
    
    public EmergencyPointsService(HttpClient httpClient, SQLiteConnection database, IConnectivity connectivity)
    {
        _httpClient = httpClient;
        _database = database;
        _connectivity = connectivity;
        _database.CreateTable<EmergencyPoint>();
    }
    
    public async Task<IEnumerable<EmergencyPoint>> GetNearbyEmergencyPointsAsync(
        double latitude, 
        double longitude, 
        double radiusKm = 5, 
        EmergencyType type = EmergencyType.All)
    {
        // Önce yerel veritabanýndan bul
        var localPoints = _database.Table<EmergencyPoint>()
            .Where(p => 
                (type == EmergencyType.All || p.Type == type) &&
                CalculateDistance(latitude, longitude, p.Latitude, p.Longitude) <= radiusKm)
            .ToList();
            
        // Eðer internet varsa ve yerel veri yetersizse çevrimiçi arama yap
        if (_connectivity.NetworkAccess == NetworkAccess.Internet && 
            (localPoints.Count < 10 || (DateTime.UtcNow - localPoints.FirstOrDefault()?.LastUpdated ?? DateTime.MinValue).TotalDays > 7))
        {
            try
            {
                var url = $"https://api.mapservice.com/pois?lat={latitude}&lng={longitude}&radius={radiusKm}&type={type.ToString().ToLowerInvariant()}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var onlinePoints = await response.Content.ReadFromJsonAsync<List<EmergencyPoint>>();
                    
                    // Veritabanýný güncelle
                    foreach (var point in onlinePoints)
                    {
                        point.LastUpdated = DateTime.UtcNow;
                        var existing = _database.Table<EmergencyPoint>()
                            .FirstOrDefault(p => p.ExternalId == point.ExternalId);
                            
                        if (existing != null)
                            _database.Update(point);
                        else
                            _database.Insert(point);
                    }
                    
                    return onlinePoints;
                }
            }
            catch
            {
                // API hatasý - yerel verilere geri dön
            }
        }
        
        return localPoints;
    }
    
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formülü ile mesafe hesaplama
        // ...
    }
}

public enum EmergencyType
  {
      All,
      Hospital,
      Police,
      Pharmacy,
      GasStation,
      FireStation
  }