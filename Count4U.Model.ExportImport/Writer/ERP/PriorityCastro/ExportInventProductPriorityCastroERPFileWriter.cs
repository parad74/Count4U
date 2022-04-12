using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductPriorityCastroERPFileWriter : IExportInventProductFileWriter
	{


		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{

		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
//Inventor Date					Format : DD/MM/YYYY
//ERPBranchCode					3 digits (if needed use prefix zero's)
//Dummy1						   Const '0'
//Item Code					   Makat
//Warehouse					 Const 'Goods'
//Dummy2						 Const '0'
//Dummy3						 Const '' (empty)
//Dummy4						Const '' (empty)
//Qty								Quantity Edit
//Dummy5						Const '0'
//Dummy6					   Const '0'

				string erpCode = ERPNum;
				if (ERPNum.Length >3) erpCode = ERPNum.Substring(0,3);
				erpCode = erpCode.PadLeft(3, '0');

				string[] newRows = new string[] {INVDate, erpCode, "0", 
					iturAnalyzes.Makat, 	"Goods", "0", "", "",
					 iturAnalyzes.QuantityEdit.ToString() , "0", "0"};
			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);
		}

		//public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		//{
		//	//ERPBranchCode(col 1-4)	ERP Branch Code(Align to the left)
		//	//Barcode (col 5-17)Align to the left  (add spaces)– check that its 13 digits !
		//	//Qty (col 18-25)Align to the right (add leading zeros)
		//	//Const (col 26-29) '0011'

		//	int quantityEditInt = (int)iturAnalyzes.QuantityEdit;
		//	string quantityEdit = quantityEditInt.ToString().PadLeft(8,'0');

		//	string erpCode = ERPNum;
		//	if (ERPNum.Length >4) erpCode = ERPNum.Substring(0,4);

		//	string barcode = "          ";
		//	string inBarcode = iturAnalyzes.Barcode;
		//	if (string.IsNullOrWhiteSpace(inBarcode) == false)
		//	{
		//		if (inBarcode.Length > 0)
		//		{
		//			string[] barcodes = inBarcode.Split(',');
		//			if (barcodes.Length > 0)
		//			{
		//				barcode = barcodes[0];
		//			}
		//			//barcode = inBarcode.Substring(0, inBarcode.Length - 1);
		//			if (barcode.Length > 13) barcode.Substring(0, 13);
		//			//barcode = barcode.PadLeft(13,' ');//Align to the right  (add spaces)
		//		}
		//	}
		//	string newRow =
		//		String.Format("{0,-4}", erpCode) +							//ERP Code Branch (col 1-4)(Align to the left)
		//		String.Format("{0,-13}", barcode) +						//Barcode(5-17)Align to the left  (add spaces)– check that its 13 digits !
		//		String.Format("{0,8}", quantityEdit) + 					//Quantity Edit (col 18-25)
		//		"0011";
		//	sw.WriteLine(newRow);

		//	}

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
