using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductWarehouseXslxFileWriter : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
	
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow,
			string ERPNum = "", string INVDate = "", string separator = ",")
		{
			int countInParentPack = iturAnalyzes.CountInParentPack;
			double quantityEdit = iturAnalyzes.QuantityEdit != null ? (double)iturAnalyzes.QuantityEdit : 0.0 ;
			double price = iturAnalyzes.Price;
			double quantityEditXcountInParentPack = countInParentPack * quantityEdit;
			double balanceQuantityERP = iturAnalyzes.QuantityOriginalERP;
			double quantityDifference = quantityEditXcountInParentPack - balanceQuantityERP;
			double inventoryValue = quantityEditXcountInParentPack* price;
			double ERPValue = balanceQuantityERP * price;
			double valueDifference = inventoryValue - ERPValue;

			string[] newRows = new string[11];
			newRows[0] = iturAnalyzes.Makat;
			newRows[1] = iturAnalyzes.ProductName;
			newRows[2] = iturAnalyzes.Price.ToString();
			newRows[3] = countInParentPack.ToString();

			newRows[4] = quantityEdit.ToString();
			newRows[5] = quantityEditXcountInParentPack.ToString();
			newRows[6] = balanceQuantityERP.ToString();
			newRows[7] = quantityDifference.ToString();

			newRows[8] = inventoryValue.ToString();
			newRows[9] = ERPValue.ToString();
			newRows[10] = valueDifference.ToString();

			//string[] newRows = new string[] { iturAnalyzes.Makat, iturAnalyzes.ProductName, iturAnalyzes.Price.ToString(), 
			//	countInParentPack.ToString(), 
			//	quantityEdit.ToString(), quantityEditXcountInParentPack.ToString(), balanceQuantityERP.ToString(),  quantityDifference.ToString(),
			//	inventoryValue.ToString(), ERPValue.ToString(),  valueDifference.ToString()};

			string newRow = string.Join(",", newRows);
			sw.WriteLine(newRow);
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddHeaderSum(StreamWriter sw, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		
			string[] newRows = new string[] { "Item Code", "Item Name", "Unit Price", 
				 "Quantity In Pack",	 
				 "Total Packs Counted", "Total Units Counted",   "Units Quantity in ERP", "Quantity Difference",
				"Inventory Value", "ERP Value",  "Value Difference ERP"};

			string newRow = string.Join(",", newRows);
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
