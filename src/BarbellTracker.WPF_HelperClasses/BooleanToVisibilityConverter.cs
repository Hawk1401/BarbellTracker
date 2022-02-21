using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BarbellTracker.WPF_HelperClasses
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // YAGNI
            throw new NotImplementedException();
        }
    }
}
