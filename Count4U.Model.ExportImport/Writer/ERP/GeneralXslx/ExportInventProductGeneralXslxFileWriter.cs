using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductGeneralXslxFileWriter : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = "^", object argument = null)
		{
	
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow,
			string ERPNum = "", string INVDate = "", string separator = "^")
		{
			double quantityEdit = iturAnalyzes.QuantityEdit != null ? (double)iturAnalyzes.QuantityEdit : 0.0;

			string[] newRows = new string[3];
			newRows[0] = iturAnalyzes.Makat;
			newRows[1] = iturAnalyzes.ProductName;
			newRows[2] = quantityEdit.ToString();

			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddHeaderSum(StreamWriter sw, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		
			string[] newRows = new string[] { "Makat", "Item Name", "Quantity"};

			string newRow = string.Join("^", newRows);
			sw.WriteLine(newRow);
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
