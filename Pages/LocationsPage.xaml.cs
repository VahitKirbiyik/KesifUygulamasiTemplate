using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using KesifUygulamasi.Models;
using KesifUygulamasi.Services;

namespace KesifUygulamasi.Pages
{
    public partial class LocationsPage : ContentPage
    {
        readonly DatabaseService _databaseService;
        ObservableCollection<LocationModel> _locations = new();

        public LocationsPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var locationList = await _databaseService.GetAllAsync();
                _locations = new ObservableCollection<LocationModel>(locationList);
                LocationsCollectionView.ItemsSource = _locations;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Konumlar yüklenemedi: {ex.Message}", "Tamam");
            }
        }

        private async void OnLocationSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is LocationModel selectedLocation)
            {
                bool silinsinMi = await DisplayAlert("Sil?", $"{selectedLocation.Title} konumu silinsin mi?", "Evet", "Hayır");
                if (silinsinMi)
                {
                    try
                    {
                        await _databaseService.DeleteLocationAsync(selectedLocation);
                        _locations.Remove(selectedLocation);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Hata", $"Silinemedi: {ex.Message}", "Tamam");
                    }
                }

                LocationsCollectionView.SelectedItem = null;
            }
        }

        private async void OnAddLocationClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Map");
        }
    }
}
