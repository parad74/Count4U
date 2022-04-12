using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductLadyComfortERPFileWriter : IExportInventProductFileWriter
	{
		//public const string LTRMark = "\u200E";
		//public const string RTMark = "\u200F";
		//Encoding hebrewEncoding = Encoding.GetEncoding("Windows-1255");

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
			//string[] newRows = new string[] { iturAnalyzes.IturCode, iturAnalyzes.IturName, iturAnalyzes.Makat, iturAnalyzes.FamilyType, iturAnalyzes.ProductName,
			//	iturAnalyzes.FamilyExtra1, 	iturAnalyzes.QuantityEdit.ToString() , iturAnalyzes.PriceSale.ToString(), iturAnalyzes.IPValueStr10 };
			//string newRow = string.Join(separator, newRows);
			//sw.WriteLine(newRow);


			//byte[] bytes = hebrewEncoding.GetBytes(iturAnalyzes.ProductName);

			//string hebrewStringName = hebrewEncoding.GetString(bytes);

			string newRow = iturAnalyzes.IturCode +
			"," +
			iturAnalyzes.IturName +
			"," +
			iturAnalyzes.Makat +
			"," +
			iturAnalyzes.FamilyType +
			"," +
			iturAnalyzes.ProductName +
				  //" " +
				  //"[" +
				  //"תו" +
				  //"]" +
				  //" " +
				  //LTRMark + 			
			"," +
			iturAnalyzes.FamilyExtra1 +
			"," +
			iturAnalyzes.QuantityEdit.ToString() +
			"," +
			iturAnalyzes.PriceSale.ToString() +
			"," +
			iturAnalyzes.IPValueStr10 +
			Environment.NewLine;
			sw.Write(newRow);
 		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//string quantityEdit = String.Format("{0:00000.00}", iturAnalyzes.QuantityEdit);
			//string erpCode = ERPNum.LeadingZero4();
			//string makatOriginal = iturAnalyzes.Makat.LeadingZero5();  //Without Mask!

			//string barcode = "            ";
			////	if (string.IsNullOrWhiteSpace(iturAnalyzes.BarcodeOriginal) == false)
			//string inBarcode = iturAnalyzes.Barcode;
			//if (string.IsNullOrWhiteSpace(inBarcode) == false)
			//{
			//	if (inBarcode.Length > 0)
			//	{
			//		string[] barcodes = inBarcode.Split(',');
			//		if (barcodes.Length > 0)
			//		{
			//			inBarcode = barcodes[0];
			//		}
			//		barcode = inBarcode.Substring(0, inBarcode.Length - 1);
			//		barcode = barcode.LeadingZero12();
			//	}
			//}

	
			//string newRow = INVDate + "|" +  											//INV Date (col 1-8)
			//	String.Format("{0,4}", erpCode) + "|" +  								//ERP Code Branch (col 10-13)
			//	String.Format("{0,5}", makatOriginal) + "|" + 						 //Makat (col 15-19)
			//	String.Format("{0,12}", barcode) + "| " +								//Barcode(21-32)
			//	String.Format("{0,8}", quantityEdit);									//Quantity Edit (col 35-42) (5.2)
			//sw.WriteLine(newRow);

			}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddHeaderSum(StreamWriter sw, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}
		public void AddFooter(StreamWriter sw, long countRow, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddFooterSum(StreamWriter sw, long countRow, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}
		public void AddRowInventProduct(StreamWriter sw, InventProduct inventProduct, long countRow,
		   string ERPNum = "", string INVDate = "", string separator = ",", object argument = null)
		{
		}
	}
}
