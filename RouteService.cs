public class RouteService
{
    public async Task<List<Location>> GetRouteAsync(Location start, Location end)
    {
        // Dummy veri: Sadece iki nokta aras� d�z �izgi
        return new List<Location> { start, end };
    }
}
