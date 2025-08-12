using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Devices.Sensors;
using KesifUygulamasiTemplate.ViewModels;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.ViewModels
{
    // Rota ViewModel'i: Baþlangýç/varýþ noktasý ve polyline verisi
    public class RouteViewModel : BaseViewModel
    {
        private readonly IRouteService _routeService;
        private Location _startLocation;
        private Location _endLocation;

        public Location StartLocation
        {
            get => _startLocation;
            set => SetProperty(ref _startLocation, value);
        }

        public Location EndLocation
        {
            get => _endLocation;
            set => SetProperty(ref _endLocation, value);
        }

        public ObservableCollection<Location> RoutePoints { get; } = new();

        public ICommand GenerateRouteCommand { get; }

        public RouteViewModel(IRouteService routeService)
        {
            _routeService = routeService;
            GenerateRouteCommand = new Command(async () => await GenerateRouteAsync());
        }

        private async Task GenerateRouteAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                if (StartLocation == null || EndLocation == null)
                {
                    ErrorMessage = "Start and End locations must be set.";
                    return;
                }

                var route = await _routeService.GetRouteAsync(StartLocation, EndLocation);
                RoutePoints.Clear();
                foreach (var point in route)
                {
                    RoutePoints.Add(point);
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Failed to generate route: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
