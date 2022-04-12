using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;


namespace Count4U.Common.Web
{
	public enum OperationIndexEnum
	{
		[Display(Name = "none", Description = "none")]
		none = 0,
		[Display(GroupName = "Clear", Name = "c_01", Description = "clear 01")]
		c_01 = 1,
		[Display(GroupName = "Clear", Name = "c_02", Description = "clear 02")]
		c_02 = 2,
		[Display(GroupName = "Clear", Name = "c_03", Description = "clear 03")]
		c_03 = 3,
		[Display(GroupName = "Clear", Name = "c_04", Description = "clear 04")]
		c_04 = 4,
		[Display(GroupName = "Clear", Name = "c_05", Description = "clear 05")]
		c_05 = 5,
		[Display(GroupName = "Clear", Name = "c_06", Description = "clear 06")]
		c_06 = 6,
		[Display(GroupName = "Clear", Name = "c_07", Description = "clear 07")]
		c_07 = 7,
		[Display(GroupName = "Clear", Name = "c_08", Description = "clear 08")]
		c_08 = 8,
		[Display(GroupName = "Clear", Name = "c_09", Description = "clear 09")]
		c_09 = 9,
		[Display(GroupName = "Clear", Name = "c_10", Description = "clear 10")]
		c_10 = 10,
		[Display(GroupName = "Import", Name = "i_11", Description = "import 11")]
		i_11 = 11,
		[Display(GroupName = "Import", Name = "i_12", Description = "import 12")]
		i_12 = 12,
		[Display(GroupName = "Import", Name = "i_13", Description = "import 13")]
		i_13 = 13,
		[Display(GroupName = "Import", Name = "i_14", Description = "import 14")]
		i_14 = 14,
		[Display(GroupName = "Import", Name = "i_15", Description = "import 15")]
		i_15 = 15,
		[Display(GroupName = "Import", Name = "i_16", Description = "import 16")]
		i_16 = 16,
		[Display(GroupName = "Import", Name = "i_17", Description = "import 17")]
		i_17 = 17,
		[Display(GroupName = "Import", Name = "i_18", Description = "import 18")]
		i_18 = 18,
		[Display(GroupName = "Import", Name = "i_19", Description = "import 19")]
		i_19 = 19,
		[Display(GroupName = "Import", Name = "i_20", Description = "import 20")]
		i_20 = 20
	}

	public enum CommandResultCodeEnum
	{
		[Display(Description = "Ok")]
		Ok = 0,
		[Display(Description = "Warning")]
		Warning = 1,
		[Display(Description = "Error")]
		Error = 2,
		[Display(Description = "Unknown")]
		Unknown = 4,
		[Display(Description = "Info")]
		Info = 8,
		[Display(Description = "ValidateError")]
		ValidateError = 16,
		[Display(Description = "ValidateWarning")]
		ValidateWarning = 32

	}

	public enum EmailTemplateEnum
	{
		[Display(Description = "none")]
		none = 0
	}


	public enum TrySendEmailAfterEnum
	{
		[Display(Description = "no")]
		no = 0,
		[Display(Description = "yes")]
		yes = 1
	}

	[Serializable]
	public enum CommandErrorCodeEnum
	{
		[Display(Description = "none")]
		none = 0,
		[Display(Description = "CommandResultIsNull")]
		CommandResultIsNull = 904,
		[Display(Description = "ProfileFileIsNull")]
		ProfileFileIsNull = 804,
		[Display(Description = "dataServerAddressUrlIsNull")]
		dataServerAddressUrlIsNull = 914,
		[Display(Description = "FileNameIsNull")]
		FileNameIsNull = 924,
		[Display(Description = "AdapterNameIsNull")]
		AdapterNameIsNull = 934,
		[Display(Description = "PathIsNull")]
		PathIsNull = 944,
		[Display(Description = "SessionCodeIsNull")]
		SessionCodeIsNull = 954,
		[Display(Description = "CommandResultWithException")]
		CommandResultWithException = 905,
		[Display(Description = "AdapterInitCommandWithException")]
		AdapterInitCommandWithException = 915,
		[Display(Description = "CommandResultCansel")]
		CommandResultCansel = 907,
		[Display(Description = "ExceptionClear")]
		ExceptionClear = 915,
		[Display(Description = "ExceptionImport")]
		ExceptionImport = 925,
		[Display(Description = "Unknown")]
		Unknown = 900,
	}

	public enum IsSingleFileOrDirectoryEnum
	{
		[Display(Description = "Unknown", ShortName = "Unknown")]
		Unknown = 0,
		[Display(Description = "Is Single File", ShortName = "Is Single File")]
		IsSingleFile = 1,
		[Display(Description = "Is Directory", ShortName = "Is Directory")]
		IsDirectory = 2,
		[Display(Description = "Empty", ShortName = "Empty")]
		Empty = 4,
		[Display(Description = "InData Directory", ShortName = "InData Directory")]
		InData = 8
	}

