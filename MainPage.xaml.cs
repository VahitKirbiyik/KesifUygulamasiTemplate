using System;
using Microsoft.Maui.Controls;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate
{
    public partial class MainPage : ContentPage
    {
        private readonly OllamaService _ollamaService;

        public MainPage(OllamaService ollamaService)
        {
            InitializeComponent();
            _ollamaService = ollamaService;
        }

        private async void OnSendPromptClicked(object sender, EventArgs e)
        {
            string prompt = PromptEntry.Text;

            if (string.IsNullOrWhiteSpace(prompt))
            {
                await DisplayAlert("Uyarı", "Lütfen bir metin giriniz.", "Tamam");
                return;
            }

            ResponseLabel.Text = "⏳ Yanıt bekleniyor...";

            try
            {
                // Örn: "llama3:13b" modelini çağırıyoruz
                string result = await _ollamaService.GenerateTextAsync("llama3:13b", prompt);

                ResponseLabel.Text = result;
            }
            catch (Exception ex)
            {
                ResponseLabel.Text = $"⚠️ Hata oluştu: {ex.Message}";
            }
        }
    }
}
