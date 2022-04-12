using System;
using System.Globalization;
using System.Windows.Data;

namespace Count4U.CustomControls.DataGrid
{
    public class DoubleToHebrewConverter : IValueConverter
    {
         public const string LTRMark = "\u200E";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                double d = (double) value;

                string doubleFormatted = System.Convert.ToDecimal(String.Format("{0:0.00}", d)).ToString();

                return LTRMark + doubleFormatted;
                
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}