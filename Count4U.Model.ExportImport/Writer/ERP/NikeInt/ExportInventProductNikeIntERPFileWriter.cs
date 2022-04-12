using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductNikeIntERPFileWriter : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{

//				  Record:
//BranchCodeERP (col 1-5)
//Makat (col 7-20)
//Quantity Edit (col 22-27)

			//00966 00091201003983 000004

			string erpCode = ERPNum;
			erpCode = erpCode.PadLeft(5, '0');
			string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit)).ToString();
			quantityEdit = quantityEdit.PadLeft(6, '0');
			//string makat = iturAnalyzes.Makat;
			//makat = makat.PadLeft(14, '0');
		
			string makatNumber = iturAnalyzes.MakatLong.ToString();
			if (makatNumber.Length > 14) { makatNumber = makatNumber.Substring(0, 14); }
			if (makatNumber.Length < 14) { makatNumber = makatNumber.PadLeft(14, '0'); }

			string newRow =   erpCode +	 //BranchCodeERP (col 1-5)
			 " " +
			String.Format("{0,14}", makatNumber) +	 //Makat (col 7-20)
			 " " +
			String.Format("{0,6}", quantityEdit) +
			Environment.NewLine;	
			sw.Write(newRow);
 		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
//			Header:
//Const“0” (col 1-5)
//Const“0" (col 7-14)
//Date (col 15-20)
//BranchCodeERP (col 22-27)
	//00000 00000000150601 000966
					
			string erpCode = ERPNum;
			erpCode = erpCode.PadLeft(6, '0');
			string invDate = INVDate;
			
 			string header = "00000 00000000" +
				String.Format("{0,-6}", invDate) +//Date (col 15-20)
				" " +
				String.Format("{0,-6}", erpCode) +	//(col 22-27)
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
