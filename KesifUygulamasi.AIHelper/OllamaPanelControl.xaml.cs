using System.Windows;
using System.Windows.Controls;
using KesifUygulamasiTemplate.Services;

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
            string prompt = PromptBox.Text;
            if (!string.IsNullOrWhiteSpace(prompt))
            {
                OutputBox.Text += $"\n> {prompt}";
                string response = await _ollama.GenerateTextAsync("llama3:13b", prompt);
                OutputBox.Text += $"\n{response}";
                PromptBox.Clear();
            }
        }
    }
}
