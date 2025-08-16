public class ARPageViewModel
{
    private readonly IARService _arService;
    public ARPageViewModel(IARService arService)
    {
        _arService = arService;
    }
    public void StartAR() => _arService.StartARSession();
    public void StopAR() => _arService.StopARSession();
}