using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Count4U.Model.Interface;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U.Validate
{
	public static class IturValidate
	{
		#region property  validate
		public static int PrefixValidate(this string entity)
		{
			if (String.IsNullOrEmpty(entity) == true)
			{
				return (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			if ((entity.Length <= 4)
				&& (Regex.IsMatch(entity, @"^[0-9]+$")))
			{
				return (int)ConvertDataErrorCodeEnum.NoError;
			}
			else
			{
				return (int)ConvertDataErrorCodeEnum.InvalidValue;
			}
		}

        public static string IturCodeValidate(this string entity)
        {
            if (String.IsNullOrEmpty(entity) == true)
            {
                return ValidateMessage.IsEmpty.Code;
            }
            if ((entity.Length == 8)
                && (Regex.IsMatch(entity, @"^[0-9]+$")))
            {
                return ValidateMessage.NoError.General;
            }
            else
            {
                return ValidateMessage.InvalidValue.Code; ;
            }
        }
		#endregion

		#region message
		public static string Bit2PrefixErrorMessage(int bit)
		{
			string ret = "";
			List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
			foreach (var b in bitList)
			{
				ret = ret + IturValidate.ConvertDataErrorCode2PrefixErrorString(b) + Environment.NewLine;
			}
			return ret;
		}

		public static string Bit2PrefixErrorMessageWitoutNewLine(int bit)
		{
			string ret = "";
			List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
			foreach (var b in bitList)
			{
				ret = ret + IturValidate.ConvertDataErrorCode2PrefixErrorString(b) + " ";
			}
			return ret;
		}

		public static string ConvertDataErrorCode2PrefixErrorString(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.Prefix;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.General;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.Itur;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.Prefix;
			}
			return "";
		}

		public static List<string> Bit2ErrorMessage(int bit)
		{
			List<string> ret = new List<string>();
			List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
			foreach (var b in bitList)
			{
				ret.Add(IturValidate.ConvertDataErrorCode2ErrorMessage(b));
			}
			return ret;
		}

		public static string ConvertDataErrorCode2ErrorMessage(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.Code;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.LocationCode;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.General;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.Code;
			}
			return "";
		}

		public static List<string> Bit2WarningMessage(int bit)
		{
			List<string> ret = new List<string>();
			List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
			foreach (var b in bitList)
			{
				ret.Add(IturValidate.ConvertDataErrorCode2WarningMessage(b));
			}
			return ret;
		}

		public static string ConvertDataErrorCode2WarningMessage(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.Code;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.LocationCode;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.General;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.Code;
			}
			return "";
		}

		#endregion

		#region Validate
		
		public static int ValidateError(this Itur entity, IturString inValue, DateTimeFormatInfo dtfi)
		{	
 			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			entity.IturCode = "";
			entity.ERPIturCode = "";
			entity.StatusIturBit = 0;
			
			//Code
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.IturCode) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (inValue.IturCode == "00000000")
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				string iturCode =  inValue.IturCode.Trim(' ');
				entity.IturCode = iturCode.CutLength(8); 
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			entity.StatusIturBit = entity.StatusIturBit.Or(ret);
			return ret;
		}


		public static int ValidateWarning(this Itur entity, IturString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;

			entity.NumberPrefix = "";
			entity.NumberSufix = "";
			entity.Number = 0;
			entity.LocationCode = "";
			entity.StatusIturBit = 0;
			entity.Name = "";

			int ret = 0;
			//	NumberPrefix, NumberSufix,	Number
			string code = entity.IturCode;
			entity.ERPIturCode = inValue.ERPIturCode != null ? inValue.ERPIturCode.CutLength(249) : "";
			try
			{
				if (inValue.Name != null)
				{
					entity.Name = inValue.Name;
					entity.Name = inValue.Name.CutLength(49);
				}
			}
			catch
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}

			try
			{
				if (code.Length == 8)
				{
					entity.NumberPrefix = code.Substring(0, 4);
					entity.NumberSufix = code.Substring(4, 4);
					//int ret = newItur.Number.ValidateInt32TrimStart0(newItur.NumberSufix);
					int number = Convert.ToInt32(entity.NumberSufix.TrimStart('0'));
					entity.Number = number;
	
				}
				//else
				//{
				//    invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
				//}
			}
			catch
			{
				invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}

			// LocationCode
			string locationCode = inValue.LocationCode;
 			if (string.IsNullOrWhiteSpace(locationCode) == true)
			{
				//fkCodeIsEmpty = (int)ConvertDataErrorCodeEnum.FKCodeIsEmpty;
				entity.LocationCode = DomainUnknownCode.UnknownLocation;
			}
			else
			{
				entity.LocationCode = locationCode;
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;

			entity.StatusIturBit = entity.StatusIturBit.Or(ret);
			return ret;
		}

		public static int Validate(this Itur entity, IturString inValue, DateTimeFormatInfo dtfi)
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

		public static bool IsDictionaryContainsKey(this Dictionary<string, Itur> dictionary, string key, string message = "", ILog log = null)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				//log.Add(MessageType.Warning.ToString() + " : "
				//    + String.Format(ValidateErrorMessage.IturCodeExistInDB, message));
				return true;
			}
			else
			{
				dictionary[key] = null;
				return false;
			}
		}

		public static void AddToDictionary(this Dictionary<string, Itur> dictionary, string key, Itur newItur, string message, ILog log)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				//log.Add(MessageType.Warning.ToString() + " : "
				//    + String.Format(ValidateErrorMessage.IturCodeExistInDB, message));
			}
			else
			{
				dictionary.Add(key, newItur);
			}
		}
		#endregion
	}

	//public static class ConvertPrefixErrorMessage
	//{
	//    public static string InvalidValuePrefix = "Invalid Prefix Value." + Environment.NewLine + 
	//                                                    "Prefix can not be Empty."  + Environment.NewLine +
	//                                                    "It Is Expected Numeric String" + Environment.NewLine + 
	//                                                    "4 characters maximum";
	//    public static string SameCodeExist = "Object with the Same Code Exist in DB";
	//    public static string FKCodeIsEmpty = "FK Code Is Empty";
	//    public static string CodeIsEmpty = "Prefix can not be Empty";
	//}
}
