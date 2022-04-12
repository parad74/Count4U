using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U.Validate
{
	public static class SupplierValidate
    {
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
					return ValidateMessage.IsEmpty.General;
			}
			return "";
		}

		#region validate
		public static int ValidateError(this Supplier entity, SupplierString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			entity.SupplierCode = "";
			//entity.CountItem = 0;
			//entity.CountDocument = 0;
			//entity.CountItur = 0;

			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.SupplierCode) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				entity.SupplierCode = inValue.SupplierCode.Trim(" ".ToCharArray());
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateWarning(this Supplier entity, SupplierString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;
			entity.Name = "";
			entity.Description = "";

			try
			{
				entity.Name = inValue.Name.Trim(" ".ToCharArray());
				entity.Description = inValue.Description.Trim(" ".ToCharArray());
			}
			catch
			{
				//invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int Validate(this Supplier entity, SupplierString inValue, DateTimeFormatInfo dtfi)
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

		#region dictionary
		public static bool IsDictionaryContainsKey(this Dictionary<string, Supplier> dictionary, string key, string message, ILog log)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				//log.Add(MessageTypeEnum.WarningParser, MessageTypeEnum.WarningParser.ToString() + " : "
				//    + String.Format(ValidateErrorMessage.SessionCodeExistInDB, message));
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void AddToDictionary(this Dictionary<string, Supplier> dictionary, string key, Supplier newItur, string message, ILog log)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				//log.Add(MessageType.Warning.ToString() + " : "
				//    + String.Format(ValidateErrorMessage.SessionCodeExistInDB, message));
			}
			else
			{
				dictionary.Add(key, newItur);
			}
		}
		#endregion
	}
}
