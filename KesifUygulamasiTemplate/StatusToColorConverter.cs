using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace KesifUygulamasi.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CalibrationStatus status)
            {
                return status switch
                {
                    CalibrationStatus.Calibrating => Colors.Orange,
                    CalibrationStatus.Success => Colors.Green,
                    CalibrationStatus.Error => Colors.Red,
                    CalibrationStatus.Cancelled => Colors.Gray,
                    CalibrationStatus.Required => Colors.DarkOrange,
                    _ => Colors.LightGray
                };
            }
            
            return Colors.LightGray;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class DoubleToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double percent)
            {
                return percent / 100.0;
            }
            
            return 0.0;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
                
            if (value is bool boolValue)
                return boolValue;
                
            if (value is DateTime)
                return true;
                
            return false;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
                
            return true;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
