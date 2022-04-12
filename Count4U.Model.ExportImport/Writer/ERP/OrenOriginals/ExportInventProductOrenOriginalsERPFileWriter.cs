using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductOrenOriginalsERPFileWriter : IExportInventProductFileWriter
	{

		
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		//JAFORA_Counted_{0}
		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//OrenDocumentType											//Const "41" (2 chars)
			//ERP Branch Code												//(6 chars – space left)  //in ERPNum
			//INV Date															//Format DD/MM/YYYY
			//Reference Code 												//DDMMYYYY & Local Branch Code (10 chars – space right)  //param in separator
			//Local Branch Code											//(4 chars – space left) //in ERPNum
			//Item Code Partial (9 chars from item Code)	//(16 chars – space right)
			//Item Code Partial2 (char 10-the end)				//(6 chars – space right) 
			//Empty Fields 													//Const commas ",,,,,,,,"
			//Price (Const 1)													//Const "1.000" (12 chars – space left)
			//Quantity Edit 													//(7 chars – space left) 
			//Empty Fields 													//Const commas ",,"
			//Total Amount													//Price * Quantity Edit (12 chars – space left, Format: "xxx.000")

			string orenDocumentType = "41";

			string ERPBranchCode = "      ";
			string localBranchCode = "    ";
			string[] codes = ERPNum.Split(';');
			if (codes.Length > 0) ERPBranchCode = codes[0].PadLeft(6, ' ');
			if (codes.Length > 1)
			{
				localBranchCode = codes[1].TrimStart('0');
				localBranchCode = localBranchCode.PadLeft(4, ' ');
			}
			string invDate = INVDate;
			string referenceCode = separator.PadRight(10, ' ');
			string itemCodePartial = iturAnalyzes.Makat;
			string itemCodePartial2 = "";
			if (iturAnalyzes.Makat.Length >= 9)
			{
				itemCodePartial = iturAnalyzes.Makat.Substring(0, 9);
				itemCodePartial = itemCodePartial.PadRight(16, ' ');
				if (iturAnalyzes.Makat.Length >= 10)
				{
					itemCodePartial2 = iturAnalyzes.Makat.Substring(9);
					itemCodePartial2 = itemCodePartial2.PadRight(6, ' ');
				}
			}
			string emptyFields = ",,,,,,";
			string price = "1.000".PadLeft(12, ' '); ;
			string quantityEdit = Convert.ToInt32(iturAnalyzes.QuantityEdit).ToString();
			string quantityEdit1 = quantityEdit.PadLeft(7, ' ');
			string emptyFields1 = ",";
			string totalAmount = quantityEdit.PadLeft(8, ' ') + ".000";
			//string quantityEditString = String.Format("{0:0.##}", iturAnalyzes.QuantityEdit);

			string[] newRows = new string[] { orenDocumentType, ERPBranchCode, invDate, referenceCode, 
				localBranchCode, itemCodePartial,itemCodePartial2, emptyFields,  price, quantityEdit1, emptyFields1, totalAmount};
			string newRow = string.Join(",", newRows);
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
