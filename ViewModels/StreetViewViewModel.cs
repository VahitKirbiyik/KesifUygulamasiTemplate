using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using KesifUygulamasiTemplate.Services;
using KesifUygulamasiTemplate.Models;
using System;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class StreetViewViewModel : INotifyPropertyChanged
    {
        private readonly StreetViewService _streetViewService = new StreetViewService();

        private StreetViewPanorama _currentPanorama;
        public StreetViewPanorama CurrentPanorama
        {
            get => _currentPanorama;
            set
            {
                _currentPanorama = value;
                OnPropertyChanged();
                UpdateStreetViewUrl();
            }
        }

        private string _streetViewUrl;
        public string StreetViewUrl
        {
            get => string.IsNullOrWhiteSpace(_streetViewUrl) ? "https://via.placeholder.com/600x400?text=Panorama+yukleniyor..." : _streetViewUrl;
            private set { _streetViewUrl = value; OnPropertyChanged(); }
        }

        public ObservableCollection<StreetViewLink> Links { get; set; } = new ObservableCollection<StreetViewLink>();

        private double _heading = 210;
        public double Heading { get => _heading; set { _heading = value; OnPropertyChanged(); UpdateStreetViewUrl(); } }

        private double _pitch = 10;
        public double Pitch { get => _pitch; set { _pitch = value; OnPropertyChanged(); UpdateStreetViewUrl(); } }

        private double _fov = 80;
        public double FOV { get => _fov; set { _fov = value; OnPropertyChanged(); UpdateStreetViewUrl(); } }

        private async void UpdateStreetViewUrl()
        {
            if (CurrentPanorama != null)
            {
                try
                {
                    var apiKey = await _streetViewService.GetApiKeyAsync();
                    StreetViewUrl = $"https://www.google.com/maps/embed/v1/streetview?key={apiKey}&location={CurrentPanorama.Latitude},{CurrentPanorama.Longitude}&heading={Heading}&pitch={Pitch}&fov={FOV}";
                }
                catch
                {
                    StreetViewUrl = "https://via.placeholder.com/600x400?text=Panorama+yuklenemedi";
                }
            }
        }

        public ICommand LoadPanoramaCommand => new Command<StreetViewLink>(async link =>
        {
            if (link == null) return;
            try
            {
                var panorama = await _streetViewService.GetPanoramaByIdAsync(link.PanoramaId);
                CurrentPanorama = panorama;
                Links.Clear();
                foreach (var l in panorama.Links)
                    Links.Add(l);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", $"Panorama yüklenemedi: {ex.Message}", "Tamam");
            }
        });

        public async Task LoadPanoramaByUserLocationAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        await Application.Current.MainPage.DisplayAlert("İzin Gerekli", "Konum izni verilmediği için panorama yüklenemiyor.", "Tamam");
                        return;
                    }
                }

                var location = await Geolocation.GetLastKnownLocationAsync()
                                ?? await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

                if (location != null)
                {
                    var panorama = await _streetViewService.GetPanorama(location.Latitude, location.Longitude);
                    CurrentPanorama = panorama;
                    Links.Clear();
                    foreach (var link in panorama.Links)
                        Links.Add(link);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", $"Konum yüklenemedi: {ex.Message}", "Tamam");
            }
        }

        public string StreetName { get; set; }
        public string City { get; set; }

        public void LoadStreetData()
        {
            // Veri yükleme işlemi
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
