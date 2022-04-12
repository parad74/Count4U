using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductAvivPOSERPFileWriter : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			string quantityEdit = String.Format("{0:00000.000}", iturAnalyzes.QuantityEdit);
			//string quantityEdit = String.Format("{####0.000}", iturAnalyzes.QuantityEdit);
			//string erpCode = ERPNum.LeadingZero6();
			//OLD
			//DocType : 1-3 (Always "003")
			//ERPNum : 5-10
			//11
			//Makat : 12-29
			//30 - 46
			//Quantity : 47-55 (5.3)

			// NEW 
			//Item Code (col 1-16)
			//Quantity Edit (col 25-33)              QuantityEdit (Format: nnnnn.sss) 		25-29 30 31-33	  (5.3)
			//364957                  00002.000
			string newRow =
				String.Format("{0,-16}", iturAnalyzes.Makat) +	 //Makat : 1-16
				@"        " +
				String.Format("{0,9}", quantityEdit);				//Quantity : 25-33 (5.3)
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
