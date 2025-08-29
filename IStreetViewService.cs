public interface IStreetViewService
{
    Task<StreetViewPanorama> GetPanoramaAsync(double latitude, double longitude);
    Task<StreetViewPanorama> GetPanoramaByIdAsync(string panoramaId);
}