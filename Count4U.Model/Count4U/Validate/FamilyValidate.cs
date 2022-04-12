using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U.Validate
{
	public static class FamilyValidate
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
		public static int ValidateError(this Family entity, FamilyString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			entity.FamilyCode = "";
	
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.FamilyCode) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				entity.FamilyCode = inValue.FamilyCode.Trim(" ".ToCharArray());
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateWarning(this Family entity, FamilyString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;
			entity.Name = "";
			entity.Description = "";
			entity.Size = "";
			entity.Type = "";
			entity.Extra1 = "";
			entity.Extra2 = "";

			try
			{
				entity.Name = inValue.Name.Trim(" ".ToCharArray());
				entity.Description = inValue.Description.Trim(" ".ToCharArray());
				entity.Size = inValue.Size.Trim(" ".ToCharArray());
				entity.Type = inValue.Type.Trim(" ".ToCharArray());
				entity.Extra1 = inValue.Extra1.Trim(" ".ToCharArray());
				entity.Extra2 = inValue.Extra2.Trim(" ".ToCharArray());

			}
			catch
			{
				//invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int Validate(this Family entity, FamilyString inValue, DateTimeFormatInfo dtfi)
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

		public static void ValidateLenght50(this Family entity)
		{
			string familyCode = entity.FamilyCode;
			string name = entity.Name;
			string description = entity.Description;
			string type = entity.Type;
			string size = entity.Size;
			string extra1 = entity.Extra1;
			string extra2 = entity.Extra2;
			if (entity.FamilyCode.Length > 49) familyCode = entity.FamilyCode.Remove(49);
			if (entity.Name.Length > 49) name = entity.Name.Remove(49);
			if (entity.Description.Length > 99) description = entity.Description.Remove(99);
			if (entity.Type.Length > 49) type = entity.Type.Remove(49);
			if (entity.Size.Length > 49) size = entity.Size.Remove(49);
			if (entity.Extra1.Length > 49) extra1 = entity.Extra1.Remove(49);
			if (entity.Extra2.Length > 49) extra2 = entity.Extra2.Remove(49);

			entity.FamilyCode = familyCode;
			entity.Name = name;
			entity.Description = description;
			entity.Type = type;
			entity.Size = size;
			entity.Extra1 = extra1;
			entity.Extra2 = extra2;

		}

		#endregion

		#region dictionary
		public static bool IsDictionaryContainsKey(this Dictionary<string, Family> dictionary, string key, string message ="", ILog log = null)
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

		public static void AddToDictionary(this Dictionary<string, Family> dictionary, string key, Family newFamily, string message, ILog log)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				//log.Add(MessageType.Warning.ToString() + " : "
				//    + String.Format(ValidateErrorMessage.SessionCodeExistInDB, message));
			}
			else
			{
				dictionary.Add(key, newFamily);
			}
		}
		#endregion
	}
}
