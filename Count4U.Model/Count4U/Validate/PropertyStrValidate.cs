using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U.Validate
{
	public static class PropertyStrValidate
	{
		#region message
		public static string ConvertDataErrorCode2ErrorMessage(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.Code;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.General;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.General;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.Code;
			}
			return "";
		}

		public static string ConvertDataErrorCode2WarningMessage(ConvertDataErrorCodeEnum status)
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
					return ValidateMessage.IsEmpty.Code;
			}
			return "";
		}
		#endregion

		#region Validate

		public static int ValidateError(this PropertyStr entity, PropertyStr inValue)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			entity.PropertyStrCode = "";
			entity.DomainObject = "Unknown";
			entity.TypeCode = "";

			//Code
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.PropertyStrCode) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				entity.PropertyStrCode = inValue.PropertyStrCode.Trim(" ".ToCharArray());
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}


		public static int ValidateProfileError(this PropertyStr entity, PropertyStr inValue)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			entity.PropertyStrCode = "";
			entity.DomainObject = "Unknown";
			entity.TypeCode = "";

			//Code
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			//else if (string.IsNullOrWhiteSpace(inValue.PropertyStrCode) == true)
			//{
			//	codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			//}
			else
			{
				entity.PropertyStrCode = inValue.PropertyStrCode.Trim(" ".ToCharArray());
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateWarning(this PropertyStr entity, PropertyStr inValue)
		{
			int ret = 0;
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			entity.Name = "";
			entity.DomainObject = "Unknown";
			entity.TypeCode = "";
			entity.Code= "";

			try
			{
				entity.Name = inValue.Name.Trim(" ".ToCharArray());
				entity.DomainObject = inValue.DomainObject.Trim(" ".ToCharArray());		   //проверить через енум TODO
				entity.TypeCode = inValue.TypeCode.Trim(" ".ToCharArray());
				entity.Code = inValue.Code.Trim(" ".ToCharArray());
			}
			catch
			{
				//invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

	
		#endregion

		#region dictionary
		public static bool IsDictionaryContainsKey(this Dictionary<string, PropertyStr> dictionary, string key, string message, ILog log)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				//log.Add(MessageTypeEnum.WarningParser, MessageTypeEnum.WarningParser.ToString() + " : "
				//    + String.Format(ValidateErrorMessage.LocationCodeExistInDB, message));
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void AddToDictionary(this Dictionary<string, PropertyStr> dictionary, string key, PropertyStr newLocation, string message, ILog log)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				//log.Add(MessageType.Warning.ToString() + " : "
				//    + String.Format(ValidateErrorMessage.LocationCodeExistInDB, message));
			}
			else
			{
				dictionary.Add(key, newLocation);
			}
		}
		#endregion


        public static string CodeValidate(string code)
        {
            if (String.IsNullOrWhiteSpace(code))
                return Localization.Resources.ViewModel_LocationAddEdit_CodeEmpty;

            return String.Empty;
        }

        public static string NameValidate(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return Localization.Resources.ViewModel_LocationAddEdit_NameEmpty;

            return String.Empty;
        }
    }
}
