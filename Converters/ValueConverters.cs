using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace KesifUygulamasiTemplate.Converters
{
    /// <summary>
    /// Bir string'in bo� olup olmad���n� kontrol eder. String bo� de�ilse true, bo�sa false d�ner.
    /// </summary>
    public class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Boolean de�eri tersine �evirir. True ise false, false ise true d�ner.
    /// </summary>
    public class InvertedBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return false;
        }
    }

    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return 0;
            if (values[0] is double value1 && values[1] is double value2)
                return value1 * value2;
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NegativeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
                return -doubleValue;
            return 0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
                return -doubleValue;
            return 0;
        }
    }
}
