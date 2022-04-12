using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Count4U.Model.Interface;
using Count4U.Model.Main;

namespace Count4U.Model.Count4U.Validate
{
	public static class CustomerValidate
	{
        public static string CodeValidate(string code)
        {
            if (String.IsNullOrWhiteSpace(code))
                return ValidateMessage.Customer.CodeEmpty;

            if (code.Length < 3 || code.Length > 10)
                return ValidateMessage.Customer.CodeLength;

            if (!Regex.IsMatch(code, @"^[-_\d\w]+$"))
                return ValidateMessage.Customer.CodeFormat;

            return String.Empty;
        }

        public static string ContactPersonValidate(string contactPerson)
        {
            if (String.IsNullOrEmpty(contactPerson))
                return String.Empty;

            if (!Regex.IsMatch(contactPerson, @"^[-_,""=.&\d\w\s]+$"))
                return ValidateMessage.Customer.CodeFormat;

            return String.Empty;
        }

        public static string NameValidate(string name)
        {
			int bit = CommonValidate.IsNullOrEmptyValid(name);
			if (bit != 0) return ValidateMessage.IsEmpty.Name;		//"Name can not be Empty";
            return String.Empty;
        }

        public static string MailValidate(string mail)
        {
			int bit = CommonValidate.IsNullOrEmptyValid(mail);
			if (bit != 0) return String.Empty;

			bit = CommonValidate.IsEmailValid(mail);
			if (bit != 0) return ValidateMessage.InvalidValue.Email; //"Email is not Valid";
  
			return String.Empty;
        }

        public static string PhoneValidate(string phone)
        {
  			int bit = CommonValidate.IsNullOrEmptyValid(phone);
			if (bit != 0) return String.Empty;

			bit = CommonValidate.IsPhoneFaxValid(phone);
			if (bit != 0) return ValidateMessage.InvalidValue.Phone; //"Phone is not Valid";

			return String.Empty;
        }

        public static string FaxValidate(string fax)
        {
			int bit = CommonValidate.IsNullOrEmptyValid(fax);
			if (bit != 0) return String.Empty;

			bit = CommonValidate.IsPhoneFaxValid(fax);
			if (bit != 0) return ValidateMessage.InvalidValue.Fax; //"Fax is not Valid";

			return String.Empty;
         }

		#region message
		//public static string ConvertDataErrorCode2ErrorMessage(ConvertDataErrorCodeEnum status)
		//{
		//    switch (status)
		//    {
		//        case ConvertDataErrorCodeEnum.InvalidValue:
		//            return ValidateMessage.InvalidValue.Code;
		//        case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
		//            return ValidateMessage.FKCodeIsEmpty.General;
		//        case ConvertDataErrorCodeEnum.SameCodeExist:
		//            return ValidateMessage.SameCodeExist.General;
		//        case ConvertDataErrorCodeEnum.CodeIsEmpty:
		//            return ValidateMessage.IsEmpty.Code;
		//    }
		//    return "";
		//}

		//public static string ConvertDataErrorCode2WarningMessage(ConvertDataErrorCodeEnum status)
		//{
		//    switch (status)
		//    {
		//        case ConvertDataErrorCodeEnum.InvalidValue:
		//            return ValidateMessage.InvalidValue.General;
		//        case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
		//            return ValidateMessage.FKCodeIsEmpty.General;
		//        case ConvertDataErrorCodeEnum.SameCodeExist:
		//            return ValidateMessage.SameCodeExist.General;
		//        case ConvertDataErrorCodeEnum.CodeIsEmpty:
		//            return ValidateMessage.IsEmpty.Code;
		//    }
		//    return "";
		//}
		#endregion

		#region Validate

		public static int ValidateError(this Customer entity, CustomerString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			entity.Code = "";

			//Code
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.Code) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				entity.Code = inValue.Code.Trim(" ".ToCharArray());
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateWarning(this Customer entity, CustomerString inValue, DateTimeFormatInfo dtfi)
		{
			int ret = 0;
			//int invalidValueBit = 0;
			//int fkCodeIsEmpty = 0;
			//int codeIsEmpty = 0;
			//entity.Name = "";
			//entity.Description = "";
			//entity.BackgroundColor = "";

			//try
			//{
			//    entity.Name = inValue.Name.Trim(" ".ToCharArray());
			//    entity.Description = inValue.Description.Trim(" ".ToCharArray());
			//}
			//catch
			//{
			//    invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			//}

			//try
			//{
			//    string bg = inValue.BackgroundColor.Trim(" ".ToCharArray());
			//    string[] backgroundColor = bg.Split(':');
			//    int bg1 = Int32.Parse(backgroundColor[0].Trim());
			//    int bg2 = Int32.Parse(backgroundColor[1].Trim());
			//    int bg3 = Int32.Parse(backgroundColor[2].Trim());
			//    bg = bg1 + ", " + bg2 + ", " + bg3;
			//    entity.BackgroundColor = bg;
			//}
			//catch
			//{
			//    invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			//}

			//ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int Validate(this Customer entity, CustomerString inValue, DateTimeFormatInfo dtfi)
		{
			int ret = 0;
			int validateError = 0;
			int validateWarning = 0;

			validateError = ValidateError(entity, inValue, dtfi);
			if (validateError == 0)
			{
				validateWarning = ValidateWarning(entity, inValue, dtfi);
			}

			ret = ret + validateError + validateWarning;
			return ret;
		}
		#endregion

    }
}
