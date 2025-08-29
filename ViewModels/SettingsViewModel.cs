using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using KesifUygulamasiTemplate.Services;
using Microsoft.Maui.Devices.Sensors;
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
            get => _streetViewUrl;
            private set { _streetViewUrl = value; OnPropertyChanged(); }
        }

        public ObservableCollection<StreetViewLink> Links { get; set; } = new ObservableCollection<StreetViewLink>();

        private async void UpdateStreetViewUrl()
        {
            if (CurrentPanorama != null)
            {
                var apiKey = await _streetViewService.GetApiKeyAsync();
                StreetViewUrl = $"https://www.google.com/maps/embed/v1/streetview?key={apiKey}&location={CurrentPanorama.Latitude},{CurrentPanorama.Longitude}&heading=210&pitch=10&fov=80";
            }
        }

        public ICommand LoadPanoramaCommand => new Command<string>(async panoramaId =>
        {
            try
            {
                var panorama = await _streetViewService.GetPanoramaByIdAsync(panoramaId);
                CurrentPanorama = panorama;
                Links.Clear();
                foreach (var link in panorama.Links)
                    Links.Add(link);
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
