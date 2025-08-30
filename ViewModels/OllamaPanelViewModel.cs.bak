using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class OllamaPanelViewModel : INotifyPropertyChanged
    {
        private readonly OllamaService _ollamaService;
        private string _soru;
        private string _cevap;
        private bool _panelAcik = false;

        public string Soru
        {
            get => _soru;
            set { _soru = value; OnPropertyChanged(); }
        }

        public string Cevap
        {
            get => _cevap;
            set { _cevap = value; OnPropertyChanged(); }
        }

        public bool PanelAcik
        {
            get => _panelAcik;
            set { _panelAcik = value; OnPropertyChanged(); }
        }

        public ICommand SoruSorCommand { get; }
        public ICommand PanelToggleCommand { get; }

        public OllamaPanelViewModel()
        {
            _ollamaService = new OllamaService();
            SoruSorCommand = new Command(async () => await SoruSor());
            PanelToggleCommand = new Command(() => PanelAcik = !PanelAcik);
        }

        private async Task SoruSor()
        {
            if (!string.IsNullOrWhiteSpace(Soru))
            {
                Cevap = "⏳ Yanıt bekleniyor...";
                try
                {
                    Cevap = await _ollamaService.SoruSorAsync(Soru);
                }
                catch (Exception ex)
                {
                    Cevap = $"⚠️ Hata: {ex.Message}";
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
