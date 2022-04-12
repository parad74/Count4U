using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.ValidateMessage
{
	public static class InvalidValue
	{
		//Localization.Resources.Bit_InvalidValue_General%"Invalid Value"
		public static string General = Localization.Resources.Bit_InvalidValue_General;
		//Localization.Resources.Bit_InvalidValue_Prefix%"Invalid Prefix Value. {0} Prefix can not be Empty. {1} It Is Expected Numeric String {2} 4 characters maximum"
		public static string Prefix = String.Format(Localization.Resources.Bit_InvalidValue_Prefix, 
			Environment.NewLine, Environment.NewLine, Environment.NewLine);
		//Localization.Resources.Bit_InvalidValue_Code%"Invalid Code Value"
		public static string Code = Localization.Resources.Bit_InvalidValue_Code;
		//Localization.Resources.Bit_InvalidValue_DocumentCode%"Invalid DocumentCode Value"
		public static string DocumentCode = Localization.Resources.Bit_InvalidValue_DocumentCode;
		//Localization.Resources.Bit_InvalidValue_Name% "Invalid Name Value"
		public static string Name = Localization.Resources.Bit_InvalidValue_Name;
		//Localization.Resources.Bit_InvalidValue_Phone% "Invalid Phone Value"
		public static string Phone = Localization.Resources.Bit_InvalidValue_Phone;
		//Localization.Resources.Bit_InvalidValue_Email%"Invalid Email Value"
		public static string Email = Localization.Resources.Bit_InvalidValue_Email;
		//Localization.Resources.Bit_InvalidValue_Fax%"Invalid Fax Value"
		public static string Fax = Localization.Resources.Bit_InvalidValue_Fax;
	}

	public static class SameCodeExist
	{
		//Localization.Resources.Bit_SameCodeExist_General%"Object with the Same Code Exist in DB"
		public static string General = Localization.Resources.Bit_SameCodeExist_General;
		//Localization.Resources.Bit_SameCodeExist_Itur%"Itur with the Same Code Exist in DB"
		public static string Itur = Localization.Resources.Bit_SameCodeExist_Itur;
		//Localization.Resources.Bit_SameCodeExist_ProductMakat%"Product with the Same Makat Exist in DB"
		public static string ProductMakat = Localization.Resources.Bit_SameCodeExist_ProductMakat;
		//Localization.Resources.Bit_SameCodeExist_ProductBarcode%"Product with the Same Barcode Exist in DB"
		public static string ProductBarcode = Localization.Resources.Bit_SameCodeExist_ProductBarcode;
		//Localization.Resources.Bit_SameCodeExist_Location%"Location with the Same Code Exist in DB"
		public static string Location = Localization.Resources.Bit_SameCodeExist_Location;
		//Localization.Resources.Bit_SameCodeExist_Document%"Document with the Same DocumentCode Exist in DB"
		public static string Document = Localization.Resources.Bit_SameCodeExist_Document;
	}

	public static class FKCodeIsEmpty
	{
		//Localization.Resources.Bit_FKCodeIsEmpty_General%"FK Code Is Empty"
		public static string General = Localization.Resources.Bit_FKCodeIsEmpty_General;
		//Localization.Resources.Bit_FKCodeIsEmpty_IturCode%"IturCode Is Empty"
		public static string IturCode = Localization.Resources.Bit_FKCodeIsEmpty_IturCode;
		//Localization.Resources.Bit_FKCodeIsEmpty_LocationCode%"LocationCode Is Empty"
		public static string LocationCode = Localization.Resources.Bit_FKCodeIsEmpty_LocationCode;
		//Localization.Resources.Bit_FKCodeIsEmpty_DocumentCode%"DocumentCode Is Empty"
		public static string DocumentCode = Localization.Resources.Bit_FKCodeIsEmpty_DocumentCode;
		//Localization.Resources.Bit_FKCodeIsEmpty_BarcodeOrMakatIsEmpty%"Barcode or Makat Is Empty"
		public static string BarcodeOrMakatIsEmpty = Localization.Resources.Bit_FKCodeIsEmpty_BarcodeOrMakatIsEmpty;
		//Localization.Resources.Bit_FKCodeIsEmpty_BarcodeOrMakatIsNotExistsInCatalog%"Barcode or Makat Is Not Exists In Catalog "
		public static string BarcodeOrMakatIsNotExistsInCatalog = Localization.Resources.Bit_FKCodeIsEmpty_BarcodeOrMakatIsNotExistsInCatalog;
	}

	public static class IsEmpty
	{
		//Localization.Resources.Bit_IsEmpty_General%"Code can not be Empty"
		public static string General = Localization.Resources.Bit_IsEmpty_General;
		//Localization.Resources.Bit_IsEmpty_Code%"Code can not be Empty"
		public static string Code = Localization.Resources.Bit_IsEmpty_Code;
		//Localization.Resources.Bit_IsEmpty_Prefix%"Prefix or Suffix can not be Empty"
		public static string Prefix = Localization.Resources.Bit_IsEmpty_Prefix;
		//Localization.Resources.Bit_IsEmpty_DocumentCode%"DocumentCode can not be Empty"
		public static string DocumentCode = Localization.Resources.Bit_IsEmpty_DocumentCode;
		//Localization.Resources.Bit_IsEmpty_Number%"Number can not be Empty"
		public static string Number = Localization.Resources.Bit_IsEmpty_Number;
		//Localization.Resources.Bit_IsEmpty_Makat%"Makat can not be Empty"
		public static string Makat = Localization.Resources.Bit_IsEmpty_Makat;
		//Localization.Resources.Bit_IsEmpty_Barcode%"Barcode can not be Empty"
		public static string Barcode = Localization.Resources.Bit_IsEmpty_Barcode;
		//Localization.Resources.Bit_IsEmpty_Name%"Name can not be Empty"
		public static string Name = Localization.Resources.Bit_IsEmpty_Name;
	}

	public static class InvalidFormat
	{
		//Localization.Resources.Bit_InvalidFormat_General%"Invalid Format"
		public static string General = Localization.Resources.Bit_InvalidFormat_General;
	}

	public static class NoError
	{
		//Localization.Resources.Bit_NoError_General%
		public static string General = "";
	}

    public static class Customer
    {
		//Localization.Resources.Bit_Customer_CodeUnique%"Customer with such code already exists"
		public static string CodeUnique = Localization.Resources.Bit_Customer_CodeUnique;
		//Localization.Resources.Bit_Customer_CodeEmpty%Code can not be empty"
		public static string CodeEmpty = Localization.Resources.Bit_Customer_CodeEmpty;
		//Localization.Resources.Bit_Customer_CodeLength%"Code length must be in range between 3 and 10"
		public static string CodeLength = Localization.Resources.Bit_Customer_CodeLength;
		//Localization.Resources.Bit_Customer_CodeFormat%"Unacceptable symbols"
		public static string CodeFormat = Localization.Resources.Bit_Customer_CodeFormat;
		//Localization.Resources.Bit_Customer_PicFileMissed%"File doesn't exist"
		public static string PicFileMissed = Localization.Resources.Bit_Customer_PicFileMissed;
		//Localization.Resources.Bit_Customer_PicFileInvalid%"File is not valid image"
		public static string PicFileInvalid = Localization.Resources.Bit_Customer_PicFileInvalid;
		//Localization.Resources.Bit_Customer_ContactPersonFormat%"Unacceptable symbols"
		public static string ContactPersonFormat = Localization.Resources.Bit_Customer_ContactPersonFormat;
    }

    public static class Branch
    {
		//Localization.Resources.Bit_Branch_CodeUnique%"Branch with such code already exists"
		public static string CodeUnique = Localization.Resources.Bit_Branch_CodeUnique;
		//Localization.Resources.Bit_Branch_CodeEmpty%"Code can not be empty"
		public static string CodeEmpty = Localization.Resources.Bit_Branch_CodeEmpty;
		//Localization.Resources.Bit_Branch_CodeLength%"Code length must be in range between 3 and 10"
		public static string CodeLength = Localization.Resources.Bit_Branch_CodeLength;
		//Localization.Resources.Bit_Branch_CodeFormat%"Unacceptable symbols"
		public static string CodeFormat = Localization.Resources.Bit_Branch_CodeFormat;
		//Localization.Resources.Bit_Branch_ContactPersonFormat%"Unacceptable symbols"
		public static string ContactPersonFormat = Localization.Resources.Bit_Branch_ContactPersonFormat;
		//Localization.Resources.Bit_Branch_CodeLocalEmpty%"Code Local can not be empty"
		public static string CodeLocalEmpty = Localization.Resources.Bit_Branch_CodeLocalEmpty;
		//Localization.Resources.Bit_Branch_CodeLocalFormat%"Unacceptable symbols"
		public static string CodeLocalFormat = Localization.Resources.Bit_Branch_CodeLocalFormat;
		//Localization.Resources.Bit_Branch_CodeErpEmpty%"Code ERP can not be empty"
		public static string CodeErpEmpty = Localization.Resources.Bit_Branch_CodeErpEmpty;
		//Localization.Resources.Bit_Branch_CodeErpFormat%"Unacceptable symbols"
		public static string CodeErpFormat = Localization.Resources.Bit_Branch_CodeErpFormat;
    }

    public static class Product
    {
		//Localization.Resources.Bit_Product_MakatIsNotUnique%"Makat is not unique"
		public static string MakatIsNotUnique = Localization.Resources.Bit_Product_MakatIsNotUnique;
		//Localization.Resources.Bit_Product_MakatIsEmpty%"Can not be empty"
		public static string MakatIsEmpty = Localization.Resources.Bit_Product_MakatIsEmpty;
		//Localization.Resources.Bit_Product_MakatFormat%"Unacceptable symbols"
		public static string MakatFormat = Localization.Resources.Bit_Product_MakatFormat;
		//Localization.Resources.Bit_Product_PriceSaleFormat%"Incorrect format"
		public static string PriceSaleFormat = Localization.Resources.Bit_Product_PriceSaleFormat;
		//Localization.Resources.Bit_Product_PriceBuyFormat%"Incorrect format"
		public static string PriceBuyFormat = Localization.Resources.Bit_Product_PriceBuyFormat;
    }

    public static class InventProduct
    {
		//Localization.Resources.Bit_InventProduct_MakatIsEmpty%"Can not be empty"
		public static string MakatIsEmpty = Localization.Resources.Bit_InventProduct_MakatIsEmpty;
		//Localization.Resources.Bit_InventProduct_MakatFormat%"Unacceptable symbols"
		public static string MakatFormat = Localization.Resources.Bit_InventProduct_MakatFormat;
		//Localization.Resources.Bit_InventProduct_QuantityEditFormat%QuantityEditFormat
		public static string QuantityEditFormat = Localization.Resources.Bit_InventProduct_QuantityEditFormat;
		public static string QuantityInPackEditFormat = Localization.Resources.Bit_InventProduct_QuantityEditFormat;//Localization.Resources.Bit_InventProduct_QuantityInPackEditFormat;
		//Localization.Resources.Bit_InventProduct_QuantityOriginalFormat%QuantityOriginalFormat
		public static string QuantityOriginalFormat = Localization.Resources.Bit_InventProduct_QuantityOriginalFormat;
    }

    public static class Location
    {
		//Localization.Resources.Bit_Location_CodeExist%"Location with such code already exists"
		public static string CodeExist = Localization.Resources.Bit_Location_CodeExist;
		//Localization.Resources.Bit_Location_CodeEmpty%"Name can not be empty"
		public static string CodeEmpty = Localization.Resources.Bit_Location_CodeEmpty;
    }


}
