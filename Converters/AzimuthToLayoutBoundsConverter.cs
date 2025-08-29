using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.Converters
{
    // Azimuth (0-360) değerini ekran koordinatına çevirir (örnek, overlay için)
    public class AzimuthToLayoutBoundsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double azimuth)
            {
                // Basit örnek: 0-360 dereceyi ekranın yatay eksenine yay
                double x = azimuth / 360.0; // 0.0 - 1.0
                return new Microsoft.Maui.Graphics.Rect(x, 0.5, 48, 48); // Yarı yükseklik, sabit boyut
            }
            return new Microsoft.Maui.Graphics.Rect(0.5, 0.5, 48, 48);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
