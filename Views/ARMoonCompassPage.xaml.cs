using KesifUygulamasiTemplate.ViewModels;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.Views
{
    public partial class ARMoonCompassPage : ContentPage
    {
        public ARMoonCompassPage(ARMoonCompassViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Kamera kodu geçici olarak devre dışı
            // await StartCameraPreviewAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // await StopCameraPreviewAsync();
        }

        // Kamera metodu geçici olarak comment out
        /*
        private async Task StartCameraPreviewAsync()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo != null)
                {
                    var stream = await photo.OpenReadAsync();
                    CameraPreview.Source = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Kamera Hatas�", ex.Message, "Tamam");
            }
        }

        private async Task StopCameraPreviewAsync()
        {
            CameraPreview.Source = null;
            await Task.CompletedTask;
        }
        */
    }
}
