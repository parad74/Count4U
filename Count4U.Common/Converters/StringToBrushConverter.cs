using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using Count4U.Common.Helpers;

namespace Count4U.Common.Converters
{
    public class StringToBrushConverter : IValueConverter
    {       
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var background = value as string;
			if (String.IsNullOrEmpty(background) == true)
			{
			    background = Common.Constants.DefaultColors.EmptyLocationColor();
			}

            return new SolidColorBrush(ColorParser.StringToColor(background));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}