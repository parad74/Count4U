using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductNetPOSSuperPharmERPFileWriter : IExportInventProductFileWriter
	{
   		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//            Record:
			//Makat (col 1-13)
			//Quantity Edit (col 14-19)
			string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit)).ToString().LeadingZero6();
			string makat = iturAnalyzes.Makat.LeadingZero13();
			string newRow =
		   String.Format("{0,-13}", makat) +					// Makat (col 1-13)
		   String.Format("{0,-6}", quantityEdit);				//Quantity Edit (col 14-19)
			sw.WriteLine(newRow);
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddHeaderSum(StreamWriter sw, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			//Header:
			//Const“X” (col 1-13)
			//Total Makats (col 14-19)			//Count Distinct Makat
			//Total Quantity (col 20-32)	  // Sum Quantity
			//XXXXXXXXXXXXX0119180000000099834

			double totalQuantity1 = Convert.ToDouble(iturAnalyzesList.Sum(x => x.QuantityEdit));
			string totalQuantity = Convert.ToInt64(totalQuantity1).ToString().LeadingZero13();
			long totalMakat1 = iturAnalyzesList.LongCount();
			string totalMakat = totalMakat1.ToString().LeadingZero6();
			string header = String.Format("{0,13}", "XXXXXXXXXXXXX") +
			String.Format("{0,-6}", totalMakat) +
			String.Format("{0,-13}", totalQuantity);

			sw.WriteLine(header);
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
