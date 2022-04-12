using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductAS400MegaERPFileWriter : IExportInventProductFileWriter
	{


		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
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

				//NEW
			//BranchCodeERP (col 1-5)				 Add leading zeros
			//Inventory Date (col 6-13)
			//Makat (col 14-26)									  Add leading zeros
			//Itur (col 27-34)
			//Const (col 33-37)													 Const: "00011"
			//Quantity expected (col 38-49)							   Const: "00011"	Const: "00000001.000"
			//Quantity Edit (col 50-61)										  Format: 8.3 (with decimal point)
			//Const (col 54-56)													   Const: "01P"

//Record Sample:
//003532016112272900104727650011900006400000009.00000000006.00001P


			string erpCode = ERPNum.Trim();
			int len = erpCode.Length;
			if (len > 5) erpCode = ERPNum.Substring(len - 5, 5);
			erpCode = erpCode.PadLeft(5, '0');

			double quantityEditDouble = Convert.ToDouble(iturAnalyzes.QuantityEdit);
			//int quantityEdit00 = (int)(quantityEditDouble * 100);

			//string quantityEdit = quantityEdit00.ToString().Trim();
			//if (quantityEdit == "0") quantityEdit = "0000000000";
			//else quantityEdit = quantityEdit00.ToString().PadLeft(10, '0');

			string quantityEdit = quantityEditDouble.ToString("F3"); //String.Format("{0:0.###}", quantityEditDouble);
			quantityEdit = quantityEdit.PadLeft(12, '0');

			string makatOriginal = iturAnalyzes.Makat;  //Without Mask!
			makatOriginal = makatOriginal.PadLeft(13,'0');

			//Inventory Date (col 6-13)
			//Makat (col 14-26)									  Add leading zeros
			//Itur (col 27-34)
			//Const (col 33-37)													 Const: "00011"
			//Quantity expected (col 38-49)							   Const: "00011"	Const: "00000001.000"
			//Quantity Edit (col 50-61)										  Format: 8.3 (with decimal point)
			//Const (col 54-56)													   Const: "01P"
			string newRow =
				String.Format("{0,5}", erpCode) + 				//BranchCodeERP (col 1-5)				 Add leading zeros
				INVDate	+															//Inventory Date (col 6-13)
				makatOriginal +													 //Makat (col 14-26)									  Add leading zeros
				iturAnalyzes.IturCode + 										//Itur (col 27-34)
				"00011"+ 																// Const: "00011"
				"00000001.000" +													//Const: "00000001.000"
				quantityEdit +														//Quantity Edit (col 50-61)										  Format: 8.3 (with decimal point)
				"01P"	 +																//Const: "01P"
				Environment.NewLine;										//CR/LF  
			sw.Write(newRow);
			//003532016112272900104727650011900006400000009.00000000006.00001P
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
	
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
