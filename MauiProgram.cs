using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Maui;
using KesifUygulamasiTemplate.Services;
using KesifUygulamasiTemplate.Services.Interfaces;
using KesifUygulamasiTemplate.Views;
using KesifUygulamasiTemplate.ViewModels;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using System.IO;
using Microsoft.Maui.Storage;

namespace KesifUygulamasiTemplate
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // -----------------------
            // Core Services
            // -----------------------
            // Database service initialized with app data path
            builder.Services.AddSingleton<DatabaseService>(sp => new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "appdata.db3")));
            builder.Services.AddSingleton<ConnectivityService>();
            builder.Services.AddSingleton<IMoonCompassService, MoonCompassService>();
            builder.Services.AddSingleton<ICompassService, CompassService>();
            builder.Services.AddSingleton<ICompassCalibrationService, CompassCalibrationService>();
            builder.Services.AddSingleton<LocationService>();
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<NavigationService>();
            builder.Services.AddSingleton<NotificationService>();
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<PushNotificationService>();
            builder.Services.AddSingleton<AppCenterAnalyticsService>();
            builder.Services.AddSingleton<ISecureDataService, SecureDataService>();

            // Platform-specific AR services
    #if ANDROID
            builder.Services.AddSingleton<IARPlatformService, Platforms.Android.ARCoreService>();
            builder.Services.AddSingleton<IARService, Platforms.Android.ARServiceAndroid>();
    #elif IOS
            builder.Services.AddSingleton<IARPlatformService, Platforms.iOS.ARKitService>();
            builder.Services.AddSingleton<IARService, Platforms.iOS.ARServiceiOS>();
    #endif

            // -----------------------
            // Additional Navigation & Map-related Services (Dependency Injection)
            // Place these after core services so they can consume DatabaseService, ConnectivityService, etc.
            // Most map/navigation services are singletons (cache/stateful), change to AddTransient if needed.
            // -----------------------
            builder.Services.AddSingleton<INavigationVoiceService, NavigationVoiceService>();
            builder.Services.AddSingleton<IOfflineRouteService, OfflineRouteService>();
            builder.Services.AddSingleton<IRoutingService, RoutingService>();
            builder.Services.AddSingleton<IEmergencyPointsService, EmergencyPointsService>();
            builder.Services.AddSingleton<IFavoritePlacesService, FavoritePlacesService>();
            builder.Services.AddSingleton<ITrafficService, TrafficService>();
            builder.Services.AddSingleton<ISearchService, SearchService>();
            builder.Services.AddSingleton<IStreetViewService, StreetViewService>();
            builder.Services.AddSingleton<IPreferencesService, PreferencesService>();
            builder.Services.AddSingleton<ILocationSharingService, LocationSharingService>();

            // If you have a MapDataService / IMapDataService, register it so others can use it:
            // (Uncomment the next line only if IMapDataService and MapDataService exist in your project)
            // builder.Services.AddSingleton<IMapDataService, MapDataService>();

            // -----------------------
            // ViewModels
            // -----------------------
            builder.Services.AddTransient<MoonCompassViewModel>();
            builder.Services.AddTransient<CompassViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<LanguageSettingsViewModel>();
            builder.Services.AddTransient<ARMoonCompassViewModel>();
            builder.Services.AddTransient<RouteViewModel>();

            // -----------------------
            // Views / Pages
            // -----------------------
            builder.Services.AddTransient<MoonCompassPage>();
            builder.Services.AddTransient<CompassPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<ARMoonCompassPage>();

    #if DEBUG
            builder.Logging.AddDebug();
    #endif

            return builder.Build();
        }
    }
}
