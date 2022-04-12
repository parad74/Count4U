using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U.Validate
{
	public static class SessionValidate
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
		public static int ValidateError(this Session entity, SessionString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			int ret = 0;

			entity.SessionCode = "";
			entity.CountItem = 0;
			entity.CountDocument = 0;
			entity.CountItur = 0;
	
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.SessionCode) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				entity.SessionCode = inValue.SessionCode.Trim(" ".ToCharArray());
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int ValidateWarning(this Session entity, SessionString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			string PDAID = "";
			string WorkID = "";
			int ret = 0;

			try
			{
				entity.CountItem = Convert.ToInt32(inValue.CountItem);
				entity.CountDocument = Convert.ToInt32(inValue.CountDocument);
				entity.CountItur = Convert.ToInt32(inValue.CountItur);
			}
			catch { }
			PDAID = Convert.ToString(inValue.PDAID).Trim(" ".ToCharArray());
			
			if (string.IsNullOrWhiteSpace(PDAID) == true || PDAID.Length != 6)
			{
				entity.PDAID = "000001";
			}
			else
			{
				try
				{
					long number = Convert.ToInt64(PDAID.TrimStart('0'));
					entity.PDAID = PDAID;
				}
				catch { entity.PDAID = "000001"; }
			}

			WorkID = Convert.ToString(inValue.WorkerGUID).Trim(" ".ToCharArray());
			
			if (string.IsNullOrWhiteSpace(WorkID) == true || WorkID.Length != 9)
			{
				entity.WorkerGUID = "000000001";
			}
			else
			{
				try
				{
					long number = Convert.ToInt64(WorkID.TrimStart('0'));
					entity.WorkerGUID = WorkID;
				}
				catch { entity.WorkerGUID = "000000001"; }
			}

			entity.PDADate = DateTime.Now;
			entity.CreateDate = DateTime.Now;
			try
			{
				DateTime dateTime = Convert.ToDateTime(inValue.PDADate.Trim(" ".ToCharArray()), dtfi);
				entity.PDADate = dateTime;
				entity.CreateDate = dateTime;
			}
			catch { }

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			return ret;
		}

		public static int Validate(this Session entity, SessionString inValue, DateTimeFormatInfo dtfi)
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
		public static bool IsDictionaryContainsKey(this Dictionary<string, Session> dictionary, string key, string message, ILog log)
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

		public static void AddToDictionary(this Dictionary<string, Session> dictionary, string key, Session newItur, string message, ILog log)
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
