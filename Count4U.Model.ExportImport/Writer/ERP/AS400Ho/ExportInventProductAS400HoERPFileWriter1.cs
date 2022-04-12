using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductAS400HoERPFileWriter1 : IExportInventProductFileWriter
	{


		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//INV Date
			//ERP Code Branch
			//Item Type (Only SN)
			//Makat 
			//Empty Field
			//Empty Field
			//Empty Field
			//Empty Field
			//Quantity Edit 
			string itemType = iturAnalyzes.UnitTypeCode.Trim().ToUpper();
			if (itemType == "SN")
			{
				string[] newRows = new string[] { INVDate, ERPNum, "K", iturAnalyzes.Makat, "", "", "", "",
				iturAnalyzes.QuantityEdit.ToString() };
				string newRow = string.Join(separator, newRows);
				sw.WriteLine(newRow);
			}
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
