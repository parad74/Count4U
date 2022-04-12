using System;
using System.Globalization;
using System.Windows.Data;

namespace Count4U.CustomControls.DataGrid
{
    public class IntToHebrewConverter : IValueConverter
    {
        public const string LTRMark = "\u200E";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return LTRMark + ((int)value).ToString();
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        } 
    }
}