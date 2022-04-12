using System;
using System.Globalization;
using System.Windows.Data;
using Count4U.Common.Helpers;

namespace Count4U.Common.Converters
{
    public class DoubleToHebrewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return UtilsConvert.HebrewDouble((double) value);
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}