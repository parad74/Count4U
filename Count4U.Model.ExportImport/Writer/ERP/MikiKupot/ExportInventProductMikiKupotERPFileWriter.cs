using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductMikiKupotERPFileWriter : IExportInventProductFileWriter
	{
  		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			string quantityEdit = String.Format("{0:00000.00}", iturAnalyzes.QuantityEdit);
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
			//Quantity Edit (col 17-24)              QuantityEdit (Format: nnnnnn.ss) 		17-24  (5.2)
			//000000000004058500012.00
			string makat = iturAnalyzes.Makat.LeadingZero16();
			string newRow =
				String.Format("{0,-16}", makat) +	 //Makat : 1-16
				String.Format("{0,8}", quantityEdit)+	//		17-24  (5.2)//old Quantity : 25-33 (5.3)
			Environment.NewLine;										//CR/LF  
			sw.Write(newRow);

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
