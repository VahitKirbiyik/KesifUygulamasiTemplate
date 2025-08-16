using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;

namespace KesifUygulamasiTemplate.Services
{
    /// <summary>
    /// Uygulama genelinde lokalizasyon ve kültür yönetimi saðlar.
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
        /// Geçerli kültür (dil)
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get => Instance._currentCulture;
            set => Instance.SetCulture(value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
        /// Tarih/saat deðerini geçerli kültüre göre kýsa saat formatýnda döndürür.
        /// </summary>
        public static string FormatTime(DateTime? time)
        {
            if (time == null) return string.Empty;
            return time.Value.ToString("t", CurrentCulture);
        }

        /// <summary>
        /// Sayý deðerini geçerli kültüre göre formatlar.
        /// </summary>
        public static string FormatNumber(double number)
        {
            return number.ToString("N0", CurrentCulture);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}