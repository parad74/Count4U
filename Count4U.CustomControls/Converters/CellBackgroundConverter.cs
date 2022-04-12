using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Count4U.CustomControls.Converters
{
    public class CellBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush = parameter as Brush;
         //   return (int)value == 1 ? new SolidColorBrush(Color.FromRgb(234, 254, 173)) : Brushes.Transparent;
            return (int)value == 1 ? brush : Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}