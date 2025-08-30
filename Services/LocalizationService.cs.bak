using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;

namespace KesifUygulamasiTemplate.Services
{
    /// <summary>
    /// Uygulama genelinde lokalizasyon ve k�lt�r y�netimi sa�lar.
    /// </summary>
    public class LocalizationService : INotifyPropertyChanged
    {
        private static readonly LocalizationService _instance = new LocalizationService();
        private readonly ResourceManager _resourceManager;
        private CultureInfo _currentCulture;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static LocalizationService Instance => _instance;

        /// <summary>
        /// Ge�erli k�lt�r (dil)
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get => Instance._currentCulture;
            set => Instance.SetCulture(value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private LocalizationService()
        {
            _resourceManager = KesifUygulamasiTemplate.Resources.Strings.AppResources.ResourceManager;
            _currentCulture = CultureInfo.CurrentUICulture;
        }

        private void SetCulture(CultureInfo value)
        {
            if (_currentCulture.Name != value.Name)
            {
                _currentCulture = value;
                Thread.CurrentThread.CurrentCulture = value;
                Thread.CurrentThread.CurrentUICulture = value;
                OnPropertyChanged(nameof(CurrentCulture));
                OnPropertyChanged(string.Empty);
            }
        }

        public string GetString(string key)
        {
            try
            {
                return _resourceManager.GetString(key, _currentCulture) ?? key;
            }
            catch
            {
                return key;
            }
        }

        public string GetString(string key, params object[] args)
        {
            try
            {
                var format = _resourceManager.GetString(key, _currentCulture) ?? key;
                return string.Format(format, args);
            }
            catch
            {
                return key;
            }
        }

        /// <summary>
        /// Tarih/saat de�erini ge�erli k�lt�re g�re k�sa tarih format�nda d�nd�r�r.
        /// </summary>
        public static string FormatDate(DateTime? date)
        {
            if (date == null) return string.Empty;
            return date.Value.ToString("d", CurrentCulture);
        }

        /// <summary>
        /// Para birimi de�erini ge�erli k�lt�re g�re formatlar.
        /// </summary>
        public static string FormatCurrency(decimal amount)
        {
            return amount.ToString("C", CurrentCulture);
        }

        /// <summary>
        /// Tarih/saat de�erini ge�erli k�lt�re g�re k�sa saat format�nda d�nd�r�r.
        /// </summary>
        public static string FormatTime(DateTime? time)
        {
            if (time == null) return string.Empty;
            return time.Value.ToString("t", CurrentCulture);
        }

        /// <summary>
        /// Say� de�erini ge�erli k�lt�re g�re formatlar.
        /// </summary>
        public static string FormatNumber(double number)
        {
            return number.ToString("N0", CurrentCulture);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }
}
