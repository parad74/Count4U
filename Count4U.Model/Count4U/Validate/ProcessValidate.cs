using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U.Validate
{
	public static class ProcessValidate
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

		//public static int ValidateError(this Location entity, LocationString inValue, Random rnd)
		//{
		//	int invalidValueBit = 0;
		//	int fkCodeIsEmpty = 0;
		//	int codeIsEmpty = 0;
		//	int ret = 0;

		//	entity.Code = "";

		//	//Code
		//	if (inValue == null)
		//	{
		//		codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
		//	}
		//	else if (string.IsNullOrWhiteSpace(inValue.Code) == true)
		//	{
		//		codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
		//	}
		//	else
		//	{
		//		entity.Code = inValue.Code.Trim(" ".ToCharArray());
		//	}

		//	ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
		//	return ret;
		//}

		//public static int ValidateWarning(this Location entity, LocationString inValue, Random rnd)
		//{
		//	int ret = 0;
		//	int invalidValueBit = 0;
		//	int fkCodeIsEmpty = 0;
		//	int codeIsEmpty = 0;
		//	entity.Name = "";
		//	entity.Description = "";
		//	entity.BackgroundColor = "";
		//	entity.Tag = "";

		//	entity.Level1 =  "";
		//	entity.Name1 = "";
		//	entity.Level2 = "";
		//	entity.Name2 = "";
		//	entity.Level3 = "";
		//	entity.Name3 = "";
		//	entity.Level4 = "";
		//	entity.Name4 = "";


		//	try
		//	{
		//		entity.Name = inValue.Name.Trim(" ".ToCharArray());
		//		entity.Description = inValue.Description.Trim(" ".ToCharArray());
		//		entity.Tag = inValue.Tag.Trim(" ".ToCharArray());
		//		entity.Level1 = inValue.Level1.Trim(" ".ToCharArray());
		//		entity.Name1 = inValue.Name1.Trim(" ".ToCharArray());
		//		entity.Level2 = inValue.Level2.Trim(" ".ToCharArray());
		//		entity.Name2 = inValue.Name2.Trim(" ".ToCharArray());
		//		entity.Level3 = inValue.Level3.Trim(" ".ToCharArray());
		//		entity.Name3 = inValue.Name3.Trim(" ".ToCharArray());
		//		entity.Level4 = inValue.Level4.Trim(" ".ToCharArray());
		//		entity.Name4 = inValue.Name4.Trim(" ".ToCharArray());
		//	}
		//	catch
		//	{
		//		//invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
		//	}

		//	if (string.IsNullOrWhiteSpace(inValue.BackgroundColor) == true)
		//	{
		//		int bg1 = rnd.Next(0, 255);
		//		int bg2 = rnd.Next(0, 255);
		//		int bg3 = rnd.Next(0, 255);
		//		entity.BackgroundColor = bg1 + ", " + bg2 + ", " + bg3;
		//	}
		//	else
		//	{
		//		try
		//		{
		//			string bg = inValue.BackgroundColor.Trim(" ".ToCharArray());
		//			string[] backgroundColor = bg.Split(':');
		//			int bg1 = Int32.Parse(backgroundColor[0].Trim()) % 255;
		//			int bg2 = Int32.Parse(backgroundColor[1].Trim()) % 255;
		//			int bg3 = Int32.Parse(backgroundColor[2].Trim()) % 255;
		//			bg = bg1 + ", " + bg2 + ", " + bg3;
		//			entity.BackgroundColor = bg;
		//		}
		//		catch
		//		{
		//			int bg1 = rnd.Next(0, 255);
		//			int bg2 = rnd.Next(0, 255);
		//			int bg3 = rnd.Next(0, 255);
		//			entity.BackgroundColor = bg1 + ", " + bg2 + ", " + bg3;
		//		}
		//	}
			
		//	ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
		//	return ret;
		//}

		//public static int Validate(this Location entity, LocationString inValue, Random rnd)
		//{
		//	int ret = 0;
		//	int validateError = 0;
		//	int validateWarning = 0;

		//	validateError = ValidateError(entity, inValue, rnd);
		//	if (validateError == 0)
		//	{
		//		validateWarning = ValidateWarning(entity, inValue, rnd);
		//	}

		//	ret = ret + validateError + validateWarning;
		//	return ret;
		//}
		#endregion

		#region dictionary
		//public static bool IsDictionaryContainsKey(this Dictionary<string, Location> dictionary, string key, string message, ILog log)
		//{
		//	if (dictionary.ContainsKey(key) == true)
		//	{
		//		//log.Add(MessageTypeEnum.WarningParser, MessageTypeEnum.WarningParser.ToString() + " : "
		//		//    + String.Format(ValidateErrorMessage.LocationCodeExistInDB, message));
		//		return true;
		//	}
		//	else
		//	{
		//		return false;
		//	}
		//}

		//public static void AddToDictionary(this Dictionary<string, Location> dictionary, string key, Location newLocation, string message, ILog log)
		//{
		//	if (dictionary.ContainsKey(key) == true)
		//	{
		//		//log.Add(MessageType.Warning.ToString() + " : "
		//		//    + String.Format(ValidateErrorMessage.LocationCodeExistInDB, message));
		//	}
		//	else
		//	{
		//		dictionary.Add(key, newLocation);
		//	}
		//}
		#endregion


        public static string ProcessCodeValidate(string processCode)
        {
			if (String.IsNullOrWhiteSpace(processCode))
				return Localization.Resources.ViewModel_ProcessCodeAddEdit_CodeEmpty;

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
