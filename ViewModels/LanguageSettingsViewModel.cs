using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using KesifUygulamasiTemplate.Services;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.ViewModels
{
    public class LanguageSettingsViewModel : BaseViewModel
    {
        private List<LanguageModel> _availableLanguages;
        private LanguageModel _selectedLanguage;

        public List<LanguageModel> AvailableLanguages
        {
            get => _availableLanguages;
            set => SetProperty(ref _availableLanguages, value);
        }

        public LanguageModel SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (SetProperty(ref _selectedLanguage, value) && value != null)
                {
                    ChangeLanguage(value.CultureCode);
                }
            }
        }

        public ICommand ChangeLanguageCommand { get; }

        public LanguageSettingsViewModel()
        {
            ChangeLanguageCommand = new Command<string>(ChangeLanguage);
            InitializeLanguages();
        }

        private void InitializeLanguages()
        {
            AvailableLanguages = new List<LanguageModel>
            {
                new LanguageModel { DisplayName = Resources.Strings.AppResources.LanguageTurkish, CultureCode = "tr" },
                new LanguageModel { DisplayName = Resources.Strings.AppResources.LanguageEnglish, CultureCode = "en" }
            };
            string currentCulture = LocalizationService.CurrentCulture.TwoLetterISOLanguageName;
            SelectedLanguage = AvailableLanguages.Find(l => l.CultureCode == currentCulture);
        }

        private void ChangeLanguage(string cultureCode)
        {
            if (string.IsNullOrEmpty(cultureCode))
                return;
            try
            {
                var newCulture = new CultureInfo(cultureCode);
                LocalizationService.CurrentCulture = newCulture;
                InitializeLanguages();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Dil deðiþtirme sýrasýnda hata oluþtu: {ex.Message}";
            }
        }
    }

    public class LanguageModel
    {
        public string DisplayName { get; set; }
        public string CultureCode { get; set; }
    }
}