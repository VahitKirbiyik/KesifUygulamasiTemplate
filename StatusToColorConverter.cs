using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? Colors.Green : Colors.LightGray;
            }

            // Try enum-like handling by string if possible
            if (value != null)
            {
                var name = value.ToString();
                if (!string.IsNullOrEmpty(name))
                {
                    if (name.IndexOf("success", StringComparison.OrdinalIgnoreCase) >= 0)
                        return Colors.Green;
                    if (name.IndexOf("calibr", StringComparison.OrdinalIgnoreCase) >= 0)
                        return Colors.Orange;
                    if (name.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0)
                        return Colors.Red;
                    if (name.IndexOf("cancel", StringComparison.OrdinalIgnoreCase) >= 0)
                        return Colors.Gray;
                }
            }

            return Colors.LightGray;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
