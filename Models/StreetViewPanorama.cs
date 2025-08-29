public class StreetViewPanorama
{
    public string Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Copyright { get; set; }
    public DateTime? DateCaptured { get; set; }
    public List<StreetViewLink> Links { get; set; } = new List<StreetViewLink>();
}
