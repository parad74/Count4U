using System;
using System.Globalization;
using System.Windows.Data;

namespace Count4U.Common.Converters
{
	public class DateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			DateTime date = (DateTime)value;
			return date.ToString("d", culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string strValue = value.ToString();
			DateTime resultDateTime;
			if (DateTime.TryParse(strValue, culture, DateTimeStyles.None, out resultDateTime))
			{
				return resultDateTime;
			}

			return value;
		}
	}
}

//см
//  <converter:ErrorConverter x:Key="errorConverter"/>
//<converter:DateConverter x:Key="dateConverter"/>
//<converter:StringToNullableNumberConverter x:Key="stringToNullableNumberConverter" />
//Пример использования в Commanding_Desktop