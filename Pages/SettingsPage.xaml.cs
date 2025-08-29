using System;
using KesifUygulamasiTemplate.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

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

        private async void PayPalDonationButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                // PayPal bağış linki - gerçek implementasyonda dinamik URL kullanılmalı
                string paypalUrl = "https://www.paypal.com/donate/?hosted_button_id=YOUR_BUTTON_ID";

                await Launcher.OpenAsync(paypalUrl);

                await DisplayAlert("Teşekkürler! 💝",
                    "PayPal sayfasına yönlendiriliyorsunuz. Bağışınız için teşekkür ederiz!",
                    "Tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata",
                    $"PayPal sayfası açılamadı: {ex.Message}",
                    "Tamam");
            }
        }

        private async void StripeDonationButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Stripe bağış linki - gerçek implementasyonda Stripe Checkout kullanılmalı
                string stripeUrl = "https://buy.stripe.com/YOUR_STRIPE_LINK";

                await Launcher.OpenAsync(stripeUrl);

                await DisplayAlert("Teşekkürler! 💝",
                    "Stripe bağış sayfasına yönlendiriliyorsunuz. Bağışınız için teşekkür ederiz!",
                    "Tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata",
                    $"Stripe sayfası açılamadı: {ex.Message}",
                    "Tamam");
            }
        }
    }
}
