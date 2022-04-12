using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Count4U.Modules.Audit.Views.Converter
{
    public class SearchValueToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string searchValue = value as String;
            string param = parameter as String;

            bool isForTextBox = String.IsNullOrEmpty(param);

            if (isForTextBox)
            {
                return searchValue == "Date" ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return searchValue == "Date" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}