	[Serializable]	
	public enum SuccessfulEnum
	{
		[Display(Description = "Successful", ShortName = "Successful")]
		Successful = 0,
		[Display(Description = "Not Successful", ShortName = "Not Successful")]
		NotSuccessful = 1,
		[Display(Description = "Not Start", ShortName = "Not Satart")]
		NotStart = 2,
		[Display(Description = "Waiting", ShortName = "Waiting")]
		Waiting = 4,
		[Display(Description = "Error", ShortName = "Error")]
		Error = 8,
		[Display(Description = "Canseled", ShortName = "Canseled")]
		Canseled = 16,
		[Display(Description = "ForgotPassword", ShortName = "Forgot Password")]
		ForgotPassword = 32,
		[Display(Description = "None", ShortName = "None")]
		None = 64,
		[Display(Description = "User not found", ShortName = "User not found")]
		UserNotFound = 128	,

	}

	public enum CompleteProcessEnum
	{
		[Display(Description = "Not Satart", ShortName = "Not Satart")]
		NotSatart = 0,
		[Display(Description = "In Process", ShortName = "In Process")]
		InProcess = 1,
		[Display(Description = "Finish Successful", ShortName = "Finish Successful")]
		FinishSuccessful = 2,
		[Display(Description = "Unknown", ShortName = "Unknown")]
		Unknown = 4,
		[Display(Description = "Finish Canseled", ShortName = "Finish Canseled")]
		FinishCanseled = 8,
		[Display(Description = "Finish Warning", ShortName = "Finish Warning")]
		FinishWarning = 16,
		[Display(Description = "Finish Error", ShortName = "Finish Error")]
		FinishError = 32
	}


	public enum AdapterCommandStepEnum
	{
		None = 0,
		[Display(GroupName = "Before", Name = "CopyFileBefore", Description = "Copy Files Before", ShortName = "Copy Files Before")]
		CopyFileBefore = 1,
		[Display(GroupName = "Before", Name = "Init", Description = "Init", ShortName = "Init")]
		Init = 2,
		[Display(GroupName = "Before", Name = "InitConfig", Description = "Init Config", ShortName = "Init Config")]
		InitConfig = 3,
		[Display(GroupName = "Operatoin", Name = "Clear", Description = "Clear", ShortName = "Clear")]
		Clear = 4,
		[Display(GroupName = "Operatoin", Name = "Import", Description = "Import", ShortName = "Import")]
		Import = 5,
		[Display(GroupName = "After", Name = "RefreshIturStatus", Description = "Refresh", ShortName = "Refresh")]
		RefreshIturStatus = 6,
		[Display(GroupName = "After", Name = "MoveFileAfter", Description = "Move File", ShortName = "Move File")]
		MoveFileAfter = 7,
		[Display(GroupName = "Result", Name = "ReturnResultCountLocation", Description = "Count Locations", ShortName = "Count Locations")]
		ReturnResultCountLocation = 8,
		[Display(GroupName = "Result", Name = "ReturnResultCountItur", Description = "Count Iturs", ShortName = "Count Iturs")]
		ReturnResultCountItur = 9,
		[Display(GroupName = "Result", Name = "ReturnResultCountDocumentHeader", Description = "Count DocumentHeaders", ShortName = "Count DocumentHeaders")]
		ReturnResultCountDocumentHeader = 10,
		[Display(GroupName = "Result", Name = "ReturnResultCountCatalog", Description = "Count Products", ShortName = "Count Products")]
		ReturnResultCountCatalog = 11,
		[Display(GroupName = "Result", Name = "ReturnResultCountInventProduct", Description = "Count InventProducts", ShortName = "Count InventProducts")]
		ReturnResultCountInventProduct = 12,
		[Display(GroupName = "Result", Name = "ReturnResultCountSection", Description = "Count Sections", ShortName = "Count Sections")]
		ReturnResultCountSection = 13,
		[Display(GroupName = "Result", Name = "ReturnResultCountSupplier", Description = "Count Suppliers", ShortName = "Count Suppliers")]
		ReturnResultCountSupplier = 14,
		[Display(GroupName = "Result", Name = "ReturnResult", Description = "Result", ShortName = "Result")]
		ReturnResult = 15,
		[Display(GroupName = "Result", Name = "ReturnErorr", Description = "Error", ShortName = "Error")]
		ReturnErorr = 16,
		[Display(GroupName = "Queue", Name = "SetQueue", Description = "Set in Queue", ShortName = "Set in Queue")]
		AddInQueue = 17,
		[Display(GroupName = "Queue", Name = "GetQueue", Description = "Up from Queue", ShortName = "Up from Queue")]
		GetQueue = 18,
		[Display(GroupName = "After", Name = "ClearStatusBit", Description = "Clear Status Bit", ShortName = "Clear Status Bit")]
		ClearStatusBit = 19


	}

