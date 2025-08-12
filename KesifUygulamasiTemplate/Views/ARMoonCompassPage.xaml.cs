using KesifUygulamasiTemplate.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel.Media;
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await StartCameraPreviewAsync();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await StopCameraPreviewAsync();
        }

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
                await DisplayAlert("Kamera Hatasý", ex.Message, "Tamam");
            }
        }

        private async Task StopCameraPreviewAsync()
        {
            CameraPreview.Source = null;
            await Task.CompletedTask;
        }
    }
}
