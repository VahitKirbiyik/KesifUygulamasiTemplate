using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Extensibility.UI;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;

namespace KesifUygulamasiTemplate.KesifUygulamasi.OllamaTool
{
    [VisualStudioContribution]
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
