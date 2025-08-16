using KesifUygulamasiTemplate.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            var databaseService = Handler?.MauiContext?.Services?.GetService<DatabaseService>();
            if (databaseService != null)
            {
                await databaseService.InitializeAsync();
            }
        }
    }
}