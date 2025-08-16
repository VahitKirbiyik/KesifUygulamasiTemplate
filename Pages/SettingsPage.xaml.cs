using System;
using KesifUygulamasiTemplate.ViewModels;
using Microsoft.Maui.Controls;

namespace KesifUygulamasi.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void OfflineModeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            // MVVM ile offline mod yönetimi
        }

        private void RequestLocationPermission_Clicked(object sender, EventArgs e)
        {
            // MVVM ile konum izni yönetimi
        }
    }
}
