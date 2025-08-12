using System;
using KesifUygulamasiTemplate.Services;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace KesifUygulamasiTemplate
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            LocalizationService.Instance.PropertyChanged += LocalizationService_PropertyChanged;
        }

        private void LocalizationService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LocalizationService.CurrentCulture) || string.IsNullOrEmpty(e.PropertyName))
            {
                OnPropertyChanged(nameof(Title));
            }
        }
    }
}
