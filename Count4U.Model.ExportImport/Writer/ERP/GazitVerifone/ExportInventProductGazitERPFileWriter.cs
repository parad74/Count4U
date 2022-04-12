using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductGazitERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			string quantityEdit = String.Format("{0:0.000}", iturAnalyzes.QuantityEdit);
			string erpCode = ERPNum.LeadingZero6();

			//DocType : 1-3 (Always "003")
			//ERPNum : 5-10
			//11
			//Makat : 12-29
			//30 - 46
			//Quantity : 47-55 (5.3)
			string newRow = "003 " + 									 //DocType : 1-3 (Always "003")
				String.Format("{0,-6}", erpCode) +					 //ERPNum : 5-10
				" " +																 //11
				String.Format("{0,-18}", iturAnalyzes.Makat) +	 //Makat : 12-29
				String.Format("{0,-17}", "") +							  //30 - 46
				String.Format("{0,9}", quantityEdit);				//Quantity : 47-55 (5.3)
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
