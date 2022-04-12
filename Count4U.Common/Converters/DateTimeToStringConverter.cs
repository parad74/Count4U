using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Count4U.Common.Converters
{
	public class DateTimeToStringConverter : IValueConverter
	{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((DateTime)value).ToString("dd/MM/yyyy  HH:mm:ss");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}
	}
}

//см
//  <converter:ErrorConverter x:Key="errorConverter"/>
//<converter:DateConverter x:Key="dateConverter"/>
//<converter:StringToNullableNumberConverter x:Key="stringToNullableNumberConverter" />
//Пример использования в Commanding_Desktop