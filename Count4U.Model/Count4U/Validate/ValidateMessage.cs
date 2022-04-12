using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.ValidateMessage
{
	public static class InvalidValue
	{
		public static string General = "Invalid Value";
		public static string Prefix = "Invalid Prefix Value." + Environment.NewLine +
												"Prefix can not be Empty." + Environment.NewLine +
												"It Is Expected Numeric String" + Environment.NewLine +
												"4 characters maximum";
		public static string Code = "Invalid Code Value";
		public static string DocumentCode = "Invalid DocumentCode Value";
	}

	public static class SameCodeExist
	{
		public static string General = "Object with the Same Code Exist in DB";
		public static string Itur = "Itur with the Same Code Exist in DB";
		public static string Location = "Location with the Same Code Exist in DB";
		public static string Document = "Document with the Same DocumentCode Exist in DB";
	}

	public static class FKCodeIsEmpty
	{
		public static string General = "FK Code Is Empty";
		public static string IturCode = "IturCode Is Empty";
		public static string LocationCode = "LocationCode Is Empty";
		public static string DocumentCode = "DocumentCode Is Empty";
		public static string BarcodeOrMakatIsEmpty = "Barcode or Makat Is Empty";
		public static string BarcodeOrMakatIsNotExistsInCatalog = "Barcode or Makat Is Not Exists In Catalog ";
	}

	public static class IsEmpty
	{
		public static string General = "Code can not be Empty";
		public static string Code = "Code can not be Empty";
		public static string Prefix = "Prefix or Suffix can not be Empty";
		public static string DocumentCode = "DocumentCode can not be Empty";
		public static string Number = "Number can not be Empty";
		public static string Makat = "Makat can not be Empty";
		public static string Barcode = "Barcode can not be Empty";
	}

	public static class InvalidFormat
	{
		public static string General = "Invalid Format";
	}



}
