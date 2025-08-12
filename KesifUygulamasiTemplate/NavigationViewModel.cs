public class NavigationViewModel : BaseViewModel
{
    private readonly ICompassService _compassService;
    private readonly ILocationService _locationService;
    
    public Location CurrentLocation { get; private set; }
    public Location TargetLocation { get; set; }
    public double BearingToTarget { get; private set; } // Hedefe olan yön
    
    // Ýki nokta arasýndaki yön hesaplama
    private double CalculateBearing(Location start, Location end) 
    {
        // Haversine formülü hesaplamasý
    }
}
