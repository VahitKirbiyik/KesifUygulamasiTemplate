// Services/RoutingService.cs
public class RoutingService : IRoutingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public RoutingService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["MapServices:ApiKey"];
    }
    
    public async Task<Route> CalculateRouteAsync(
        Location start, 
        Location end, 
        TransportMode mode = TransportMode.Driving,
        List<Location> waypoints = null)
    {
        var modeParam = mode switch
        {
            TransportMode.Walking => "walking",
            TransportMode.Bicycling => "bicycling",
            TransportMode.Transit => "transit",
            _ => "driving"
        };
        
        var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={start.Latitude},{start.Longitude}&destination={end.Latitude},{end.Longitude}&mode={modeParam}&key={_apiKey}";
        
        if (waypoints != null && waypoints.Any())
        {
            var waypointsParam = string.Join("|", waypoints.Select(wp => $"{wp.Latitude},{wp.Longitude}"));
            url += $"&waypoints={waypointsParam}";
        }
        
        var response = await _httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return ParseRouteResponse(content, mode);
        }
        
        return null;
    }
    
    private Route ParseRouteResponse(string json, TransportMode mode)
    {
        // Google Directions API yanýtýný ayrýþtýr
        // ve Route nesnesine dönüþtür
        var route = new Route
        {
            TransportMode = mode,
            // Diðer özellikleri doldur
        };
        
        return route;
    }
}

// Models/TransportMode.cs
public enum TransportMode
{
    Driving,
    Walking,
    Bicycling,
    Transit
}

// Models/Route.cs
public class Route
{
    public string RouteId { get; set; } = Guid.NewGuid().ToString();
    public Location Start { get; set; }
    public Location End { get; set; }
    public List<Location> Waypoints { get; set; } = new();
    public List<RouteStep> Steps { get; set; } = new();
    public double DistanceKm { get; set; }
    public TimeSpan Duration { get; set; }
    public TimeSpan DurationInTraffic { get; set; }
    public TransportMode TransportMode { get; set; }
    public List<RouteAlternative> Alternatives { get; set; } = new();
}

// Models/RouteStep.cs
public class RouteStep
{
    public string Instruction { get; set; }
    public Location StartLocation { get; set; }
    public Location EndLocation { get; set; }
    public double DistanceKm { get; set; }
    public TimeSpan Duration { get; set; }
    public string ManeuverType { get; set; } // turn-right, turn-left, etc.
}

// Models/RouteAlternative.cs
public class RouteAlternative
{
    public string Summary { get; set; }
    public double DistanceKm { get; set; }
    public TimeSpan Duration { get; set; }
    public bool HasTolls { get; set; }
    public bool HasHighways { get; set; }
}