using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductYarpaERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
			throw new NotImplementedException();
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//Makat (col 1-6)
			//Quantity Edit - Complete (col 7-13)		Integers (Left side of the decimal dot)
			//Quantity Edit - Fraction (col 14-20)   Fraction (Right side of the ecimal dot)
			// 10508      2      0
			// 10509      4      0
			//  1051      0      0

			string iturAnalyzesMakat = iturAnalyzes.Makat;
			if (iturAnalyzesMakat.Length > 6) { iturAnalyzesMakat = iturAnalyzes.Makat.Substring(0, 6); }

			long quantityEdit = Convert.ToInt64(iturAnalyzes.QuantityEdit);
			int quantityInPackEdit = iturAnalyzes.QuantityInPackEdit;
			
			string quantityEditStr = quantityEdit.ToString();
			if (quantityEditStr.Length > 7) { quantityEditStr = quantityEditStr.Substring(0, 7); }
			if (String.IsNullOrWhiteSpace(quantityEditStr) == true) quantityEditStr = "0";

			string quantityInPackEditStr = quantityInPackEdit.ToString();
			if (quantityInPackEditStr.Length > 7) { quantityInPackEditStr = quantityInPackEditStr.Substring(0, 7); }
			if (String.IsNullOrWhiteSpace(quantityInPackEditStr) == true) quantityInPackEditStr = "0";

			string newRow =
				String.Format("{0,6}", iturAnalyzesMakat) +	 //Makat (col 1-6)
				String.Format("{0,7}", quantityEditStr) +			//Quantity Edit - Complete (col 7-13)		Integers (Left side of the decimal dot)
				String.Format("{0,7}", quantityInPackEditStr);			//Quantity Edit - Fraction (col 14-20)   Fraction (Right side of the ecimal dot)
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
