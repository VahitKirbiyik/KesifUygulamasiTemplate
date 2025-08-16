// Services/LocationSharingService.cs
public class LocationSharingService : ILocationSharingService
{
    private readonly IShare _share;
    private readonly IMap _map;
    private readonly IBarcodeGeneratorService _barcodeGenerator;
    
    public LocationSharingService(IShare share, IMap map, IBarcodeGeneratorService barcodeGenerator)
    {
        _share = share;
        _map = map;
        _barcodeGenerator = barcodeGenerator;
    }
    
    public async Task ShareLocationViaTextAsync(Location location, string message = null)
    {
        var locationText = $"?? Konumum: {location.Latitude}, {location.Longitude}";
        if (!string.IsNullOrEmpty(message))
            locationText = $"{message}\n\n{locationText}";
            
        var googleMapsUrl = $"https://maps.google.com/maps?q={location.Latitude},{location.Longitude}";
        locationText += $"\n\n{googleMapsUrl}";
        
        await _share.RequestAsync(new ShareTextRequest
        {
            Text = locationText,
            Title = "Konum Paylaþ"
        });
    }
    
    public async Task ShareLocationViaMapAppAsync(Location location, string label = "Paylaþýlan Konum")
    {
        await _map.OpenAsync(location.Latitude, location.Longitude, new MapLaunchOptions
        {
            Name = label,
            NavigationMode = NavigationMode.None
        });
    }
    
    public async Task<ImageSource> GenerateQrCodeForLocationAsync(Location location)
    {
        var locationUrl = $"https://maps.google.com/maps?q={location.Latitude},{location.Longitude}";
        var qrCodeImage = await _barcodeGenerator.GenerateQrCodeAsync(locationUrl, 300, 300);
        return ImageSource.FromStream(() => new MemoryStream(qrCodeImage));
    }
    
    public async Task ShareLocationViaEmailAsync(Location location, string email, string subject = "Konum Paylaþýmý")
    {
        var body = $"Merhaba,\n\nAþaðýdaki baðlantýdan konumuma ulaþabilirsin:\n\nhttps://maps.google.com/maps?q={location.Latitude},{location.Longitude}\n\nÝyi günler!";
        
        var message = new EmailMessage
        {
            Subject = subject,
            Body = body,
            To = new List<string> { email }
        };
        
        await Email.ComposeAsync(message);
    }
}