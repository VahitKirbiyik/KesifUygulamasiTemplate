public class StreetViewPanorama
{
    public string Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Yeni eklenen özellikler
    public string Copyright { get; set; }
    public DateTime? DateCaptured { get; set; }
    public List<string> Links { get; set; } = new List<string>();
}
