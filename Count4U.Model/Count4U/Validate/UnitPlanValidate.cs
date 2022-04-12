using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U.Validate
{
	public static class UnitPlanValidate
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

		public static int ValidateError(this UnitPlan entity, UnitPlanString inValue)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			entity.UnitPlanCode = "";

			//Code
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.UnitPlanCode) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				entity.UnitPlanCode = inValue.UnitPlanCode.Trim(' ');
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateWarning(this UnitPlan entity, UnitPlanString inValue)
		{
			int ret = 0;
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;


			entity.LayerCode = inValue.LayerCode.Trim(' ');
			entity.ObjectCode = inValue.ObjectCode.Trim(' ');
			entity.Description = inValue.Description.Trim(' ');

			double startX = 0.0;
			bool yes = Double.TryParse(inValue.StartX.Trim(), out startX);
			if (yes == false)
			{
				if (string.IsNullOrWhiteSpace(inValue.StartX.Trim()) == false)
				{
					invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
				}
			}

			double startY = 0.0;
			yes = Double.TryParse(inValue.StartY.Trim(), out startY);
			if (yes == false)
			{
				if (string.IsNullOrWhiteSpace(inValue.StartY.Trim()) == false)
				{
					invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
				}
			}

			double height = 0.0;
			yes = Double.TryParse(inValue.Height.Trim(), out height);
			if (yes == false)
			{
				if (string.IsNullOrWhiteSpace(inValue.Height.Trim()) == false)
				{
					invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
				}
			}

			double width = 0.0;
			yes = Double.TryParse(inValue.Width.Trim(), out width);
			if (yes == false)
			{
				if (string.IsNullOrWhiteSpace(inValue.Width.Trim()) == false)
				{
					invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
				}
			}

			int rotate = 0;
			yes = Int32.TryParse(inValue.Rotate.Trim(), out rotate);
			if (yes == false)
			{
				if (string.IsNullOrWhiteSpace(inValue.Rotate.Trim()) == false)
				{
					invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
				}
			}

			entity.StartX =startX;
			entity.StartY =startY;
			entity.Height = height;
			entity.Width = width;
			entity.Rotate = rotate;
			

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int Validate(this UnitPlan entity, UnitPlanString inValue)
		{
			int ret = 0;
			int validateError = 0;
			int validateWarning = 0;

			validateError = ValidateError(entity, inValue);
			if (validateError == 0)
			{
				validateWarning = ValidateWarning(entity, inValue);
			}

			ret = ret + validateError + validateWarning;
			return ret;
		}
		#endregion

		#region dictionary
		public static bool IsDictionaryContainsKey(this Dictionary<string, UnitPlan> dictionary, string key, string message, ILog log)
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

		public static void AddToDictionary(this Dictionary<string, UnitPlan> dictionary, string key, UnitPlan newLocation, string message, ILog log)
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
