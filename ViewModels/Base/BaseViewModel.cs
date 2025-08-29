using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasiTemplate.ViewModels.Base
{
    /// <summary>
    /// Tüm ViewModels için temel sınıf
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _errorMessage = string.Empty;
        private string _title = string.Empty;

        /// <summary>
        /// Sayfa başlığı
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Meşgul durumu, genellikle işlem devam ederken true olarak ayarlanır
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Meşgul değil durumu (IsBusy'nin tersi)
        /// </summary>
        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// Hata mesajı, işlem başarısız olduğunda ayarlanır
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Property değişikliği olayı
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// PropertyChanged olayını tetikler
        /// </summary>
        /// <param name="propertyName">Değişen property adı</param>
        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Property'yi ayarlar ve değişiklik olduğunda PropertyChanged olayını tetikler
        /// </summary>
        /// <typeparam name="T">Property tipi</typeparam>
        /// <param name="storage">Referans olarak depolama alanı</param>
        /// <param name="value">Ayarlanacak yeni değer</param>
        /// <param name="propertyName">Property adı</param>
        /// <returns>Değer değiştiyse true, aksi halde false</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected NavigationService? NavigationService { get; }

        public BaseViewModel()
        {
            NavigationService = App.Current?.Handler?.MauiContext?.Services?.GetService<NavigationService>();
        }

        /// <summary>
        /// ViewModel'den animasyonlu sayfa geçişi için yardımcı metot.
        /// </summary>
        protected async Task NavigateToAsync(Page page, bool animated = true)
        {
            if (NavigationService != null)
                await NavigationService.PushAsync(page, animated);
        }

        /// <summary>
        /// Async komut çalıştırma yardımcı metodu
        /// </summary>
        public async Task ExecuteAsync(Func<Task> action)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                await action();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Async komut çalıştırma yardımcı metodu (mesaj ile)
        /// </summary>
        public async Task ExecuteAsync(Func<Task> action, string loadingMessage)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                // Burada loadingMessage kullanılabilir (örneğin bir loading indicator göstermek için)
                await action();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Başarı mesajı gösterme yardımcı metodu
        /// </summary>
        public async Task ShowSuccessAsync(string message)
        {
            // Implementation would depend on your UI framework
            await Task.CompletedTask;
        }

        /// <summary>
        /// Bilgi mesajı gösterme yardımcı metodu
        /// </summary>
        public async Task ShowInfoAsync(string message)
        {
            // Implementation would depend on your UI framework
            await Task.CompletedTask;
        }

        /// <summary>
        /// Uyarı mesajı gösterme yardımcı metodu
        /// </summary>
        public async Task ShowWarningAsync(string message)
        {
            // Implementation would depend on your UI framework
            await Task.CompletedTask;
        }

        /// <summary>
        /// Hata mesajı gösterme yardımcı metodu
        /// </summary>
        public async Task ShowErrorAsync(string message)
        {
            // Implementation would depend on your UI framework
            await Task.CompletedTask;
        }
    }
}
