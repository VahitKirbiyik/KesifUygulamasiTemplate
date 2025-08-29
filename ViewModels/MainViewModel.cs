using System;
using System.Threading.Tasks;
using System.Windows.Input;
using KesifUygulamasiTemplate.Services;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly OllamaService _ollamaService;

        private string _generatedText;
        public string GeneratedText
        {
            get => _generatedText;
            set => SetProperty(ref _generatedText, value);
        }

        public ICommand GenerateCommand { get; }

        // DI ile OllamaService alıyoruz
        public MainViewModel(OllamaService ollamaService)
        {
            _ollamaService = ollamaService;

            // Komut: UI'daki butona bağlanacak
            GenerateCommand = new Command(async () => await GenerateTextAsync());
        }

        // Ollama'dan metin üretme
        private async Task GenerateTextAsync()
        {
            try
            {
                string prompt = "C# ile basit bir Hello World uygulaması yaz";
                GeneratedText = "Üretiliyor...";
                var result = await _ollamaService.GenerateTextAsync("llama3:13b", prompt);
                GeneratedText = result;
            }
            catch (Exception ex)
            {
                GeneratedText = $"Hata: {ex.Message}";
            }
        }
    }
}
