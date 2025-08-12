public class RouteService
{
    public async Task<List<Location>> GetRouteAsync(Location start, Location end)
    {
        // Dummy veri: Sadece iki nokta arasý düz çizgi
        return new List<Location> { start, end };
    }
}
