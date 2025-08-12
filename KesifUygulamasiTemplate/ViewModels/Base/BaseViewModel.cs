using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KesifUygulamasiTemplate.Services;

namespace KesifUygulamasi.ViewModels.Base
{
    /// <summary>
    /// Tüm ViewModels için temel sýnýf
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _errorMessage;

        /// <summary>
        /// Meþgul durumu, genellikle iþlem devam ederken true olarak ayarlanýr
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Hata mesajý, iþlem baþarýsýz olduðunda ayarlanýr
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Property deðiþikliði olayý
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// PropertyChanged olayýný tetikler
        /// </summary>
        /// <param name="propertyName">Deðiþen property adý</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Property'yi ayarlar ve deðiþiklik olduðunda PropertyChanged olayýný tetikler
        /// </summary>
        /// <typeparam name="T">Property tipi</typeparam>
        /// <param name="storage">Referans olarak depolama alaný</param>
        /// <param name="value">Ayarlanacak yeni deðer</param>
        /// <param name="propertyName">Property adý</param>
        /// <returns>Deðer deðiþtiyse true, aksi halde false</returns>
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
        /// ViewModel'den animasyonlu sayfa geçiþi için yardýmcý metot.
        /// </summary>
        protected async Task NavigateToAsync(Page page, bool animated = true)
        {
            if (NavigationService != null)
                await NavigationService.PushAsync(page, animated);
        }
    }
}
