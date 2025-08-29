using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace KesifUygulamasi.OllamaTool
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    public sealed class OllamaToolPackage : AsyncPackage
    {
        public const string PackageGuidString = "462d2fdd-29c7-40d8-92eb-897fe53ae136";

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            // Panel veya komut başlatma kodları buraya eklenebilir
        }

        #endregion
    }
}