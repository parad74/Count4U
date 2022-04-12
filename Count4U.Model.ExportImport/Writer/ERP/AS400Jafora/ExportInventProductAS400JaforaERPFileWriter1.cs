using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductAS400JaforaERPFileWriter1 : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		//"JAFORA_Not_Counted_{0}"
		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			// в ERPNum UnitTypeCode
			// в INVDate  ddMM
			// в separator  YYYY
			//	Field1: DATE								Format: "DDMM"
			//Field2: YEAR								Format: "YYYY"
			//Field3: SITE (CONST)				"3000"
			//Field4: SUB SITE (CONST)		"3101"	
			//Field5: Item Code						Original item code
			//Field6: Quantity Edit					Total quantity for that item
			//Field7: UnitTypeCode					Item Unit Type (from catalog)
			//Field8: Section Code					SectionCode (from catalog)
			string quantityEditString = String.Format("{0:0.##}", iturAnalyzes.QuantityEdit);
			string[] newRows = new string[] { INVDate, separator, "3000", "3101", iturAnalyzes.Makat, quantityEditString, iturAnalyzes.UnitTypeCode, iturAnalyzes.SectionCode };
			string newRow = string.Join(",", newRows);
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
