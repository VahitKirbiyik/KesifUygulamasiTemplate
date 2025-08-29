using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace KesifUygulamasi.AIHelper.OllamaPanel
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("B1A1C123-1234-4567-89AB-CDEF12345678")]
    [ProvideToolWindow(typeof(OllamaPanelControl))]
    public sealed class OllamaPanelPackage : AsyncPackage
    {
        protected override System.Threading.Tasks.Task InitializeAsync(
            System.Threading.CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            this.JoinableTaskFactory.RunAsync(async () =>
            {
                await this.ShowToolWindowAsync(typeof(OllamaPanelControl), 0, true, cancellationToken);
            });

            return base.InitializeAsync(cancellationToken, progress);
        }
    }
}
