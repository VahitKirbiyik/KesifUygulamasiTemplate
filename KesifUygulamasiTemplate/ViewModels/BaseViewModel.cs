using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using KesifUygulamasiTemplate.Services;
using Microsoft.Maui.Controls;
using Microsoft.AppCenter.Crashes;

namespace KesifUygulamasiTemplate.ViewModels
{
    /// <summary>
    /// Tüm ViewModels için geliþmiþ temel sýnýf
    /// MVVM pattern, navigasyon, loading ve hata yönetimi için optimize edilmiþtir
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Private Fields
        private bool _isBusy;
        private string _errorMessage;
        private string _title;
        #endregion

        #region Public Properties
        /// <summary>
        /// Meþgul durumu - async iþlemler sýrasýnda true olur
        /// UI'da loading indicator'larý için kullanýlýr
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Hata mesajý - iþlem baþarýsýz olduðunda kullanýcýya gösterilir
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Sayfa baþlýðý - her ViewModel için farklý olabilir
        /// </summary>
        public virtual string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Loading durumunun tersi - UI elementlerini devre dýþý býrakmak için
        /// </summary>
        public bool IsNotBusy => !IsBusy;
        #endregion

        #region Services
        protected NavigationService NavigationService { get; }
        protected NotificationService NotificationService { get; }
        private readonly AppCenterAnalyticsService _analyticsService;
        #endregion

        #region Constructor
        public BaseViewModel()
        {
            var serviceProvider = Application.Current?.Handler?.MauiContext?.Services;
            NavigationService = serviceProvider?.GetService<NavigationService>();
            NotificationService = serviceProvider?.GetService<NotificationService>();
            _analyticsService = serviceProvider?.GetService<AppCenterAnalyticsService>();
        }
        #endregion

        #region Property Changed Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// PropertyChanged olayýný tetikler
        /// </summary>
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Property deðerini güvenli þekilde ayarlar ve gerekirse PropertyChanged tetikler
        /// </summary>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            
            // IsBusy deðiþtiðinde IsNotBusy'yi de güncelle
            if (propertyName == nameof(IsBusy))
                OnPropertyChanged(nameof(IsNotBusy));
                
            return true;
        }
        #endregion

        #region Navigation Helpers
        /// <summary>
        /// Animasyonlu sayfa geçiþi yapar
        /// </summary>
        protected async Task NavigateToAsync(Page page, bool animated = true)
        {
            if (NavigationService != null && page != null)
                await NavigationService.PushAsync(page, animated);
        }

        /// <summary>
        /// Shell tabanlý navigasyon
        /// </summary>
        protected async Task NavigateToAsync(string route, bool animated = true)
        {
            if (NavigationService != null && !string.IsNullOrWhiteSpace(route))
                await NavigationService.GoToAsync(route, animated);
        }

        /// <summary>
        /// Parametreli Shell navigasyon
        /// </summary>
        protected async Task NavigateToAsync(string route, IDictionary<string, object> parameters, bool animated = true)
        {
            if (NavigationService != null && !string.IsNullOrWhiteSpace(route))
                await NavigationService.GoToAsync(route, parameters, animated);
        }

        /// <summary>
        /// Geri gitme iþlemi
        /// </summary>
        protected async Task GoBackAsync(bool animated = true)
        {
            if (NavigationService != null)
                await NavigationService.PopAsync(animated);
        }
        #endregion

        #region Safe Execution Methods
        /// <summary>
        /// Async iþlemleri güvenli þekilde çalýþtýrýr
        /// IsBusy ve ErrorMessage yönetimini otomatik yapar
        /// </summary>
        protected async Task ExecuteAsync(Func<Task> operation, string loadingMessage = null)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                if (!string.IsNullOrWhiteSpace(loadingMessage))
                    await NotificationService?.ShowLoadingAsync(loadingMessage);

                await operation();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetUserFriendlyErrorMessage(ex);
                await ShowErrorAsync(ErrorMessage);
                _analyticsService?.TrackError(ex); // AppCenter Crashes
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Return deðeri olan async iþlemleri güvenli þekilde çalýþtýrýr
        /// </summary>
        protected async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, T defaultValue = default)
        {
            if (IsBusy)
                return defaultValue;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                return await operation();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetUserFriendlyErrorMessage(ex);
                await ShowErrorAsync(ErrorMessage);
                _analyticsService?.TrackError(ex); // AppCenter Crashes
                return defaultValue;
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region Notification Helpers
        /// <summary>
        /// Hata mesajýný kullanýcýya gösterir (Toast ile)
        /// </summary>
        protected virtual async Task ShowErrorAsync(string message)
        {
            if (NotificationService != null && !string.IsNullOrWhiteSpace(message))
                await NotificationService.ShowErrorAsync(message);
        }

        /// <summary>
        /// Baþarý mesajýný kullanýcýya gösterir
        /// </summary>
        protected virtual async Task ShowSuccessAsync(string message)
        {
            if (NotificationService != null && !string.IsNullOrWhiteSpace(message))
                await NotificationService.ShowSuccessAsync(message);
        }

        /// <summary>
        /// Bilgi mesajýný gösterir
        /// </summary>
        protected virtual async Task ShowInfoAsync(string message)
        {
            if (NotificationService != null && !string.IsNullOrWhiteSpace(message))
                await NotificationService.ShowInfoAsync(message);
        }

        /// <summary>
        /// Uyarý mesajýný gösterir
        /// </summary>
        protected virtual async Task ShowWarningAsync(string message)
        {
            if (NotificationService != null && !string.IsNullOrWhiteSpace(message))
                await NotificationService.ShowWarningAsync(message);
        }

        /// <summary>
        /// Konfirmasyon dialog'u gösterir
        /// </summary>
        protected virtual async Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Evet", string cancel = "Ýptal")
        {
            if (NotificationService != null)
                return await NotificationService.ShowConfirmationAsync(title, message, accept, cancel);
            return false;
        }
        #endregion

        #region Error Handling
        /// <summary>
        /// Exception'larý kullanýcý dostu mesajlara çevirir
        /// </summary>
        private string GetUserFriendlyErrorMessage(Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException => "Bu iþlem için yetkiniz bulunmuyor.",
                TimeoutException => "Ýþlem zaman aþýmýna uðradý. Lütfen tekrar deneyin.",
                System.Net.NetworkInformation.NetworkInformationException => "Að baðlantýsý sorunu. Ýnternet baðlantýnýzý kontrol edin.",
                TaskCanceledException => "Ýþlem iptal edildi.",
                ArgumentNullException => "Gerekli bilgiler eksik.",
                InvalidOperationException => "Bu iþlem þu anda gerçekleþtirilemez.",
                _ => $"Beklenmeyen bir hata oluþtu: {ex.Message}"
            };
        }
        #endregion

        #region Lifecycle
        /// <summary>
        /// ViewModel initialize edildiðinde çaðrýlýr
        /// Derived class'lar bu metodu override edebilir
        /// </summary>
        public virtual async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// ViewModel dispose edildiðinde çaðrýlýr
        /// Kaynaklarý temizlemek için kullanýlýr
        /// </summary>
        public virtual void Cleanup()
        {
            // Event handler'larý temizle, timer'larý durdur vs.
        }
        #endregion
    }
}
