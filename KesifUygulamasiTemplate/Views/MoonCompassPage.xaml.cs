using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using KesifUygulamasiTemplate.ViewModels;
using KesifUygulamasiTemplate.Services;
using Microsoft.Maui.Devices.Sensors;
using KesifUygulamasiTemplate.Resources.Strings;

namespace KesifUygulamasiTemplate.Views
{
    public partial class MoonCompassPage : ContentPage
    {
        private readonly MoonCompassViewModel _viewModel;
        private readonly LocalizationService _localizationService;
        
        public MoonCompassPage(MoonCompassViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _localizationService = LocalizationService.Instance;
            BindingContext = _viewModel;
            
            // Dil deðiþikliklerini dinle
            _localizationService.PropertyChanged += (s, e) => {
                if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(LocalizationService.CurrentCulture))
                {
                    // Sayfadaki tüm çevirileri güncelle
                    OnPropertyChanged(nameof(Title));
                }
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            try
            {
                // Gerçek konum almanýn yerine sabit bir konum kullanýlýyor
                // Gerçek bir uygulamada burada Geolocation servisi kullanýlabilir
                await _viewModel.LoadMoonDataAsync(40.7128, -74.0060);
            }
            catch (Exception ex)
            {
                await DisplayAlert(AppResources.Error, ex.Message, AppResources.OK);
            }
        }
    }
}
