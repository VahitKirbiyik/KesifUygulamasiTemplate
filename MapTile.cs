// Eklenmesi gereken:
public class MapTile
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int ZoomLevel { get; set; }
    public double North { get; set; }
    public double South { get; set; }
    public double East { get; set; }
    public double West { get; set; }
    public byte[] Data { get; set; }
    public DateTime LastUpdated { get; set; }
}