	public enum ProfiFileStepEnum
	{
		None = 0,
		[Display(GroupName = "Before", Name = "CopyFileBefore", Description = "Copy Files Before", ShortName = "Copy Files Before")]
		CopyFileBefore = 1,
		[Display(GroupName = "Before", Name = "Init", Description = "Init", ShortName = "Init")]
		Init = 2,
		[Display(GroupName = "Before", Name = "InitConfig", Description = "Init Config", ShortName = "Init Config")]
		InitConfig = 3,
		[Display(GroupName = "Operatoin", Name = "Clear", Description = "Clear", ShortName = "Clear")]
		Clear = 4,
		[Display(GroupName = "Operatoin", Name = "SaveOrUpdatOnFtp", Description = "Save Or Updat On Ftp", ShortName = "SaveToFtp")]
		SaveOrUpdatOnFtp = 5,
		[Display(GroupName = "Operatoin", Name = "UpdateOrInsertObjectFromFtpToDb", Description = "Update Or Insert Object From Ftp To Db", ShortName = "UpdateOrInsertInventorFromFtpToDb")]
		UpdateOrInsertObjectFromFtpToDb = 6,
		[Display(GroupName = "Operatoin", Name = "GetByCodeFromFtp", Description = "Get By Code From Ftp", ShortName = "GetByCodeFromFtpTo")]
		GetByCodeFromFtp = 7,
		[Display(GroupName = "Result", Name = "ReturnResultCountLocation", Description = "Count Locations", ShortName = "Count Locations")]
		ReturnResultCountLocation = 8,
		[Display(GroupName = "Result", Name = "ReturnResultCountItur", Description = "Count Iturs", ShortName = "Count Iturs")]
		ReturnResultCountItur = 9,
		[Display(GroupName = "Result", Name = "ReturnResultCountDocumentHeader", Description = "Count DocumentHeaders", ShortName = "Count DocumentHeaders")]
		ReturnResultCountDocumentHeader = 10,
		[Display(GroupName = "Result", Name = "ReturnResultCountCatalog", Description = "Count Products", ShortName = "Count Products")]
		ReturnResultCountCatalog = 11,
		[Display(GroupName = "Result", Name = "ReturnResultCountInventProduct", Description = "Count InventProducts", ShortName = "Count InventProducts")]
		ReturnResultCountInventProduct = 12,
		[Display(GroupName = "Result", Name = "ReturnResultCountSection", Description = "Count Sections", ShortName = "Count Sections")]
		ReturnResultCountSection = 13,
		[Display(GroupName = "Result", Name = "ReturnResultCountSupplier", Description = "Count Suppliers", ShortName = "Count Suppliers")]
		ReturnResultCountSupplier = 14,
		[Display(GroupName = "Result", Name = "ReturnResult", Description = "Result", ShortName = "Result")]
		ReturnResult = 15,
		[Display(GroupName = "Result", Name = "ReturnErorr", Description = "Error", ShortName = "Error")]
		ReturnErorr = 16,
		[Display(GroupName = "Queue", Name = "SetQueue", Description = "Set in Queue", ShortName = "Set in Queue")]
		AddInQueue = 17,
		[Display(GroupName = "Queue", Name = "GetQueue", Description = "Up from Queue", ShortName = "Up from Queue")]
		GetQueue = 18,
		[Display(GroupName = "After", Name = "ClearStatusBit", Description = "Clear Status Bit", ShortName = "Clear Status Bit")]
		ClearStatusBit = 19


	}

	//public static class EnumHelper
	//{
	//	public static string GetDisplayDescription(this System.Enum val)
	//	{
	//		return val.GetType()
	//				  .GetMember(val.ToString())
	//				  .FirstOrDefault()
	//				  ?.GetCustomAttribute<DisplayAttribute>(false)
	//				  ?.Description
	//				  ?? val.ToString();
	//	}

	//	public static string GetDisplayDescriptionHe(this System.Enum val)
	//	{
	//		return val.GetType()
	//				  .GetMember(val.ToString())
	//				  .FirstOrDefault()
	//				  ?.GetCustomAttribute<DisplayAttribute>(false)
	//				  ?.ShortName
	//				  ?? val.ToString();
	//	}

	//	public static System.Enum GetEnumFromInt(this System.Enum defaultVal, int intVal)
	//	{
	//		Type EnumType = defaultVal.GetType();
	//		if (Enum.IsDefined(EnumType, intVal))
	//		{
	//			// System.Enum enm  = (EnumType)intVal; // преобразование 
	//			//  или CustomEnum enm = (CustomEnum)Enum.ToObject(typeof(CustomEnum), number);
	//			System.Enum enm = (System.Enum)Enum.ToObject(EnumType, intVal);
	//			return enm;
	//		}
	//		return defaultVal;
	//	}

	//	public static System.Enum GetEnumFromString(this System.Enum defaultVal, string stringVal)
	//	{
	//		Type EnumType = defaultVal.GetType();
	//		if (Enum.IsDefined(EnumType, stringVal))
	//		{
	//			// System.Enum enm  = (EnumType)intVal; // преобразование 
	//			//  или CustomEnum enm = (CustomEnum)Enum.ToObject(typeof(CustomEnum), number);
	//			System.Enum enm = (System.Enum)Enum.Parse(EnumType, stringVal);
	//			return enm;
	//		}
	//		return defaultVal;
	//	}

	//}
}
