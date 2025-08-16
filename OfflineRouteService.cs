// Services/OfflineRouteService.cs
public class OfflineRouteService : IOfflineRouteService
{
    private readonly SQLiteConnection _database;
    
    public OfflineRouteService(SQLiteConnection database)
    {
        _database = database;
        _database.CreateTable<SavedRoute>();
        _database.CreateTable<RoutePoint>();
    }
    
    public async Task<int> SaveRouteAsync(SavedRoute route, List<Location> points)
    {
        _database.BeginTransaction();
        try
        {
            var routeId = _database.Insert(route);
            
            for (int i = 0; i < points.Count; i++)
            {
                _database.Insert(new RoutePoint
                {
                    RouteId = routeId,
                    Latitude = points[i].Latitude,
                    Longitude = points[i].Longitude,
                    Sequence = i
                });
            }
            
            _database.Commit();
            return routeId;
        }
        catch (Exception ex)
        {
            _database.Rollback();
            Console.WriteLine($"Rota kaydetme hatasý: {ex.Message}");
            throw;
        }
    }
    
    public SavedRoute GetRouteById(int id)
    {
        var route = _database.Get<SavedRoute>(id);
        if (route != null)
        {
            route.Points = _database.Table<RoutePoint>()
                .Where(p => p.RouteId == id)
                .OrderBy(p => p.Sequence)
                .Select(p => new Location(p.Latitude, p.Longitude))
                .ToList();
        }
        return route;
    }
    
    public List<SavedRoute> GetAllRoutes()
    {
        var routes = _database.Table<SavedRoute>().ToList();
        
        foreach (var route in routes)
        {
            route.Points = _database.Table<RoutePoint>()
                .Where(p => p.RouteId == route.Id)
                .OrderBy(p => p.Sequence)
                .Select(p => new Location(p.Latitude, p.Longitude))
                .ToList();
        }
        
        return routes;
    }
    
    public bool DeleteRoute(int id)
    {
        _database.BeginTransaction();
        try
        {
            _database.Delete<RoutePoint>(p => p.RouteId == id);
            _database.Delete<SavedRoute>(id);
            _database.Commit();
            return true;
        }
        catch
        {
            _database.Rollback();
            return false;
        }
    }
}

// Models/SavedRoute.cs
public class SavedRoute
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public double EndLatitude { get; set; }
    public double EndLongitude { get; set; }
    public double DistanceKm { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public string TransportMode { get; set; } = "Driving";
    
    [Ignore]
    public List<Location> Points { get; set; } = new();
}

// Models/RoutePoint.cs
public class RoutePoint
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public int RouteId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Sequence { get; set; }
}