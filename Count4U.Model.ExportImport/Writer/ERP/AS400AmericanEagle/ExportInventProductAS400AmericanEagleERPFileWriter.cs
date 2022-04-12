using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductAS400AmericanEagleERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{

		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//ERPBranchCode(col 8-10)
			//Makat (col 12-26)
			//Qty (col 44-51)

			//ERP Branch Code
			//Align to the left 
			//Align to the right 
			//Type- 5.3 without the dot

			//Record Sample:
			//	   012 127027010048                       15000
			double quantityEditDouble  = Convert.ToDouble(iturAnalyzes.QuantityEdit);
			int quantityEdit000 = (int)(quantityEditDouble * 1000);

			string quantityEdit = quantityEdit000.ToString().Trim();
			if (quantityEdit == "0") quantityEdit = "000";  //Type- 5.3 without the dot

			string erpCode = ERPNum.LeadingZero3();
			string makatOriginal = iturAnalyzes.Makat;  //Without Mask!

			//ERPBranchCode(col 8-10)
			//Makat (col 12-26)
			//Qty (col 44-51)

			string newRow = "       " +
				String.Format("{0,3}", erpCode) + " " +							//ERP Code Branch (col 8-10)
				String.Format("{0,-15}", makatOriginal) +							 //Makat  (col 12-26)
				"                 " +
				String.Format("{0,8}", quantityEdit);								//Quantity Edit col 44-51)
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
