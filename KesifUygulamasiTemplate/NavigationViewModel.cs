public class NavigationViewModel : BaseViewModel
{
    private readonly ICompassService _compassService;
    private readonly ILocationService _locationService;
    
    public Location CurrentLocation { get; private set; }
    public Location TargetLocation { get; set; }
    public double BearingToTarget { get; private set; } // Hedefe olan y�n
    
    // �ki nokta aras�ndaki y�n hesaplama
    private double CalculateBearing(Location start, Location end) 
    {
        // Haversine form�l� hesaplamas�
    }
}
