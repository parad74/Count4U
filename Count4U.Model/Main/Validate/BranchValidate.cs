using System;
using System.Text.RegularExpressions;

namespace Count4U.Model.Count4U.Validate
{
    public class BranchValidate
    {
        public static string CodeValidate(string code)
        {
            if (String.IsNullOrWhiteSpace(code))
                return ValidateMessage.Branch.CodeEmpty;

            if (code.Length < 3 || code.Length > 20)
                return ValidateMessage.Branch.CodeLength;

            if (!Regex.IsMatch(code, @"^[-_\d\w]+$"))
                return ValidateMessage.Branch.CodeFormat;

            return String.Empty;
        }

        public static string CodeLocalValidate(string codeLocal)
        {
            if (String.IsNullOrWhiteSpace(codeLocal))
                return ValidateMessage.Branch.CodeLocalEmpty;

            if (!Regex.IsMatch(codeLocal, @"^[-_\d\w]+$"))
                return ValidateMessage.Branch.CodeLocalFormat;

            return String.Empty;
        }

        public static string CodeErpValidate(string codeErp)
        {
            if (String.IsNullOrWhiteSpace(codeErp) == true)
                return ValidateMessage.Branch.CodeErpEmpty;
			//if (!Regex.IsMatch(_code, "^[A-Za-z0-9]+$"))
			//if (Regex.IsMatch(codeErp, @"^[-_A-Za-z0-9]+$") == false)
			//   return ValidateMessage.Branch.CodeErpFormat;

            return String.Empty;
        }

        public static string ContactPersonValidate(string contactPerson)
        {
            if (String.IsNullOrEmpty(contactPerson))
                return String.Empty;

            if (!Regex.IsMatch(contactPerson, @"^[-_,""=.&\d\w\s]+$"))
                return ValidateMessage.Branch.CodeFormat;

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


		public static string ConvertDataErrorCode2PrefixErrorString(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.General;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.General;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.General;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.General;
			}
			return "";
		}

		public static string ConvertDataErrorCode2ErrorMessage(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.General;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.General;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.General;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.General;
			}
			return "";
		}
    }
}