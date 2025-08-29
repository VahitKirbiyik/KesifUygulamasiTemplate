using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.Resources.Strings
{
    // Dinamik indexer ile string kaynaklar�na eri�im sa�layan s�n�f
    public class AppResources : INotifyPropertyChanged
    {
        public static string AppName => LocalizationService.Instance.GetString(nameof(AppName));
        public static string Calibrating => LocalizationService.Instance.GetString(nameof(Calibrating));
        public static string CalibrationComplete => LocalizationService.Instance.GetString(nameof(CalibrationComplete));
        public static string CalibrationRequired => LocalizationService.Instance.GetString(nameof(CalibrationRequired));
        public static string Cancel => LocalizationService.Instance.GetString(nameof(Cancel));
        public static string Compass => LocalizationService.Instance.GetString(nameof(Compass));
        public static string Error => LocalizationService.Instance.GetString(nameof(Error));
        public static string Loading => LocalizationService.Instance.GetString(nameof(Loading));
        public static string LocationAccessRequired => LocalizationService.Instance.GetString(nameof(LocationAccessRequired));
        public static string MoonCompass => LocalizationService.Instance.GetString(nameof(MoonCompass));
        public static string MoonPhase => LocalizationService.Instance.GetString(nameof(MoonPhase));
        public static string MoonRise => LocalizationService.Instance.GetString(nameof(MoonRise));
        public static string MoonSet => LocalizationService.Instance.GetString(nameof(MoonSet));
        public static string OK => LocalizationService.Instance.GetString(nameof(OK));
        public static string Refresh => LocalizationService.Instance.GetString(nameof(Refresh));
        public static string Settings => LocalizationService.Instance.GetString(nameof(Settings));
        public static string Start => LocalizationService.Instance.GetString(nameof(Start));
        public static string Stop => LocalizationService.Instance.GetString(nameof(Stop));
        public static string Language => LocalizationService.Instance.GetString(nameof(Language));
        public static string LanguageEnglish => LocalizationService.Instance.GetString(nameof(LanguageEnglish));
        public static string LanguageTurkish => LocalizationService.Instance.GetString(nameof(LanguageTurkish));
        public static string ChangeLanguage => LocalizationService.Instance.GetString(nameof(ChangeLanguage));
        public static string DefaultLocation => LocalizationService.Instance.GetString(nameof(DefaultLocation));
        public static string LocationNotAvailable => LocalizationService.Instance.GetString(nameof(LocationNotAvailable));
        public static string PermissionGranted => LocalizationService.Instance.GetString(nameof(PermissionGranted));
        public static string PermissionDenied => LocalizationService.Instance.GetString(nameof(PermissionDenied));
        public static string MoonInformation => LocalizationService.Instance.GetString(nameof(MoonInformation));
        public static string LocationUpdated => LocalizationService.Instance.GetString(nameof(LocationUpdated));
        public static string ShareMoonData => LocalizationService.Instance.GetString(nameof(ShareMoonData));
        public static string ShareSuccessful => LocalizationService.Instance.GetString(nameof(ShareSuccessful));
        public static string InternetConnected => LocalizationService.Instance.GetString(nameof(InternetConnected));
        public static string InternetDisconnected => LocalizationService.Instance.GetString(nameof(InternetDisconnected));

        // INotifyPropertyChanged implementasyonu dil de�i�ikli�inde UI g�ncellemesi i�in
        public event PropertyChangedEventHandler? PropertyChanged;

        // StringFormat ile kaynak metnini formatla
        public static string FormatString(string resourceKey, params object[] args)
        {
            return LocalizationService.Instance.GetString(resourceKey, args);
        }

        // Tarih, saat ve say� formatlar� i�in yard�mc� metotlar
        public static string FormatDate(DateTime date)
        {
            return LocalizationService.FormatDate(date);
        }

        public static string FormatTime(DateTime time)
        {
            return LocalizationService.FormatTime(time);
        }

        public static string FormatNumber(double number)
        {
            return LocalizationService.FormatNumber(number);
        }

        public static string FormatCurrency(decimal amount)
        {
            return LocalizationService.FormatCurrency(amount);
        }

        // ResourceManager i�in gerekli
        private static System.Resources.ResourceManager? resourceManager;
        public static System.Resources.ResourceManager ResourceManager => resourceManager ?? (resourceManager = new System.Resources.ResourceManager(typeof(AppResources)));
    }
}
