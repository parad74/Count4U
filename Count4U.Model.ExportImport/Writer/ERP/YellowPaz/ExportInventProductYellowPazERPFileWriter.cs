using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductYellowPazERPFileWriter : IExportInventProductFileWriter
	{
	public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{

//				  Record:			 old
//BranchCodeERP (col 1-5)
//Makat (col 7-20)
//Quantity Edit (col 22-27)

			//00966 00091201003983 000004

//			Record:
//Makat (col 1-13)
			//Qty (col 14-23)							   Quantity Format: 7.3 (without the dot)

//Const “         0         0" (col 24-43)
//CR/LF  (col 44)


			string erpCode = ERPNum;
			erpCode = erpCode.PadLeft(5, '0');
			string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit) * 1000).ToString();

			string makat = iturAnalyzes.Makat;
	
			string newRow =
			String.Format("{0,13}", makat) +	 //Makat (col 1-13)
			String.Format("{0,10}", quantityEdit) +	//Qty (col 14-23)							   Quantity Format: 7.3 (without the dot)
			"         0         0" +
			Environment.NewLine;							  //Const “         0         0" (col 24-43)
			sw.Write(newRow);								   //CR/LF  (col 44)
 		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		
//			Header:
//BranchCodeERP (col 8-10)
//Const “  00000001”  (col 11-20)
//Date “DD/MM/YYYY” (col 21-30)
//Const Space (col 31-37)
//BranchCodeERP (col 38-40)
//CR/LF  (col 41)
  //       109  0000000101/01/2016       109
		
			string erpCode = ERPNum;
			if (ERPNum.Length > 3) erpCode = ERPNum.Substring(0,3);
			string invDate = INVDate;
			
 			string header =	 "       " +
				String.Format("{0,-3}", erpCode) +//BranchCodeERP (col 8-10)
				 "  00000001" +
				String.Format("{0,-10}", invDate) +//Date “DD/MM/YYYY” (col 21-30)
				"       " +
				String.Format("{0,-3}", erpCode) +//BranchCodeERP (col 38-40)
			Environment.NewLine;												
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
