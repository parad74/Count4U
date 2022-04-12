using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;

namespace Count4U.Model.Main
{
	//public class ConfigIni
	//{
		//public long ID { get; set; }
		//public string Description { get; set; }
		//public string Name { get; set; }
		//public string Value { get; set; }
		//public string CustomerСode { get; set; }
	//}
	public enum CustomerConfigIniEnum
	{
		Hash = 0,
		FileType = 1,
		QType = 2,
		UseAlphaKey = 3,
		ClientID = 4,
		NewItem = 5,
		IturNameType = 6,
		IturNamePrefix = 7,
		IturInvertPrefix = 8,
		Password = 9,
		HTcalculateLookUp = 10,
		LookUpEXE = 11,
		IsInvertWords = 12,
		IsInvertLetters = 13,
		EncodingCodePage = 14,
		AddNewLocation = 15,
		MaxQuantity = 16,
		LastSync = 17,
		IsInvertWordsConfig = 18,
		IsInvertLettersConfig = 19,
		IsCutAfterInvert = 20,
		AllowZeroQuantity = 21,
		PdaType,
		MaintenanceType,
		ProgramType,
		SearchDef,
		NewItemBool,
		ChangeQuantityType,
		IncludePreviousInventor,
		IncludeCurrentInventor,
		IncludeProfile,
		MaxLen,
		//IturTypeByName,
		CreateZipFile,
		IsAddBinarySearch,
		ConfirmNewLocation,
		ConfirmNewItem,
		AutoSendData,
		AllowQuantityFraction,
		AddExtraInputValue,
		AddExtraInputValueHeaderName,
		AddExtraInputValueSelectFromBatchListForm,
		AllowNewValueFromBatchListForm,
		SearchIfExistInBatchList,
		AllowMinusQuantity,
		FractionCalculate,
		ExcludeNotExistingInCatalog,
		PartialQuantity,
		Host1,
		Host2,
		DefaultHost,
		Timeout,
		Retry,
		SameBarcodeInLocation
	}

	public class CustomerConfigIniTitle
	{
		public static string Hash = "Hash";
		public static string FileType = "File Type";
		public static string QType = "Quantity Type";
		public static string UseAlphaKey = "Use Alpha Key";
		public static string ClientID = "Client ID";
		public static string NewItem = "New Item";
		public static string NewItemBool = "New Item";
		public static string IturNameType = "IturNameType";
		public static string IturNamePrefix = "IturNamePrefix";
		public static string Password = "Password";
		public static string HTcalculateLookUp = "HTcalculateLookUp";
		public static string LookUpEXE = "LookUpEXE";
		public static string Host1 = "Host1";
		public static string Host2 = "Host2";
		public static string DefaultHost = "DefaultHost";
		public static string Timeout = "Timeout";
		public static string Retry = "Retry";
		public static string SameBarcodeInLocation = "SameBarcodeInLocation";
		
		
		public static string AddNewLocation = "AddNewLocation";
		public static string MaxQuantity = "MaxQuantity";
		public static string AllowZeroQuantity = "AllowZeroQuantity";
		public static string LastSync = "LastSync";
		public static string IsInvertWordsConfig = "IsInvertWordsConfig";
		public static string IsInvertLettersConfig = "IsInvertLettersConfig";
		public static string IsInvertWords = "IsInvertWords";
		public static string IsInvertLetters = "IsInvertLetters";
		public static string EncodingCodePage = "EncodingCodePage";
		public static string IsCutAfterInvert = "IsCutAfterInvert";
		
	}
}
