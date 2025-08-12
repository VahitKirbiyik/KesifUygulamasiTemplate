using KesifUygulamasiTemplate.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

namespace KesifUygulamasiTemplate
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // AppCenter Analytics baþlat
            AppCenterAnalyticsService.Initialize();
            var offlineSync = Handler?.MauiContext?.Services?.GetService<OfflineSyncService>();
            offlineSync?.Start();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            var databaseService = Handler?.MauiContext?.Services?.GetService<DatabaseService>();
            if (databaseService != null)
            {
                try
                {
                    await databaseService.InitializeAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
                }
            }
        }
    }
}
