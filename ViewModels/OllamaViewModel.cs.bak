using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class OllamaViewModel : INotifyPropertyChanged
    {
        private readonly OllamaService _ollamaService;

        private string _prompt;
        private string _yanit;

        public string Prompt
        {
            get => _prompt;
            set { _prompt = value; OnPropertyChanged(); }
        }

        public string Yanit
        {
            get => _yanit;
            set { _yanit = value; OnPropertyChanged(); }
        }

        public OllamaViewModel(OllamaService ollamaService)
        {
            _ollamaService = ollamaService;
        }

        public async Task SorguCalistirAsync()
        {
            if (string.IsNullOrWhiteSpace(Prompt))
                return;

            Yanit = "Ollama düşünüyor...";
            Yanit = await _ollamaService.SorguGonderAsync(Prompt);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
