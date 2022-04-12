using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductHashERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			string quantityEdit = String.Format("{0:0.00}", iturAnalyzes.QuantityEdit);
			int len = ERPNum.Trim().Length;
			string erpCode = "";
			if (len > 0) erpCode = ERPNum.Substring(len - 2, 2);
			erpCode = erpCode.TrimStart('0');
		
				//DocType : 1-2 (Const "14")
				//Item Code : 3-16   Align to the right (if smaller than 13 chars
				//EditQuantity : 17-24 (5.2)  Format "5.2" with leading spaces
				//Date Of Inventor : 25-30  DDMMYY
				//Branch Code ERP : 31-32  Cut Branch code to 2 digits (last 2 digits from the right+ change leading zero if exist to space)
			//14      BAG5851     1.00311213 2
			//14         284316   23.00310514 1
			string newRow = "14" + 											 //DocType : 1-2 (Const "14")
				String.Format("{0,13}", iturAnalyzes.Makat) +	 //Item Code : 3-15  Align to the right if smaller than 13 chars
				String.Format("{0,9}", quantityEdit) +				 //EditQuantity : 16-24 (5.2)  Format "5.2" with leading spaces
				INVDate +															//Date Of Inventor : 25-30  DDMMYY
				String.Format("{0,2}", erpCode);					    	 //ERPNum : 31-32   Cut Branch code to 2 digits (last 2 digits from the right+ change leading zero if exist to space)
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
