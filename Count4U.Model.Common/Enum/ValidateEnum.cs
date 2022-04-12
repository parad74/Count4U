using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model
{
 	public static class ValidateErrorMessage	   //TODO
	{
		//Localization.Resources.Bit_ValidateErrorMessage_FileIsNotExist%"File : [{0}] Is not Exist"
		public static string FileIsNotExist = Localization.Resources.Bit_ValidateErrorMessage_FileIsNotExist;
		//Localization.Resources.Bit_ValidateErrorMessage_FileIsEmpty%"File : [{0}] Is Empty or Is not Exist"
		public static string FileIsEmpty = Localization.Resources.Bit_ValidateErrorMessage_FileIsEmpty;
		//Localization.Resources.Bit_ValidateErrorMessage_NoHeaderLine%"There Is no Header Line or not the Marker Header in the First Line of File :  [{0}]"
		public static string NoHeaderLine = Localization.Resources.Bit_ValidateErrorMessage_NoHeaderLine;
		//Localization.Resources.Bit_ValidateErrorMessage_NoOneDataRow%"There Is no One Data Row in File : [{0}]"
		public static string NoOneDataRow = Localization.Resources.Bit_ValidateErrorMessage_NoOneDataRow;
		//Localization.Resources.Bit_ValidateErrorMessage_NoMatchNumberSubstrings%"Does not Match the Number of Substrings in the Data Row with the Expected :  [{0}]"
		public static string NoMatchNumberSubstrings = Localization.Resources.Bit_ValidateErrorMessage_NoMatchNumberSubstrings;
		//Localization.Resources.Bit_ValidateErrorMessage_NoExpectedLengthString%"Length of Input String does not match the Expected Length : [{0}]"
		public static string NoExpectedLengthString = Localization.Resources.Bit_ValidateErrorMessage_NoExpectedLengthString;
		//Localization.Resources.Bit_ValidateErrorMessage_NoExpectedMarker%"Data Row no Expected Marker : [{0}]"
		public static string NoExpectedMarker = Localization.Resources.Bit_ValidateErrorMessage_NoExpectedMarker;
		//Localization.Resources.Bit_ValidateErrorMessage_MakatNotExistInDB%"Makat [{0}] Exist in Data Row, but Not Exist in Catalog as Makat"
		public static string MakatNotExistInDB = Localization.Resources.Bit_ValidateErrorMessage_MakatNotExistInDB;
		//Localization.Resources.Bit_ValidateErrorMessage_MakatExistInDB%"Same Makat [{0}] Exist in DB"
		public static string MakatExistInDB = Localization.Resources.Bit_ValidateErrorMessage_MakatExistInDB;
		//Localization.Resources.Bit_ValidateErrorMessage_BarcodeNotExistInDB%"Barcode [{0}] Exist in Data Row, but Not Exist in Catalog as Barcode"
		public static string BarcodeNotExistInDB = Localization.Resources.Bit_ValidateErrorMessage_BarcodeNotExistInDB;
		//Localization.Resources.Bit_ValidateErrorMessage_BarcodeExistInDB%"Same Barcode [{0}] Exist in DB"
		public static string BarcodeExistInDB = Localization.Resources.Bit_ValidateErrorMessage_BarcodeExistInDB;
		//Localization.Resources.Bit_ValidateErrorMessage_BarcodeIsEmpty%"Barcode : [{0}] Is Empty"
		public static string BarcodeIsEmpty = Localization.Resources.Bit_ValidateErrorMessage_BarcodeIsEmpty;
		//Localization.Resources.Bit_ValidateErrorMessage_MakatIsEmpty%"Makat : [{0}] Is Empty"
		public static string MakatIsEmpty = Localization.Resources.Bit_ValidateErrorMessage_MakatIsEmpty;
		//Localization.Resources.Bit_ValidateErrorMessage_ParentMakatIsEmpty%"Parent Makat : [{0}] Is Empty"
		public static string ParentMakatIsEmpty = Localization.Resources.Bit_ValidateErrorMessage_ParentMakatIsEmpty;
		//Localization.Resources.Bit_ValidateErrorMessage_IturCodeNotExistInDB%"IturCode [{0}] Exist in Data Row, but Not Exist in Itur as Code"
		public static string IturCodeNotExistInDB = Localization.Resources.Bit_ValidateErrorMessage_IturCodeNotExistInDB;
		//Localization.Resources.Bit_ValidateErrorMessage_IturCodeExistInDB%"Itur with Same Code [{0}] Exist in DB"
		public static string IturCodeExistInDB = Localization.Resources.Bit_ValidateErrorMessage_IturCodeExistInDB;
		//Localization.Resources.Bit_ValidateErrorMessage_DocumentCodeExistInDB%"Document with Same Code [{0}] Exist in DB"
		public static string DocumentCodeExistInDB = Localization.Resources.Bit_ValidateErrorMessage_DocumentCodeExistInDB;
		//Localization.Resources.Bit_ValidateErrorMessage_SessionCodeExistInDB%"Session with Same Code [{0}] Exist in DB"
		public static string SessionCodeExistInDB = Localization.Resources.Bit_ValidateErrorMessage_SessionCodeExistInDB;
		//Localization.Resources.Bit_ValidateErrorMessage_LocationCodeNotExistInDB%"LocationCode [{0}] Exist in Data Row, but Not Exist in Location as Code"
		public static string LocationCodeNotExistInDB = Localization.Resources.Bit_ValidateErrorMessage_LocationCodeNotExistInDB;
		//Localization.Resources.Bit_ValidateErrorMessage_LocationCodeExistInDB%"Location with Same Code [{0}] Exist in DB"
		public static string LocationCodeExistInDB = Localization.Resources.Bit_ValidateErrorMessage_LocationCodeExistInDB;
		//Localization.Resources.Bit_ValidateErrorMessage_MakatAndBarcodeNotExistInDB%"Barcode (or Makat) [{0}] Exist in Data Row, but Not Exist in Catalog
		public static string MakatAndBarcodeNotExistInDB = Localization.Resources.Bit_ValidateErrorMessage_MakatAndBarcodeNotExistInDB;
		//Localization.Resources.Bit_ValidateErrorMessage_Warning%"Parser Warning : "
		public static string Warning = Localization.Resources.Bit_ValidateErrorMessage_Warning;
		//Localization.Resources.Bit_ValidateErrorMessage_Error%"Parser Error : "
		public static string Error = Localization.Resources.Bit_ValidateErrorMessage_Error;
	}

	public enum MessageTypeEnum
	{
		Error = 1,
		ErrorDB = 2,
		Fatal = 3, 
		Trace = 4,
		TraceParser = 5,
		TraceRepository = 6,
		TraceProvider = 7,
		TraceParserResult = 8,
		TraceRepositoryResult = 9,
		TraceProviderResult = 10,
		Warning =11,
		WarningParser = 12,
		WarningRepository = 13,
		SimpleTrace = 14,
		EndTrace = 15
	
	}

    public enum ImportExportLoglevel
    {
        Simple,
        Info,
        Advanced,
        Configurable
    }

	public static class MessageTypeCaption
	{
		//Localization.Resources.Bit_MessageTypeCaption_Error%"Error"
		public static string Error = Localization.Resources.Bit_MessageTypeCaption_Error;
		//Localization.Resources.Bit_MessageTypeCaption_ErrorDB%"ErrorDB"
		public static string ErrorDB = Localization.Resources.Bit_MessageTypeCaption_ErrorDB;
		//Localization.Resources.Bit_MessageTypeCaption_Fatal%"Fatal"
		public static string Fatal = Localization.Resources.Bit_MessageTypeCaption_Fatal;
		//Localization.Resources.Bit_MessageTypeCaption_Trace%"Trace"
		public static string Trace = Localization.Resources.Bit_MessageTypeCaption_Trace;
		//Localization.Resources.Bit_MessageTypeCaption_TraceParser%"TraceParser"
		public static string TraceParser = Localization.Resources.Bit_MessageTypeCaption_TraceParser;
		//Localization.Resources.Bit_MessageTypeCaption_TraceRepository%"TraceRepository"
		public static string TraceRepository = Localization.Resources.Bit_MessageTypeCaption_TraceRepository;
		//Localization.Resources.Bit_MessageTypeCaption_TraceProvider%"TraceProvider"
		public static string TraceProvider = Localization.Resources.Bit_MessageTypeCaption_TraceProvider;
		//Localization.Resources.Bit_MessageTypeCaption_Warning%"Warning"
		public static string Warning = Localization.Resources.Bit_MessageTypeCaption_Warning;
		//Localization.Resources.Bit_MessageTypeCaption_WarningParser%"WarningParser"
		public static string WarningParser = Localization.Resources.Bit_MessageTypeCaption_WarningParser;
		//Localization.Resources.Bit_MessageTypeCaption_WarningRepository%"WarningRepository"
		public static string WarningRepository = Localization.Resources.Bit_MessageTypeCaption_WarningRepository;
		//Localization.Resources.Bit_MessageTypeCaption_TraceParserResult%"TraceParserResult"
		public static string TraceParserResult = Localization.Resources.Bit_MessageTypeCaption_TraceParserResult;
		//Localization.Resources.Bit_MessageTypeCaption_TraceRepositoryResult%"TraceRepositoryResult"
		public static string TraceRepositoryResult = Localization.Resources.Bit_MessageTypeCaption_TraceRepositoryResult;
		//Localization.Resources.Bit_MessageTypeCaption_TraceProviderResult%"TraceProviderResult"
		public static string TraceProviderResult = Localization.Resources.Bit_MessageTypeCaption_TraceProviderResult;
		//Localization.Resources.Bit_MessageTypeCaption_SimpleTrace%"Simple Trace"
		public static string SimpleTrace = Localization.Resources.Bit_MessageTypeCaption_SimpleTrace;

	}
}
