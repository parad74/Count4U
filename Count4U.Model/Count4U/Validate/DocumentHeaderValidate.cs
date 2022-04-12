using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Interface;
using Count4U.Model;

namespace Count4U.Model.Count4U
{
	public static class DocumentHeaderValidate
	{
		public static string ConvertDataErrorCode2ErrorMessage(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.DocumentCode;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.IturCode;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.Document;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.DocumentCode;
			}
			return "";
		}

		public static List<string> Bit2WarningMessage(int bit)
		{
			List<string> ret = new List<string>();
			List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
			foreach (var b in bitList)
			{
				ret.Add(DocumentHeaderValidate.ConvertDataErrorCode2WarningMessage(b));
			}
			return ret;
		}

		public static string ConvertDataErrorCode2WarningMessage(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.NoError:
					return ValidateMessage.NoError.General;
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ValidateMessage.InvalidValue.General;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ValidateMessage.FKCodeIsEmpty.IturCode;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ValidateMessage.SameCodeExist.General;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ValidateMessage.IsEmpty.DocumentCode;
			}
			return "";
		}

		#region validate
		public static int ValidateError(this DocumentHeader entity, DocumentHeaderString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			entity.DocumentCode = "";
			entity.StatusDocHeaderBit = 0;
			entity.DocNum = 1;

			int ret = 0;

			//DocumentCode
			if (inValue == null)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else if (string.IsNullOrWhiteSpace(inValue.DocumentCode) == true)
			{
				codeIsEmpty = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
			}
			else
			{
				entity.DocumentCode = inValue.DocumentCode.Trim(" ".ToCharArray());
			}

			if (string.IsNullOrWhiteSpace(inValue.IturCode) == true)
			{
				fkCodeIsEmpty = (int)ConvertDataErrorCodeEnum.FKCodeIsEmpty;
			}
 
			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			entity.StatusDocHeaderBit = entity.StatusDocHeaderBit.Or(ret);
			return ret;
		}

		public static int ValidateWarning(this DocumentHeader entity, DocumentHeaderString inValue, DateTimeFormatInfo dtfi)
		{
			int invalidValueBit = 0;
			int fkCodeIsEmpty = 0;
			int codeIsEmpty = 0;
			entity.Name = "";
			entity.SessionCode = "";
			entity.Code = "";
			entity.StatusDocHeaderBit = 0;
			entity.DocNum = 1;
			string iturCode = "";
			int ret = 0;

			entity.Approve = null;
			//try
			//{
			if (inValue != null)
			{
				entity.Name = inValue.Name;
				entity.SessionCode = inValue.SessionCode;
				entity.Code = inValue.Code;
				string WorkID = Convert.ToString(inValue.WorkerGUID);

				if (string.IsNullOrWhiteSpace(WorkID) == true)
				{
					entity.WorkerGUID = "000000001";
				}
				//else if ((WorkID.Trim(" ".ToCharArray()).Length) != 9)
				//{
				//	entity.WorkerGUID = "000000001";
				//}
				else
				{
					WorkID = WorkID.Trim(" ".ToCharArray());
					//long number = 0;
					//bool resultParse = long.TryParse(WorkID.TrimStart('0'), out number);
					//if (resultParse == true)
					//{
						entity.WorkerGUID = WorkID;
					//}
					//else
					//{
					//	entity.WorkerGUID = "000000001";
					//}
				}

				entity.CreateDate = DateTime.Now;
				entity.ModifyDate = DateTime.Now;
				try
				{
					DateTime dateTime = Convert.ToDateTime(inValue.CreateDate.Trim(" ".ToCharArray()), dtfi);
					entity.CreateDate = dateTime;
				}
				catch { }

				iturCode = inValue.IturCode;
	
				if (iturCode.Length == 8)
				{
					try
					{
						long number = Convert.ToInt64(iturCode.TrimStart('0'));
						entity.IturCode = iturCode;
					}
					catch
					{
						invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
					}
				}
				else
				{
					invalidValueBit = (int)ConvertDataErrorCodeEnum.InvalidValue;
				}
			}

			ret = ret + invalidValueBit + fkCodeIsEmpty + codeIsEmpty;
			entity.StatusDocHeaderBit = entity.StatusDocHeaderBit.Or(ret);
			return ret;
		}

		public static int Validate(this DocumentHeader entity, DocumentHeaderString inValue, DateTimeFormatInfo dtfi)
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
 		public static bool IsDictionaryContainsKey(this Dictionary<string, DocumentHeader> dictionary, string key, string message, ILog log)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				//log.Add(MessageTypeEnum.WarningParser, MessageTypeEnum.WarningParser.ToString() + " : "
				//    + String.Format(ValidateErrorMessage.DocumentCodeExistInDB, message));
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void AddToDictionary(this Dictionary<string, DocumentHeader> dictionary, string key, DocumentHeader newItur, string message, ILog log)
		{
			if (dictionary.ContainsKey(key) == true)
			{
				//log.Add(MessageType.Warning.ToString() + " : "
				//    + String.Format(ValidateErrorMessage.DocumentCodeExistInDB, message));
			}
			else
			{
				dictionary.Add(key, newItur);
			}
		}
		#endregion
    }
}
