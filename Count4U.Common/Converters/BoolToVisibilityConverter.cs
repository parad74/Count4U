using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Count4U.Common.Converters
{
    public class BoolToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;

            if ((parameter != null)
                && ((String)parameter == "Invert"))
            {
                return val ? Visibility.Collapsed : Visibility.Visible;
            }

            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}