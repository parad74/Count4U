using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductPriorityAldoERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
	
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ";")
		{
			//IdentifiantPriority	0			   makat
			//ברקוד			1						   barcode
			//פריט תאור	2						  name = style +" " + colorCode + " " + size 
			//Style				3						 style
			//colorid			4						  colorCode
			//unitsize			5							  size
			//Total Count (סכימה של הכמות שנספרה)		6			quantityEdit

			long quantityEdit = Convert.ToInt64(iturAnalyzes.QuantityEdit);
			string quantityEditStr = quantityEdit.ToString();
			if (String.IsNullOrWhiteSpace(quantityEditStr) == true) quantityEditStr = "0";

			string makat = iturAnalyzes.Makat;
			string barcode = iturAnalyzes.Barcode;
			if (String.IsNullOrWhiteSpace(barcode) == true) barcode = makat;
			string name = iturAnalyzes.ProductName;
			string[] names = name.Split(" ".ToCharArray());
			string style = "";
			string color =  "";
			string size =  "";
			if (names.Length > 0) style = names[0];
			if (names.Length > 1) color = names[1];
			if (names.Length > 2) size = names[2];
			string[] newRows = new string[] { makat, barcode, name, style, color, size, quantityEditStr };
			string newRow = string.Join(",", newRows);
			sw.WriteLine(newRow);
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			string[] newRows = new string[] { @"IdentifiantPriority", @"ברקוד", @"פריט תאור ", @"Style ", @"colorid ", @"unitsize	", @"Total Count (סכימה של הכמות שנספרה)   " };

			string newRow = string.Join(",", newRows);
			sw.WriteLine(newRow);

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
