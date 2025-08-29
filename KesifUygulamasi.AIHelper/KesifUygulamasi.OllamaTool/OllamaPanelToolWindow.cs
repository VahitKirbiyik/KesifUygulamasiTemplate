using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.Extensibility.UI;
using System.Threading;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.KesifUygulamasi.OllamaTool
{
    [VisualStudioContribution]
    public class OllamaPanelToolWindow : ToolWindow
    {
        private OllamaPanelContent _content;

        public OllamaPanelToolWindow()
        {
            Title = "Ollama Paneli";
        }

        public override ToolWindowConfiguration ToolWindowConfiguration => new()
        {
            Placement = ToolWindowPlacement.Floating,
            IsInitiallyVisible = false,
            InitialWidth = 400,
            InitialHeight = 600
        };

        public override Task InitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
        {
            _content ??= new OllamaPanelContent();
            return Task.FromResult<IRemoteUserControl>(_content);
        }
    }

    internal sealed class OllamaPanelContent : RemoteUserControl
    {
        public OllamaPanelContent() : base(new OllamaPanelViewModel()) { }

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
        <TextBox Grid.Row='0'
                 Text='{Binding ChatHistory}'
                 IsReadOnly='True'
                 AcceptsReturn='True'
                 VerticalScrollBarVisibility='Auto'
                 TextWrapping='Wrap'/>
        <TextBox Grid.Row='1'
                 Margin='0,8,0,8'
                 Text='{Binding Prompt, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}'
                 AcceptsReturn='False'/>
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
}