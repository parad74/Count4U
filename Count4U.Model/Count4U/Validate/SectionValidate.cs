using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U.Validate
{
	public static class SectionValidate
	{
		#region property  validate
	
		#endregion

		#region message
		public static List<string> Bit2ErrorMessage(int bit)
		{
			List<string> ret = new List<string>();
			List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
			foreach (var b in bitList)
			{
				ret.Add(SectionValidate.ConvertDataErrorCode2ErrorMessage(b));
			}
			return ret;
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

		public static List<string> Bit2WarningMessage(int bit)
		{
			List<string> ret = new List<string>();
			List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
			foreach (var b in bitList)
			{
				ret.Add(SectionValidate.ConvertDataErrorCode2WarningMessage(b));
			}
			return ret;
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

		#endregion

		#region Validate

		public static int ValidateError(this Section entity, SectionString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			entity.SectionCode = "";
			entity.Name = "";
			entity.Description = "";
	
				//Code
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.SectionCode) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				entity.SectionCode = inValue.SectionCode.Trim(" ".ToCharArray());
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			//entity.StatusIturBit = entity.StatusIturBit.Or(ret);
			return ret;
		}


		public static int ValidateWarning(this Section entity, SectionString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;

			int ret = 0;

			entity.Name = inValue.Name;
			entity.Description = inValue.Description;
			entity.ParentSectionCode = inValue.ParentSectionCode;
			entity.Tag = inValue.Tag;
			entity.TypeCode = string.IsNullOrWhiteSpace(inValue.TypeCode) == false ? inValue.TypeCode : TypeSectionEnum.S.ToString();
	
			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int Validate(this Section entity, SectionString inValue, DateTimeFormatInfo dtfi)
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

		public static bool IsDictionaryContainsKey(this Dictionary<string, Section> dictionary, string key, string message = "", ILog log = null)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				return true;
			}
			else
			{
				dictionary[key] = null;
				return false;
			}
		}

		public static void AddToDictionary(this Dictionary<string, Section> dictionary, string key, Section newItur, string message, ILog log)
		{
			if (dictionary.ContainsKey(key) == true)
			{
			}
			else
			{
				dictionary.Add(key, newItur);
			}
		}
		#endregion
	}


}
