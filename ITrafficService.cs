// Services/TrafficService.cs
public interface ITrafficService
{
    Task<TrafficInfo> GetTrafficInfoAsync(double lat, double lng, double radius);
    Task<List<TrafficIncident>> GetTrafficIncidentsAsync(double north, double south, double east, double west);
    Task<Route> GetOptimizedRouteWithTrafficAsync(Location start, Location end, TransportMode mode, RouteOptimizationPreference preference);
}

public class TrafficService : ITrafficService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IConnectivity _connectivity;
    
    public TrafficService(HttpClient httpClient, IConfiguration config, IConnectivity connectivity)
    {
        _httpClient = httpClient;
        _apiKey = config["MapServices:GoogleApiKey"];
        _connectivity = connectivity;
    }
    
    public async Task<TrafficInfo> GetTrafficInfoAsync(double lat, double lng, double radius)
    {
        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            return null;
            
        try
        {
            // Gerçek uygulamada, Google Maps Distance Matrix API veya 
            // alternatif bir trafik API'si kullanabilirsiniz
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json" +
                      $"?origins={lat},{lng}" +
                      $"&destinations={lat + 0.01},{lng + 0.01}" +
                      $"&departure_time=now" +
                      $"&key={_apiKey}";
                      
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var distanceMatrix = await response.Content.ReadFromJsonAsync<GoogleDistanceMatrixResponse>();
                
                if (distanceMatrix?.Rows?.Count > 0 && 
                    distanceMatrix.Rows[0].Elements?.Count > 0 &&
                    distanceMatrix.Rows[0].Elements[0].Status == "OK")
                {
                    var element = distanceMatrix.Rows[0].Elements[0];
                    
                    return new TrafficInfo
                    {
                        CongestionLevel = CalculateCongestionLevel(
                            element.Duration.Value, 
                            element.DurationInTraffic?.Value ?? element.Duration.Value),
                        DelayTime = TimeSpan.FromSeconds(
                            (element.DurationInTraffic?.Value ?? element.Duration.Value) - element.Duration.Value),
                        TypicalTravelTime = TimeSpan.FromSeconds(element.Duration.Value),
                        CurrentTravelTime = TimeSpan.FromSeconds(element.DurationInTraffic?.Value ?? element.Duration.Value)
                    };
                }
            }
            
            return new TrafficInfo { CongestionLevel = 0, DelayTime = TimeSpan.Zero };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Trafik bilgisi alma hatasý: {ex.Message}");
            return new TrafficInfo { CongestionLevel = 0, DelayTime = TimeSpan.Zero };
        }
    }
    
    public async Task<List<TrafficIncident>> GetTrafficIncidentsAsync(double north, double south, double east, double west)
    {
        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            return new List<TrafficIncident>();
            
        try
        {
            // Bu örnek bir trafik olaylarý API'si çaðrýsýdýr
            // Gerçek uygulamada HERE, TomTom veya benzer bir API kullanabilirsiniz
            var url = $"https://traffic.api.example.com/incidents" +
                      $"?bbox={west},{south},{east},{north}" +
                      $"&key={_apiKey}";
                      
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var incidents = await response.Content.ReadFromJsonAsync<List<TrafficIncident>>();
                return incidents ?? new List<TrafficIncident>();
            }
            
            return new List<TrafficIncident>();
        }
        catch
        {
            return new List<TrafficIncident>();
        }
    }
    
    public async Task<Route> GetOptimizedRouteWithTrafficAsync(
        Location start, Location end, TransportMode mode, RouteOptimizationPreference preference)
    {
        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            return null;
            
        try
        {
            var trafficModel = preference switch
            {
                RouteOptimizationPreference.BestGuess => "best_guess",
                RouteOptimizationPreference.Optimistic => "optimistic",
                RouteOptimizationPreference.Pessimistic => "pessimistic",
                _ => "best_guess"
            };
            
            var modeStr = mode switch
            {
                TransportMode.Walking => "walking",
                TransportMode.Bicycling => "bicycling",
                TransportMode.Transit => "transit",
                _ => "driving"
            };
            
            var url = $"https://maps.googleapis.com/maps/api/directions/json" +
                      $"?origin={start.Latitude},{start.Longitude}" +
                      $"&destination={end.Latitude},{end.Longitude}" +
                      $"&mode={modeStr}" +
                      $"&departure_time=now" +
                      $"&traffic_model={trafficModel}" +
                      $"&alternatives=true" +
                      $"&key={_apiKey}";
                      
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var directionsResponse = await response.Content.ReadFromJsonAsync<GoogleDirectionsResponse>();
                
                if (directionsResponse?.Status == "OK" && directionsResponse.Routes?.Count > 0)
                {
                    return ConvertToRoute(directionsResponse.Routes[0], mode, start, end);
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Trafik optimizasyonlu rota hesaplama hatasý: {ex.Message}");
            return null;
        }
    }
    
    private int CalculateCongestionLevel(int normalDuration, int trafficDuration)
    {
        if (normalDuration == 0)
            return 0;
            
        double ratio = (double)trafficDuration / normalDuration;
        
        if (ratio <= 1.1) return 0; // Normal
        if (ratio <= 1.3) return 25; // Az yoðun
        if (ratio <= 1.5) return 50; // Orta yoðun
        if (ratio <= 2.0) return 75; // Yoðun
        return 100; // Çok yoðun
    }
    
    private Route ConvertToRoute(GoogleRoute googleRoute, TransportMode mode, Location start, Location end)
    {
        var route = new Route
        {
            RouteId = Guid.NewGuid().ToString(),
            Start = start,
            End = end,
            TransportMode = mode,
            DistanceKm = googleRoute.Legs.Sum(l => l.Distance.Value) / 1000.0,
            Duration = TimeSpan.FromSeconds(googleRoute.Legs.Sum(l => l.Duration.Value)),
            DurationInTraffic = TimeSpan.FromSeconds(googleRoute.Legs.Sum(l => l.DurationInTraffic?.Value ?? l.Duration.Value))
        };
        
        // Adýmlarý doldur
        foreach (var leg in googleRoute.Legs)
        {
            foreach (var step in leg.Steps)
            {
                route.Steps.Add(new RouteStep
                {
                    Instruction = step.HtmlInstructions,
                    StartLocation = new Location(step.StartLocation.Lat, step.StartLocation.Lng),
                    EndLocation = new Location(step.EndLocation.Lat, step.EndLocation.Lng),
                    DistanceKm = step.Distance.Value / 1000.0,
                    Duration = TimeSpan.FromSeconds(step.Duration.Value),
                    ManeuverType = DetermineManeuverType(step)
                });
            }
        }
        
        return route;
    }
    
    private string DetermineManeuverType(GoogleStep step)
    {
        // Adýmýn açýklamasýna göre manevra tipini belirle
        // (turn-right, turn-left, merge, etc.)
        var instruction = step.HtmlInstructions.ToLower();
        
        if (instruction.Contains("right")) return "turn-right";
        if (instruction.Contains("left")) return "turn-left";
        if (instruction.Contains("merge")) return "merge";
        if (instruction.Contains("exit")) return "take-exit";
        if (instruction.Contains("continue")) return "continue";
        if (instruction.Contains("arrive")) return "arrive";
        return "straight";
    }
}

