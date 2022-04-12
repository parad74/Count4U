using System;
using System.Globalization;
using System.Windows.Data;
using Count4U.Model;

namespace Count4U.Common.Converters
{
    public class CBIContextToBoolConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CBIContext context = (CBIContext) value;
            return context == CBIContext.CreateInventor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}