using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasi.ViewModels.Base
{
    /// <summary>
    /// T�m ViewModels i�in temel s�n�f
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _errorMessage;

        /// <summary>
        /// Me�gul durumu, genellikle i�lem devam ederken true olarak ayarlan�r
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Hata mesaj�, i�lem ba�ar�s�z oldu�unda ayarlan�r
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Property de�i�ikli�i olay�
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// PropertyChanged olay�n� tetikler
        /// </summary>
        /// <param name="propertyName">De�i�en property ad�</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Property'yi ayarlar ve de�i�iklik oldu�unda PropertyChanged olay�n� tetikler
        /// </summary>
        /// <typeparam name="T">Property tipi</typeparam>
        /// <param name="storage">Referans olarak depolama alan�</param>
        /// <param name="value">Ayarlanacak yeni de�er</param>
        /// <param name="propertyName">Property ad�</param>
        /// <returns>De�er de�i�tiyse true, aksi halde false</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected NavigationService NavigationService { get; }

        public BaseViewModel()
        {
            NavigationService = App.Current?.Handler?.MauiContext?.Services?.GetService<NavigationService>();
        }

        /// <summary>
        /// ViewModel'den animasyonlu sayfa ge�i�i i�in yard�mc� metot.
        /// </summary>
        protected async Task NavigateToAsync(Page page, bool animated = true)
        {
            if (NavigationService != null)
                await NavigationService.PushAsync(page, animated);
        }
    }
}
