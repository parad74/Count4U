using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductAS400HonigmanERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{

		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//ERPBranchCode(col  1-3)
			//Makat (col 4-9)
			//Qty (col 16-20)
			//Date (21-28)

			//ERP Branch Code
			//Align to the left 
			//Align to the right (add leading zeros – integers only)
			//Format YYYYMMDD

			//Record Sample:
			//	145200003      0000520150610
			double quantityEditDouble  = Convert.ToDouble(iturAnalyzes.QuantityEdit);
			int quantityEditInt = (int)(quantityEditDouble);

			string quantityEdit = quantityEditInt.ToString().Trim();
			quantityEdit = quantityEdit.PadLeft(5, '0');
			if (quantityEdit == "0") quantityEdit = "00000";  

			string erpCode = ERPNum.LeadingZero3();
			string makatOriginal = iturAnalyzes.Makat;  //Without Mask!

			//ERPBranchCode(col 8-10)
			//Makat (col 12-26)
			//Qty (col 44-51)

			string newRow = 	String.Format("{0,3}", erpCode) +				//ERP Code Branch (col1-3)
				String.Format("{0,-6}", makatOriginal) +							 //Makat  (col 4-9)
				"      " +
				String.Format("{0,5}", quantityEdit) +								//Quantity Edit col 16-20)
				INVDate;																			//YYYYMMDD
			sw.WriteLine(newRow);
			//Environment.NewLine;										//CR/LF  
			//sw.Write(newRow);

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
