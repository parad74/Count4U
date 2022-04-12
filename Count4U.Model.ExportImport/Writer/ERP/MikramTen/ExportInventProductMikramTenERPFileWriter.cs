using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductMikramTenERPFileWriter : IExportInventProductFileWriter
	{
			public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
	
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//Date Format (DD/MM/YY)
			//ERP Code Branch (Add leading zeros)
			//Item Code (Add leading zeros) – Use the Mask!
			//Barcode (Cut the last right char of the barcode and add leading zeros)
			//Quantity (Format: NNNNN.NNN)  add leading zeros to set the format
			//Record Sample:
			//21/10/12|0706|0000020500017|729000899802| 00002.00

			string quantityEdit = String.Format("{0:00000.00}", iturAnalyzes.QuantityEdit);
			string erpCode = ERPNum.LeadingZero4();
			string makat = iturAnalyzes.Makat.LeadingZero13();
			string barcode = "            ";

			//if (string.IsNullOrWhiteSpace(iturAnalyzes.Barcode) == false)
			//{
			//    if (iturAnalyzes.Barcode.Length > 0)
			//    {
			//        barcode = iturAnalyzes.Barcode.Substring(0, iturAnalyzes.Barcode.Length - 1);
			//        barcode = barcode.LeadingZero12();
			//    }
			//}

			string inBarcode = iturAnalyzes.Barcode;
			if (string.IsNullOrWhiteSpace(inBarcode) == false)
			{
				if (inBarcode.Length > 0)
				{
					string[] barcodes = inBarcode.Split(',');
					if (barcodes.Length > 0)
					{
						inBarcode = barcodes[0];
					}
					barcode = inBarcode.Substring(0, inBarcode.Length - 1);
					barcode = barcode.LeadingZero12();
				}
			}

			//INV Date (col 1-8)
			//ERP Code Branch (col 10-13)
			//Makat (col 15-27)
			//Barcode(29-40)
			//Quantity Edit (col 43-50)


			string newRow = INVDate + "|" +  											//INV Date (col 1-8)
				String.Format("{0,4}", erpCode) + "|" +  								//ERP Code Branch (col 10-13)
				String.Format("{0,13}", makat) + "|" + 									 //Makat (col 15-19)	
				String.Format("{0,12}", barcode) + "| " +								//Barcode(29-40)
				String.Format("{0,8}", quantityEdit);									//Quantity Edit (col 43-50) (5.2)
			sw.WriteLine(newRow);
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
