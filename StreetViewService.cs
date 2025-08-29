using System.Net.Http.Json;

public class StreetViewService : IStreetViewService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public StreetViewService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<StreetViewPanorama> GetPanoramaAsync(double latitude, double longitude)
    {
        // Google Street View API endpoint örneği
        var url = $"https://maps.googleapis.com/maps/api/streetview/metadata?location={latitude},{longitude}&key={_apiKey}";
        var response = await _httpClient.GetFromJsonAsync<StreetViewPanorama>(url);
        return response;
    }

    public async Task<StreetViewPanorama> GetPanoramaByIdAsync(string panoramaId)
    {
        // PanoramaId ile Street View verisi çekme (örnek)
        var url = $"https://maps.googleapis.com/maps/api/streetview/metadata?pano={panoramaId}&key={_apiKey}";
        var response = await _httpClient.GetFromJsonAsync<StreetViewPanorama>(url);
        return response;
    }
}