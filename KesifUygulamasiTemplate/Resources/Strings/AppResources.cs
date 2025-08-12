using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.Resources.Strings
{
    // Dinamik indexer ile string kaynaklarýna eriþim saðlayan sýnýf
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

        // INotifyPropertyChanged implementasyonu dil deðiþikliðinde UI güncellemesi için
        public event PropertyChangedEventHandler PropertyChanged;

        // StringFormat ile kaynak metnini formatla
        public static string FormatString(string resourceKey, params object[] args)
        {
            return LocalizationService.Instance.GetString(resourceKey, args);
        }
        
        // Tarih, saat ve sayý formatlarý için yardýmcý metotlar
        public static string FormatDate(DateTime date)
        {
            return LocalizationService.Instance.FormatDate(date);
        }
        
        public static string FormatTime(DateTime time)
        {
            return LocalizationService.Instance.FormatTime(time);
        }
        
        public static string FormatNumber(double number)
        {
            return LocalizationService.Instance.FormatNumber(number);
        }
        
        public static string FormatCurrency(decimal amount)
        {
            return LocalizationService.Instance.FormatCurrency(amount);
        }
        
        // ResourceManager için gerekli
        private static System.Resources.ResourceManager resourceManager;
        public static System.Resources.ResourceManager ResourceManager => resourceManager ?? (resourceManager = new System.Resources.ResourceManager(typeof(AppResources)));
    }
}