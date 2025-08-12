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
    /// T�m ViewModels i�in geli�mi� temel s�n�f
    /// MVVM pattern, navigasyon, loading ve hata y�netimi i�in optimize edilmi�tir
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
        /// Me�gul durumu - async i�lemler s�ras�nda true olur
        /// UI'da loading indicator'lar� i�in kullan�l�r
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Hata mesaj� - i�lem ba�ar�s�z oldu�unda kullan�c�ya g�sterilir
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Sayfa ba�l��� - her ViewModel i�in farkl� olabilir
        /// </summary>
        public virtual string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Loading durumunun tersi - UI elementlerini devre d��� b�rakmak i�in
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
        /// PropertyChanged olay�n� tetikler
        /// </summary>
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Property de�erini g�venli �ekilde ayarlar ve gerekirse PropertyChanged tetikler
        /// </summary>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            
            // IsBusy de�i�ti�inde IsNotBusy'yi de g�ncelle
            if (propertyName == nameof(IsBusy))
                OnPropertyChanged(nameof(IsNotBusy));
                
            return true;
        }
        #endregion

        #region Navigation Helpers
        /// <summary>
        /// Animasyonlu sayfa ge�i�i yapar
        /// </summary>
        protected async Task NavigateToAsync(Page page, bool animated = true)
        {
            if (NavigationService != null && page != null)
                await NavigationService.PushAsync(page, animated);
        }

        /// <summary>
        /// Shell tabanl� navigasyon
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
        /// Geri gitme i�lemi
        /// </summary>
        protected async Task GoBackAsync(bool animated = true)
        {
            if (NavigationService != null)
                await NavigationService.PopAsync(animated);
        }
        #endregion

        #region Safe Execution Methods
        /// <summary>
        /// Async i�lemleri g�venli �ekilde �al��t�r�r
        /// IsBusy ve ErrorMessage y�netimini otomatik yapar
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
        /// Return de�eri olan async i�lemleri g�venli �ekilde �al��t�r�r
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
        /// Hata mesaj�n� kullan�c�ya g�sterir (Toast ile)
        /// </summary>
        protected virtual async Task ShowErrorAsync(string message)
        {
            if (NotificationService != null && !string.IsNullOrWhiteSpace(message))
                await NotificationService.ShowErrorAsync(message);
        }

        /// <summary>
        /// Ba�ar� mesaj�n� kullan�c�ya g�sterir
        /// </summary>
        protected virtual async Task ShowSuccessAsync(string message)
        {
            if (NotificationService != null && !string.IsNullOrWhiteSpace(message))
                await NotificationService.ShowSuccessAsync(message);
        }

        /// <summary>
        /// Bilgi mesaj�n� g�sterir
        /// </summary>
        protected virtual async Task ShowInfoAsync(string message)
        {
            if (NotificationService != null && !string.IsNullOrWhiteSpace(message))
                await NotificationService.ShowInfoAsync(message);
        }

        /// <summary>
        /// Uyar� mesaj�n� g�sterir
        /// </summary>
        protected virtual async Task ShowWarningAsync(string message)
        {
            if (NotificationService != null && !string.IsNullOrWhiteSpace(message))
                await NotificationService.ShowWarningAsync(message);
        }

        /// <summary>
        /// Konfirmasyon dialog'u g�sterir
        /// </summary>
        protected virtual async Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Evet", string cancel = "�ptal")
        {
            if (NotificationService != null)
                return await NotificationService.ShowConfirmationAsync(title, message, accept, cancel);
            return false;
        }
        #endregion

        #region Error Handling
        /// <summary>
        /// Exception'lar� kullan�c� dostu mesajlara �evirir
        /// </summary>
        private string GetUserFriendlyErrorMessage(Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException => "Bu i�lem i�in yetkiniz bulunmuyor.",
                TimeoutException => "��lem zaman a��m�na u�rad�. L�tfen tekrar deneyin.",
                System.Net.NetworkInformation.NetworkInformationException => "A� ba�lant�s� sorunu. �nternet ba�lant�n�z� kontrol edin.",
                TaskCanceledException => "��lem iptal edildi.",
                ArgumentNullException => "Gerekli bilgiler eksik.",
                InvalidOperationException => "Bu i�lem �u anda ger�ekle�tirilemez.",
                _ => $"Beklenmeyen bir hata olu�tu: {ex.Message}"
            };
        }
        #endregion

        #region Lifecycle
        /// <summary>
        /// ViewModel initialize edildi�inde �a�r�l�r
        /// Derived class'lar bu metodu override edebilir
        /// </summary>
        public virtual async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// ViewModel dispose edildi�inde �a�r�l�r
        /// Kaynaklar� temizlemek i�in kullan�l�r
        /// </summary>
        public virtual void Cleanup()
        {
            // Event handler'lar� temizle, timer'lar� durdur vs.
        }
        #endregion
    }
}
