using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Count4U.Common.Converters
{
    class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;

            if (String.IsNullOrEmpty(status) == false) 
            {
                switch (status)
                {
                    case "StatusIturCode1":
                        return Brushes.LightYellow;

                    case "StatusIturCode2":
                        return Brushes.LightGreen;

                    case "StatusIturCode3":
                        return Brushes.LightBlue;

                    default:
						return Brushes.LightCyan;
                       // throw new ArgumentOutOfRangeException();
                }
            }
	            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}