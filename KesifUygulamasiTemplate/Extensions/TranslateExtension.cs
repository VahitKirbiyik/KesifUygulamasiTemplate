using System;
using KesifUygulamasiTemplate.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace KesifUygulamasiTemplate.Extensions
{
    [ContentProperty("Key")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Key { get; set; }
        public string StringFormat { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(Key))
                return string.Empty;

            var translation = LocalizationService.Instance.GetString(Key);

            if (!string.IsNullOrWhiteSpace(StringFormat))
                return string.Format(StringFormat, translation);

            return translation;
        }
    }

    // Dinamik olarak dil deðiþiminde otomatik güncellenen çeviri için
    public class TranslateBindingExtension : IMarkupExtension
    {
        public string Key { get; set; }
        public string StringFormat { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(Key))
                return string.Empty;

            var binding = new Binding(
                path: $"[{Key}]",
                mode: BindingMode.OneWay,
                source: LocalizationService.Instance);

            if (!string.IsNullOrWhiteSpace(StringFormat))
                binding.StringFormat = StringFormat;

            return binding;
        }
    }
}