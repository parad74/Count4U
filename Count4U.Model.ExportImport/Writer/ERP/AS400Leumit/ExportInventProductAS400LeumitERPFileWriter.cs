using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductAS400LeumitERPFileWriter : IExportInventProductFileWriter
	{


		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			
			//Makat (col 1-6)
			//Quantity Edit - Complete (col 7-13)		Integers (Left side of the decimal dot)
			//Quantity Edit - Fraction (col 14-20)   Fraction (Right side of the ecimal dot)
			// 10508      2      0
			// 10509      4      0
			//  1051      0      0

			string iturAnalyzesMakat = iturAnalyzes.Makat;
			if (iturAnalyzesMakat.Length > 16) { iturAnalyzesMakat = iturAnalyzes.Makat.Substring(0, 16); }

			long quantityEdit = Convert.ToInt64(iturAnalyzes.QuantityEdit);
			int quantityInPackEdit = iturAnalyzes.QuantityInPackEdit;
			
			string quantityEditStr = quantityEdit.ToString();
			if (quantityEditStr.Length > 8) { quantityEditStr = quantityEditStr.Substring(0, 8); }
			if (String.IsNullOrWhiteSpace(quantityEditStr) == true) quantityEditStr = "0";

			string quantityInPackEditStr = quantityInPackEdit.ToString();
			if (quantityInPackEditStr.Length > 8) { quantityInPackEditStr = quantityInPackEditStr.Substring(0, 8); }
			if (String.IsNullOrWhiteSpace(quantityInPackEditStr) == true) quantityInPackEditStr = "0";

			//01007804,20130317,93598           ,       1,      45 
			//ERP Code Branch (col 1-8)		  Add leading zeros
			//INV Date (col 10-17)				 Date format: YYYYMMDD
			//Makat (col 19-34)					   Attach to the left
			//Quantity Edit – Complete (col 36-43)		Attach to the right
			//Quantity Edit – Partial (col 45-52)			Attach to the right
			//00001160,20112307,   0000000000223,75      ,0       
			//01007804,20130317,00102           ,       3,       0			  //+
			string newRow =
				String.Format("{0,8}", ERPNum) +					 //ERP Code Branch (col 1-8)
				"," +
				String.Format("{0,8}", INVDate) +					 //INV Date (col 10-17)
				"," +
				String.Format("{0,-16}", iturAnalyzesMakat) +	 //Makat (col 19-34)
				"," +
				String.Format("{0,8}", quantityEditStr) +			//Quantity Edit – Complete (col 36-43)		Integers (Left side of the decimal dot)
				"," +
				String.Format("{0,8}", quantityInPackEditStr) +		//Quantity Edit – Partial (col 45-52)   Fraction (Right side of the ecimal dot)
			Environment.NewLine;										//CR/LF  
			sw.Write(newRow);

			//sw.WriteLine(newRow);
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddHeaderSum(StreamWriter sw, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddFooter(StreamWriter sw, long countRow, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
//Footer:
			//ERP Code Branch (col 1-8)							 Add leading zeros
			//INV Date (col 10-17)									Date format: YYYYMMDD
			//Constant "999999999999999 " (col 19-34)			  "9" = Col 19-33 & " " = Col 34
			//Total Data lines (col 36-43)									Attach to the right
			//Constant "       0" (col 45-52)									" " = Col 45-51 & "0" = Col 52

			//	 01007804,20130317,999999999999999 ,    1103,       0 

			string newRow =
				String.Format("{0,8}", ERPNum) +					 //ERP Code Branch (col 1-8)
				"," +
				String.Format("{0,8}", INVDate) +					 //INV Date (col 10-17) Date format: YYYYMMDD
				",999999999999999 ," +
				String.Format("{0,8}", countRow) +
				",       0";		
			sw.WriteLine(newRow);

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
