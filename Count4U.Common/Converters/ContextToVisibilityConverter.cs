using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Count4U.Model;

namespace Count4U.Common.Converters
{
    public class ContextToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CBIContext val = (CBIContext)value;

            string contexts = parameter as String;
            if (String.IsNullOrEmpty(contexts) == true)  return Visibility.Visible;

            string[] visContext = contexts.Split(new char[] { '|' });
            return visContext.Any(r => r == val.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}