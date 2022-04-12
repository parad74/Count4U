using System;
using System.ComponentModel;

namespace Count4U.Model.Common
{
    public static class AndroidUtils
    {
		//the difference, measured in milliseconds, between the current time and midnight, January 1, 1970 UTC.
		public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0,
												DateTimeKind.Utc);

		public static DateTime ConvertFromAndroidTime(this DateTime androidTime)
		{
			TimeSpan ticks = TimeSpan.FromMilliseconds(androidTime.Ticks);
			DateTime netDateTime = UnixEpoch + ticks;
			DateTime dt = new DateTime(netDateTime.Year, netDateTime.Month, netDateTime.Day, netDateTime.Hour, netDateTime.Minute, netDateTime.Second);
			return dt.ToLocalTime();
		}

		public static string ConvertDateTimeToAndroid(this DateTime count4uDateTime)
		{
			TimeSpan toAndroid = count4uDateTime - UnixEpoch;
			Int64 totalMillisecond = Convert.ToInt64(toAndroid.TotalMilliseconds);
			if (totalMillisecond == 0)
			{
				return "";
			}
			return totalMillisecond.ToString();
			//long ticks = toAndroid.Ticks;
			//if (ticks == 0) return "";
			//return ticks.ToString();
		}

		public static T? GetNullableValue<T>(this string stringValue) where T : struct
		{
			T? item;

			try
			{
				item = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(stringValue);
			}
			catch (Exception)
			{
				item = null;
			}

			return item;
		}

		public static string GetValueOrDefaultString(this string value)
		{
			return value ?? "";
		}


		public static DateTime GetMinDateTime(this DateTime value)
		{
			return UnixEpoch;
		}

		public static string GetValueOrDefaultString<T>(this T? value) where T : struct
		{
			return value.HasValue ? value.Value.ToString() : "";
		}

		public static string GetValueOrDefaultString(this string value, string equalEmpty)
		{
			if (string.IsNullOrWhiteSpace(value) == true) return "";
			if (value.Trim().ToLower() == equalEmpty.Trim().ToLower()) return "";
			return value;

		}
		public static string CutLength(this string stringValue, int MaxLength)
		{
			if (string.IsNullOrWhiteSpace(stringValue) == true) return "";
			stringValue = stringValue.Trim();
			if (stringValue.Length <= MaxLength) return stringValue;
			else return stringValue.Substring(0, MaxLength);
		}
    }
}

	
