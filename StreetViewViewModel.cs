public class StreetViewViewModel : BaseViewModel
{
    private readonly IStreetViewService _streetViewService;
    private readonly IGeolocation _geolocation;

    public StreetViewPanorama CurrentPanorama { get; set; }
    public ObservableCollection<StreetViewLink> Links { get; set; } = new();

    public Command LoadCurrentLocationPanoramaCommand { get; }
    public Command<StreetViewLink> NavigateToLinkCommand { get; }

    public StreetViewViewModel(IStreetViewService streetViewService, IGeolocation geolocation)
    {
        _streetViewService = streetViewService;
        _geolocation = geolocation;

        LoadCurrentLocationPanoramaCommand = new Command(async () => await LoadCurrentLocationPanorama());
        NavigateToLinkCommand = new Command<StreetViewLink>(async (link) => await LoadPanoramaById(link.PanoramaId));
    }

    private async Task LoadCurrentLocationPanorama()
    {
        var location = await _geolocation.GetLastKnownLocationAsync();
        if (location != null)
        {
            var panorama = await _streetViewService.GetPanoramaAsync(location.Latitude, location.Longitude);
            SetPanorama(panorama);
        }
    }

    private async Task LoadPanoramaById(string panoramaId)
    {
        var panorama = await _streetViewService.GetPanoramaByIdAsync(panoramaId);
        SetPanorama(panorama);
    }

    private void SetPanorama(StreetViewPanorama panorama)
    {
        CurrentPanorama = panorama;
        Links.Clear();
        foreach (var link in panorama.Links)
            Links.Add(link);
        OnPropertyChanged(nameof(CurrentPanorama));
        OnPropertyChanged(nameof(Links));
    }
}