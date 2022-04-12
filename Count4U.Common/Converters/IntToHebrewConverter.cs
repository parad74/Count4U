using System;
using System.Globalization;
using System.Windows.Data;
using Count4U.Common.Helpers;

namespace Count4U.Common.Converters
{
    public class IntToHebrewConverter : IValueConverter
    {
      

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return UtilsConvert.HebrewInt((int)value);
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        } 
    }
}