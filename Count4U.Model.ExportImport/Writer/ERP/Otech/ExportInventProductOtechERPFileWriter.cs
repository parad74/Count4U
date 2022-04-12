using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductOtechERPFileWriter : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
//			Record:
//Makat (col 1-15)
//Quantity sign (16)
//Quantity Edit (col 17-19) 

			//  7297485020972+  1
			string sign = "+";
			if(iturAnalyzes.QuantityEdit < 0)	sign = "-";
			string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit)).ToString();
			string newRow = String.Format("{0,15}", iturAnalyzes.Makat) +	 //Makat (col 1-15)
			sign +
			String.Format("{0,3}", quantityEdit) +
			Environment.NewLine;	
			sw.Write(newRow);

		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
//			Header:
//BranchCodeERP (col 1-3)
//Const Spaces (col 4-7)
//Const "H" (col 8-19)
//CR/LF  (col 34)
//007    HHHHHHHHHHHH
					
			string erpCode = ERPNum;
			if (ERPNum.Length > 3) { erpCode = ERPNum.Substring(0, 3); }	
			string header = String.Format("{0,-3}", erpCode) +	//BranchCodeERP (col 1-3)
				"    HHHHHHHHHHHH"+										 //Const "H" (col 8-19)
			Environment.NewLine;														//CR/LF  (col 34)					  ?
			sw.Write(header);

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
