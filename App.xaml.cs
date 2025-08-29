using Microsoft.Maui.Controls;
using KesifUygulamasiTemplate.Views;
using KesifUygulamasiTemplate.ViewModels;
using KesifUygulamasiTemplate.Helpers;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // 1️⃣ API Key’i SecureStorage’a ekle (ilk çalıştırmada)
            _ = EnsureApiKeyAsync();

            // 2️⃣ Ana sayfa oluştur ve NavigationPage ile ata
            var mainPage = new StreetViewPage();
            MainPage = new NavigationPage(mainPage);

            // 3️⃣ ViewModel kontrolü ve panorama yükleme
            if (mainPage?.BindingContext is StreetViewViewModel viewModel)
            {
                _ = LoadUserPanoramaAsync(viewModel);
            }
        }

        /// <summary>
        /// SecureStorage’da API Key yoksa ekler
        /// </summary>
        private async Task EnsureApiKeyAsync()
        {
            try
            {
                var existingKey = await SecureStorageHelper.GetApiKeyAsync();
                if (string.IsNullOrWhiteSpace(existingKey))
                {
                    // 🔐 Gerçek projede bu key’i Google Cloud Console’dan alın
                    var apiKey = Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_KEY") ?? "AIzaSyD3x-TESTKEY-EXAMPLE123456789";
                    await SecureStorageHelper.SetApiKeyAsync(apiKey);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Key kontrolü başarısız: {ex.Message}");
            }
        }

        /// <summary>
        /// Kullanıcının konumuna göre StreetView panorama yükler
        /// </summary>
        private async Task LoadUserPanoramaAsync(StreetViewViewModel viewModel)
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        await MainPage.DisplayAlert("İzin Gerekli", "Konum izni verilmediği için panorama yüklenemiyor.", "Tamam");
                        return;
                    }
                }

                await viewModel.LoadPanoramaByUserLocationAsync();
            }
            catch (Exception ex)
            {
                await MainPage.DisplayAlert("Hata", $"Konum yüklenemedi: {ex.Message}", "Tamam");
            }
        }
    }
}