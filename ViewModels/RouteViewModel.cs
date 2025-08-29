using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using KesifUygulamasiTemplate.Models;
using KesifUygulamasiTemplate.Services.Interfaces;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class RouteViewModel : INotifyPropertyChanged
    {
        private LocationModel? _startLocation;
        private LocationModel? _endLocation;
        private ObservableCollection<LocationModel> _routePoints;
        private bool _isBusy;
        private string _errorMessage;

        public event PropertyChangedEventHandler? PropertyChanged;

        public LocationModel? StartLocation
        {
            get => _startLocation;
            set
            {
                _startLocation = value;
                OnPropertyChanged();
            }
        }

        public LocationModel? EndLocation
        {
            get => _endLocation;
            set
            {
                _endLocation = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LocationModel> RoutePoints
        {
            get => _routePoints;
            set
            {
                _routePoints = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand GetRouteCommand { get; }
        public ICommand GenerateRouteCommand { get; }

        private readonly IRouteService _routeService;

        public RouteViewModel(IRouteService routeService)
        {
            _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));
            _routePoints = new ObservableCollection<LocationModel>();
            _errorMessage = string.Empty;
            GetRouteCommand = new Command(async () => await GenerateRouteAsync());
            GenerateRouteCommand = new Command(async () => await GenerateRouteAsync());
        }

        public RouteViewModel()
        {
            _routePoints = new ObservableCollection<LocationModel>();
            _errorMessage = string.Empty;
            GetRouteCommand = new Command(async () => await GenerateRouteAsync());
            GenerateRouteCommand = new Command(async () => await GenerateRouteAsync());
        }

        private async Task GenerateRouteAsync()
        {
            if (StartLocation == null || EndLocation == null)
            {
                ErrorMessage = "Başlangıç ve bitiş noktalarını belirtin";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                // Route generation logic here
                // This is a placeholder - implement actual routing logic
                await Task.Delay(100); // Simulate async operation
                RoutePoints.Clear();
                if (StartLocation != null && EndLocation != null)
                {
                    RoutePoints.Add(StartLocation);
                    RoutePoints.Add(EndLocation);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Rota oluşturma hatası: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }
}
