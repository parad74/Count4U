using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductAS400MangoERPFileWriter : IExportInventProductFileWriter
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

			//---------------
			//Branch ERP_Code (col 1-2)   Only 2 digits (Cut only the last 2 digit of branch code)
			//Item Code (col 3-14)
			//Const "    " (col 15-17)  3 Spaces - const
			//Quantity Edit (col 18-27)   Add leading zeros (format 8.2 without the dot)
			//Record Sample:
			//54110001242010   0000000100

			int len = ERPNum.Trim().Length;
			string erpCode = "";
			if (len > 2) erpCode = ERPNum.Substring(len - 2, 2);

			double quantityEditDouble  = Convert.ToDouble(iturAnalyzes.QuantityEdit);
			int quantityEdit00 = (int)(quantityEditDouble * 100);

			string quantityEdit = quantityEdit00.ToString().Trim();
			if (quantityEdit == "0") quantityEdit = "0000000000";
			else quantityEdit = quantityEdit00.ToString().PadLeft(10, '0');

			string makatOriginal = iturAnalyzes.Makat;  //Without Mask!

			//ERPBranchCode(col 8-10)
			//Makat (col 12-26)
			//Qty (col 44-51)

			string newRow = 
				String.Format("{0,2}", erpCode) + 				//Branch ERP_Code	 (col 1-2)   Only 2 digits (Cut only the last 2 digit of branch code)
				String.Format("{0,12}", makatOriginal) +					//Item Code (col 3-14)
				"   " +																		//Const "    " (col 15-17)  3 Spaces - const
				String.Format("{0,10}", quantityEdit) +					//Quantity Edit (col 18-27)   Add leading zeros (format 8.2 without the dot)
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
