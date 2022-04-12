using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductRetalixNextERPFileWriter0 : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{

//            Const “001” (col 1-3)
//Sequantial Number (col 4-9)
//Makat (col 11-24)
//Qty (col 26-33)
//Sequantial Number in Itur (col 35-36)

			long seq1 = (int)(countRow / 60) + 1;
			int seq2 = (int)(countRow % 60);
			if (seq2 == 0)
			{
				seq2 = 60;
				seq1 = seq1 - 1;
			}
			string seqStr1 = seq1.ToString().LeadingZero6();
			string seqStr2 = seq2.ToString().LeadingZero2();
			string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit)).ToString().LeadingZero8();
			string makat = iturAnalyzes.Makat.LeadingZero14();

			string newRow = "001" +								 //            Const “001” (col 1-3)
			seqStr1 +												   //Sequantial Number  (col 4-9)
			" " + 
		   String.Format("{0,-13}", makat) +					// Makat (col 11-24)
		   " " + 
		   String.Format("{0,-8}", quantityEdit	+			//Quantity Edit (col 14-19)
			" " + 
			seqStr2	);
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
