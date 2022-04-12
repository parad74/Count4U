using System;
using System.Globalization;
using System.Windows.Data;

namespace Count4U.Common.Converters
{
	/// <summary>
	/// Пример написание конвертора
	/// </summary>
	public class StringToNullableNumberConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string stringValue = value as string;
			if (stringValue != null)
			{
				if (targetType == typeof(int?))
				{
					int result;
					if (int.TryParse(stringValue, out result))
						return result;

					return null;
				}

				if (targetType == typeof(decimal?))
				{
					decimal result;
					if (decimal.TryParse(stringValue, out result))
						return result;

					return null;
				}
			}

			return value;
		}
	}

}


//Пример использования в Commanding_Desktop см
//<TextBox AutomationProperties.AutomationId="ShippingTextBox" Grid.Row="4" Grid.Column="1" Text="{Binding Shipping, Converter={StaticResource stringToNullableNumberConverter}}" />
