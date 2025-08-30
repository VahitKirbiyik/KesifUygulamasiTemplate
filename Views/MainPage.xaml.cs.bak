using Microsoft.Maui.Controls;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly OllamaService _ollamaService;

        public MainPage(OllamaService ollamaService)
        {
            InitializeComponent();
            _ollamaService = ollamaService;
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            string prompt = PromptEntry.Text;
            if (string.IsNullOrWhiteSpace(prompt)) return;

            OutputBox.Text = "Yanıt bekleniyor...";
            string response = await _ollamaService.GenerateTextAsync("llama3:13b", prompt);
            OutputBox.Text = response;
        }
    }
}
