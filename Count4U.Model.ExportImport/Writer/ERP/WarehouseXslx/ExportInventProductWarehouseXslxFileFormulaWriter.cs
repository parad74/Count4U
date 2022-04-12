using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductWarehouseXslxFileFormulaWriter : IExportInventProductFileWriter
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
			 newRows[0] =iturAnalyzes.Makat;																		 //1
			 newRows[1] =iturAnalyzes.ProductName;																//2
			newRows[2] =iturAnalyzes.Price.ToString();															//3
			newRows[3] =countInParentPack.ToString();															//4

			newRows[4] =quantityEdit.ToString();																	 //5
			newRows[5] = "F=RC[-2]*RC[-1]";//quantityEditXcountInParentPack.ToString();	//6	 = 4*5	"F=RC[-2]*RC[-1]"
			newRows[6] = balanceQuantityERP.ToString();														//7
			newRows[7] = "F=RC[-2]-RC[-1]";//quantityDifference.ToString()						//8	= 6-7	"F=RC[-2]-RC[-1]"

			newRows[8] = "F=RC[-6]*RC[-3]"; // inventoryValue.ToString();							//9  = 3*6		"F=RC[-6]*RC[-3]"
			newRows[9] = "F=RC[-7]*RC[-3]";//ERPValue.ToString();										//10  = 3*7		"F=RC[-7]*RC[-3]"
			newRows[10] = "F=RC[-2]-RC[-1]";/*valueDifference.ToString()*/					   //11 = 9-10		"F=RC[-2]-RC[-1]"

			//{ iturAnalyzes.Makat, iturAnalyzes.ProductName, iturAnalyzes.Price.ToString(), 
			//	countInParentPack.ToString(), 
			//	quantityEdit.ToString(), quantityEditXcountInParentPack.ToString(), balanceQuantityERP.ToString(),  string.Empty /*quantityDifference.ToString()*/,
			//	inventoryValue.ToString(), ERPValue.ToString(),  string.Empty/*valueDifference.ToString()*/};
			  //
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
