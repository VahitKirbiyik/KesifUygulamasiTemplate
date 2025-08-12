public interface IRouteService
{
    Task<RouteModel> GetRouteAsync(Location start, Location end, string travelMode = "driving");
}

public class RouteService : IRouteService
{
    public async Task<RouteModel> GetRouteAsync(Location start, Location end, string travelMode = "driving")
    {
        // Gerçek API entegrasyonu burada olacak
        // Þimdilik dummy veri:
        return new RouteModel
        {
            Points = new List<Location> { start, end },
            TotalDistance = 1.2, // km
            EstimatedTime = TimeSpan.FromMinutes(15)
        };
    }
}

public class RouteModel
{
    public List<Location> Points { get; set; } = new();
    public double TotalDistance { get; set; }
    public TimeSpan EstimatedTime { get; set; }
}
