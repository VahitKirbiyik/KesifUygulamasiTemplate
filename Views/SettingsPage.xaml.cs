using System;
using System.Globalization;
using KesifUygulamasiTemplate.Services;
using KesifUygulamasiTemplate.ViewModels;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace KesifUygulamasiTemplate.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
            LocalizationService.Instance.PropertyChanged += LocalizationService_PropertyChanged;
        }

        private void LocalizationService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LocalizationService.CurrentCulture) || string.IsNullOrEmpty(e.PropertyName))
            {
                _viewModel.OnPropertyChanged(nameof(_viewModel.IsTurkishSelected));
                _viewModel.OnPropertyChanged(nameof(_viewModel.IsEnglishSelected));
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckLocationPermission();
        }

        private void Turkish_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                LocalizationService.CurrentCulture = new CultureInfo("tr");
            }
        }

        private void English_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                LocalizationService.CurrentCulture = new CultureInfo("en");
            }
        }

        private void OfflineModeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            _viewModel.SetOfflineMode(e.Value);
        }

        private void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Tema değişikliği kodu
        }

        private async void RequestLocationPermission_Clicked(object sender, EventArgs e)
        {
            await _viewModel.RequestLocationPermissionAsync();
            CheckLocationPermission();
        }

        private async void CheckLocationPermission()
        {
            var status = await _viewModel.CheckLocationPermissionAsync();
            LocationPermissionLabel.Text = status;
        }
    }
}
