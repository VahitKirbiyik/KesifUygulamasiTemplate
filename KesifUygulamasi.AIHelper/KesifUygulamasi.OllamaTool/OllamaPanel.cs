// File: OllamaPanelToolWindow.cs
using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.Extensibility.UI;

namespace KesifUygulamasiTemplate.KesifUygulamasi.OllamaTool
{
    [VisualStudioContribution]
    public class OllamaPanel : ToolWindow
    {
        private OllamaPanelContent _content;

        public OllamaPanel()
        {
            Title = "Ollama Chat";
        }

        public override ToolWindowConfiguration ToolWindowConfiguration => new ToolWindowConfiguration
        {
            // Sağ tarafa dock’lanmış bir araç penceresi
            Placement = ToolWindowPlacement.DocumentWell,
            DockDirection = Dock.Right,
            InitialWidth = 400,
            InitialHeight = 600
        };

        public override Task InitializeAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
        {
            _content ??= new OllamaPanelContent();
            return Task.FromResult<IRemoteUserControl>(_content);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _content?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Remote UI içeriği: XAML’i programatik olarak sağlar ve bir ViewModel’e bağlanır.
    /// </summary>
    internal sealed class OllamaPanelContent : RemoteUserControl
    {
        public OllamaPanelContent() : base(new OllamaPanelViewModel())
        {
        }

        public override Task<string> GetXamlAsync(CancellationToken cancellationToken)
        {
            const string xaml = @"
<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
              xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
    <Grid Margin='8'>
        <Grid.RowDefinitions>
            <RowDefinition Height='*' />
            <RowDefinition Height='Auto' />
            <RowDefinition Height='Auto' />
        </Grid.RowDefinitions>

        <!-- Sohbet geçmişi -->
        <TextBox Grid.Row='0'
                 Text='{Binding ChatHistory}'
                 IsReadOnly='True'
                 AcceptsReturn='True'
                 VerticalScrollBarVisibility='Auto'
                 TextWrapping='Wrap'/>

        <!-- Prompt girişi -->
        <TextBox Grid.Row='1'
                 Margin='0,8,0,8'
                 Text='{Binding Prompt, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}'
                 AcceptsReturn='False'/>

        <!-- Gönder -->
        <Button Grid.Row='2'
                Content='Gönder'
                Width='100'
                HorizontalAlignment='Right'
                Command='{Binding SendCommand}'/>
    </Grid>
</DataTemplate>";
            return Task.FromResult(xaml);
        }
    }

    /// <summary>
    /// Remote UI DataContext (ViewModel). DataContract/DataMember zorunludur.
    /// </summary>
    [DataContract]
    internal sealed class OllamaPanelViewModel : NotifyPropertyChangedObject
    {
        private string _chatHistory = string.Empty;
        private string _prompt = string.Empty;

        public OllamaPanelViewModel()
        {
            // UI tarafından tetiklenen komut
            SendCommand = new AsyncCommand(async (parameter, cancellationToken) =>
            {
                var input = Prompt?.Trim();
                if (!string.IsNullOrEmpty(input))
                {
                    ChatHistory += "Sen: " + input + Environment.NewLine;
                    Prompt = string.Empty;

                    // TODO: Buraya Ollama CLI/API entegrasyonunu ekleyebilirsin.
                    string cevap = ">>> Bot cevabı buraya gelecek...";
                    ChatHistory += "Bot: " + cevap + Environment.NewLine;
                }

                await Task.CompletedTask;
            });
        }

        [DataMember]
        public string ChatHistory
        {
            get => _chatHistory;
            set => SetProperty(ref _chatHistory, value);
        }

        [DataMember]
        public string Prompt
        {
            get => _prompt;
            set => SetProperty(ref _prompt, value);
        }

        // Remote UI, IAsyncCommand’i proxy’leyerek XAML’de ICommand olarak bağlar.
        [DataMember]
        public AsyncCommand SendCommand { get; }
    }
}
