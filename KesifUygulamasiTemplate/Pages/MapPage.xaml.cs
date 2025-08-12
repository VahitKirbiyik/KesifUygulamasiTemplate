using System;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using KesifUygulamasi.Models;
using KesifUygulamasi.Services;

namespace KesifUygulamasi.Pages
{
    public partial class MapPage : ContentPage
    {
        readonly DatabaseService _databaseService;

        public MapPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

            // Harita ayarları
            mapView.IsShowingUser = true;
            mapView.MapType = MapType.Street;

            // Kaydet butonunu başta pasif yap
            saveButton.IsEnabled = false;

            // Başlık yazıldıkça kaydet butonunu aktif/pasif et
            titleEntry.TextChanged += (s, e) =>
                saveButton.IsEnabled = !string.IsNullOrWhiteSpace(e.NewTextValue);

            // Buton olayları
            addPinButton.Clicked += OnAddPinClicked!;
            saveButton.Clicked += OnSaveClicked!;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Konum izni iste
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("İzin Gerekli", "Konum izni gerekli.", "Tamam");
                return;
            }

            // Önceki pinleri temizle
            mapView.Pins.Clear();

            // Veritabanından pinleri yükle
            try
            {
                var locations = await _databaseService.GetAllAsync();
                foreach (var loc in locations)
                {
                    mapView.Pins.Add(new Pin
                    {
                        Label = loc.Title,
                        Address = loc.Description,
                        Location = new Location(loc.Latitude, loc.Longitude)
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Pinler yüklenemedi: {ex.Message}", "Tamam");
            }
        }

        private void OnAddPinClicked(object sender, EventArgs e)
        {
            if (mapView.VisibleRegion == null)
                return;

            var center = mapView.VisibleRegion.Center;
            mapView.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(1)));

            var newLocation = new LocationModel
            {
                Title = titleEntry.Text ?? string.Empty,
                Description = descriptionEntry.Text ?? string.Empty,
                Latitude = center.Latitude,
                Longitude = center.Longitude
            };

            mapView.Pins.Add(new Pin
            {
                Label = newLocation.Title,
                Address = newLocation.Description,
                Location = new Location(newLocation.Latitude, newLocation.Longitude)
            });
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (mapView.VisibleRegion == null)
                return;

            var center = mapView.VisibleRegion.Center;
            var newLocation = new LocationModel
            {
                Title = titleEntry.Text ?? string.Empty,
                Description = descriptionEntry.Text ?? string.Empty,
                Latitude = center.Latitude,
                Longitude = center.Longitude
            };

            try
            {
                await _databaseService.SaveLocationAsync(newLocation);
                await DisplayAlert("Başarılı", "Konum kaydedildi.", "Tamam");

                titleEntry.Text = string.Empty;
                descriptionEntry.Text = string.Empty;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Kaydedilemedi: {ex.Message}", "Tamam");
            }
        }
    }
}
