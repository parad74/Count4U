using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model
{

	public static class ValidateErrorMessage	   //TODO
	{
		public static string FileIsNotExist = "File : [ {0} ] Is not Exist";
		public static string FileIsEmpty = "File : [ {0} ] Is Empty or Is not Exist";
		public static string NoHeaderLine = "There Is no Header Line or not the Marker Header in the First Line of File :  [ {0} ]";
		public static string NoOneDataRow = "There Is no One Data Row in File : [ {0} ]";
		public static string NoMatchNumberSubstrings = "Does not Match the Number of Substrings in the Data Row with the Expected :  [ {0} ]";
		public static string NoExpectedLengthString = "Length of Input String does not match the Expected Length : [ {0} ]";
		public static string NoExpectedMarker = "Data Row no Expected Marker : [ {0} ]";
		public static string MakatNotExistInDB = "Makat [ {0} ] Exist in Data Row, but Not Exist in Catalog as Makat";
		public static string MakatExistInDB = "Same Makat [ {0} ] Exist in DB";
		public static string BarcodeNotExistInDB = "Barcode [ {0} ] Exist in Data Row, but Not Exist in Catalog as Barcode";
		public static string BarcodeExistInDB = "Same Barcode [ {0} ] Exist in DB";
		public static string BarcodeIsEmpty = "Barcode : [ {0} ] Is Empty";
		public static string MakatIsEmpty = "Makat : [ {0} ] Is Empty";
		public static string ParentMakatIsEmpty = "Parent Makat : [ {0} ] Is Empty";
		public static string IturCodeNotExistInDB = "IturCode [ {0} ] Exist in Data Row, but Not Exist in Itur as Code";
		public static string IturCodeExistInDB = "Itur with Same Code [ {0} ] Exist in DB";
		public static string DocumentCodeExistInDB = "Document with Same Code [ {0} ] Exist in DB";
		public static string SessionCodeExistInDB = "Session with Same Code [ {0} ] Exist in DB";
		public static string LocationCodeNotExistInDB = "LocationCode [ {0} ] Exist in Data Row, but Not Exist in Location as Code";
		public static string LocationCodeExistInDB = "Location with Same Code [ {0} ] Exist in DB";
		public static string MakatAndBarcodeNotExistInDB = "Barcode (or Makat) [ {0} ] Exist in Data Row, but Not Exist in Catalog";
		public static string Warning = "Parser Warning : ";
		public static string Error = "Parser Error : ";
	}

	public enum MessageType
	{
		Warning = 1,
		Error = 2,
		Fatal = 3
	}
}
