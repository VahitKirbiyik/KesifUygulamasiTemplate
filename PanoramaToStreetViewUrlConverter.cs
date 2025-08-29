using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasiTemplate.Converters
{
    public class PanoramaToStreetViewUrlConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is StreetViewPanorama panorama)
            {
                // Google Street View Embed URL
                return $"https://www.google.com/maps/embed/v1/streetview?key=YOUR_API_KEY&location={panorama.Latitude},{panorama.Longitude}&heading={panorama.Heading}&pitch={panorama.Pitch}";
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
