using System;
using System.Text.RegularExpressions;

namespace Count4U.Model.Count4U.Validate
{
    public class CommonValidate
    {
		public static int IsNullOrEmptyValid(string value)
		{
			if (String.IsNullOrWhiteSpace(value) == true)
			{
				return (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
			else 
			{
				return (int)ConvertDataErrorCodeEnum.NoError;
			}
		}

		public static int IsEmailValid(string email)
		{
			if (String.IsNullOrEmpty(email) == false)
			{
				const string pattern =
						 @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
				  + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
				  + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
				  + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";


				if (Regex.IsMatch(email, pattern) == false)
				{
					return (int)ConvertDataErrorCodeEnum.InvalidValue;
				}
			}
			return (int)ConvertDataErrorCodeEnum.NoError;
		}

		public static int IsPhoneFaxValid(string phoneFax)
		{
			if (String.IsNullOrEmpty(phoneFax) == false)
			{
				const string patern = @"^\+?[0-9-#\s]{5,20}$";

				if (Regex.IsMatch(phoneFax, patern) == false)
				{
					return (int)ConvertDataErrorCodeEnum.InvalidValue;
				}
			}	
			return (int)ConvertDataErrorCodeEnum.NoError;
		}

        public static bool IsOkAsDouble(string value)
        {
            if (String.IsNullOrEmpty(value))
                return true;

            double res;
            return Double.TryParse(value, out res);
        }

		public static bool IsOkAsInt(string value)
		{
			if (String.IsNullOrEmpty(value))
				return true;

			int res;
			return Int32.TryParse(value, out res);
		}

        public static int DigitsAfterDecimalPoint(string number)
        {
            if (String.IsNullOrWhiteSpace(number))
                return 0;

            if (number.Contains("."))
            {
                int length = number.Substring(number.IndexOf(".") + 1).Length;
                return length;
            }

            if (number.Contains(","))
            {
                int length = number.Substring(number.IndexOf(",") + 1).Length;
                return length;
            }

            return 0;
        }
	
    }
}