using System.Windows;
using System.Windows.Controls;

namespace KesifUygulamasi.AIHelper.OllamaPanel
{
    public partial class OllamaPanelControl : UserControl
    {
        private readonly OllamaService _ollama;

        public OllamaPanelControl()
        {
            InitializeComponent();
            _ollama = new OllamaService();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string prompt = InputBox.Text;
            if (!string.IsNullOrWhiteSpace(prompt))
            {
                OutputBox.Text = "Yanıt alınıyor...";
                string result = await _ollama.GenerateTextAsync("llama3:13b", prompt);
                OutputBox.Text = result;
            }
        }
    }
}
