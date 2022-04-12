using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductMiniSoftERPFileWriter2 : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
			throw new NotImplementedException();
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			string quantityEdit = String.Format("{0:0000000.00}", iturAnalyzes.QuantityEdit);  // ( nnnnnnn.ss)
			string erpCode = ERPNum.LeadingZero4();

//Const (col 1-6)
//BranchERPCode (col 8-11)
//Item Code (col 13-24)
//Quantity Edit (col 26-35)

//"000003"
//"0004" (add leading zeros)
//Makat (12 Digits max) 
//QuantityEdit (Format: nnnnnnn.ss) 

//Records Sample:
//000003 0004 704005198989 0000001.00

			string newRow = "000003 " + 									 //Const (col 1-6)   "000003"
				String.Format("{0,-4}", erpCode) +					 //BranchERPCode (col 8-11)  (add leading zeros)
				" " +																 //11
				String.Format("{0,-12}", iturAnalyzes.Makat) +	 //Makat (12 Digits max) 
				" " +
				String.Format("{0,9}", quantityEdit)+				//QuantityEdit  (col 26-35)(Format: nnnnnnn.ss) 
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
