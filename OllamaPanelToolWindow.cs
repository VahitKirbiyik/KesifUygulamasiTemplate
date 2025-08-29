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
            Title = "Ollama Chat";
        }

        public override ToolWindowConfiguration ToolWindowConfiguration => new()
        {
            Placement = ToolWindowPlacement.Docked,
            DockingPosition = ToolWindowDockingPosition.Right,
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
}