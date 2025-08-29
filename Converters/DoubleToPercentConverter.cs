using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.Converters
{
    public class DoubleToPercentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double d)
                return (d * 100).ToString("F2") + "%";
            return "0%";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string s && double.TryParse(s.Replace("%", ""), out double result))
                return result / 100;
            return 0.0;
        }
    }
}
