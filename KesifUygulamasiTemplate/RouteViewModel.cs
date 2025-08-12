using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using KesifUygulamasiTemplate.ViewModels;
using KesifUygulamasiTemplate.Services.Interfaces;

namespace KesifUygulamasiTemplate.ViewModels
{
    // ViewModel'leri CommunityToolkit.Mvvm özellikleriyle güçlendir
    public partial class RouteViewModel : BaseViewModel
    {
        private readonly IRouteService _routeService;

        [ObservableProperty]
        private Location? startLocation;
        
        [ObservableProperty]
        private Location? endLocation;
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasValidRoute))]
        private ObservableCollection<Location> routePoints = new();
        
        public bool HasValidRoute => RoutePoints?.Count > 1;

        public RouteViewModel(IRouteService routeService)
        {
            _routeService = routeService;
        }
        
        [RelayCommand(CanExecute = nameof(CanGenerateRoute))]
        private async Task GenerateRouteAsync()
        {
            if (StartLocation != null && EndLocation != null)
            {
                var route = await _routeService.CalculateRouteAsync(StartLocation, EndLocation);
                RoutePoints = new ObservableCollection<Location>(route);
            }
        }
        
        private bool CanGenerateRoute() => 
            StartLocation != null && EndLocation != null && !IsBusy;
    }
}