// Models/TrafficInfo.cs
public class TrafficInfo
{
    public int CongestionLevel { get; set; } // 0-100 arasý (0: akýcý, 100: çok yoðun)
    public TimeSpan DelayTime { get; set; }
    public TimeSpan TypicalTravelTime { get; set; }
    public TimeSpan CurrentTravelTime { get; set; }
    public string CongestionDescription => CongestionLevel switch
    {
        < 10 => "Akýcý",
        < 30 => "Az yoðun",
        < 60 => "Orta yoðun",
        < 85 => "Yoðun",
        _ => "Çok yoðun"
    };
}

// Models/TrafficIncident.cs
public class TrafficIncident
{
    public string Id { get; set; }
    public string Type { get; set; } // ACCIDENT, CONSTRUCTION, ROAD_CLOSED, etc.
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int SeverityCode { get; set; } // 1-4 (1: minor, 4: major)
    public string SeverityDescription => SeverityCode switch
    {
        1 => "Az etkili",
        2 => "Orta etkili",
        3 => "Ciddi",
        4 => "Çok ciddi",
        _ => "Bilinmiyor"
    };
}

// Models/RouteOptimizationPreference.cs
public enum RouteOptimizationPreference
{
    BestGuess,
    Optimistic,
    Pessimistic
}

// Google API Yanýt Modelleri (temsili)
public class GoogleDistanceMatrixResponse
{
    public List<GoogleDMRow> Rows { get; set; }
}

public class GoogleDMRow
{
    public List<GoogleDMElement> Elements { get; set; }
}

public class GoogleDMElement
{
    public string Status { get; set; }
    public GoogleDMValue Duration { get; set; }
    public GoogleDMValue DurationInTraffic { get; set; }
    public GoogleDMValue Distance { get; set; }
}

public class GoogleDMValue
{
    public int Value { get; set; }
    public string Text { get; set; }
}

public class GoogleDirectionsResponse
{
    public string Status { get; set; }
    public List<GoogleRoute> Routes { get; set; }
}

public class GoogleRoute
{
    public string Summary { get; set; }
    public List<GoogleLeg> Legs { get; set; }
    public GooglePolyline OverviewPolyline { get; set; }
}

public class GoogleLeg
{
    public GoogleDMValue Distance { get; set; }
    public GoogleDMValue Duration { get; set; }
    public GoogleDMValue DurationInTraffic { get; set; }
    public List<GoogleStep> Steps { get; set; }
}

public class GoogleStep
{
    public string HtmlInstructions { get; set; }
    public GoogleDMValue Distance { get; set; }
    public GoogleDMValue Duration { get; set; }
    public LatLng StartLocation { get; set; }
    public LatLng EndLocation { get; set; }
}

public class GooglePolyline
{
    public string Points { get; set; }
}