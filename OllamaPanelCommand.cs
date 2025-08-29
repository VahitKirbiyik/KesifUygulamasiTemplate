using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.KesifUygulamasi.OllamaTool
{
    [VisualStudioContribution]
    public class OllamaPanelCommand : Command
    {
        public override CommandConfiguration CommandConfiguration => new()
        {
            DisplayName = "Ollama Panelini Aç",
            Placements = new[]
            {
                CommandPlacement.KnownPlacements.ToolsMenu
            }
        };

        public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
        {
            await this.Extensibility.Shell().ShowToolWindowAsync<OllamaPanelToolWindow>(cancellationToken);
        }
    }
}
