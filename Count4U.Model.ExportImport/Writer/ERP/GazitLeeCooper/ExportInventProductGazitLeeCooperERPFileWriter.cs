using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductGazitLeeCooperERPFileWriter : IExportInventProductFileWriter
	{


		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
			//NEW
			//Date (col 1-8) - Format DDMMYYYY		=8
			//Const : “ “												=1
			//ERP Branch Code (col 10-12) 				=3
			//Const : “   “										   =3
			//Makat (col 16-27)									 =12
			//Const : “              “									= 14
			//Quantity Edit (col 42-46)						 =5

			//Date: 15042018 (Date of Export)
			//Const : “ “
			//ERP Branch Code: 088
			//Const : “   “	
			//Makat: 812045210030
			//Const : “              “			
			//Quantity: 1

			//Record sample:
			//15042018 088   812045210030              1

			string erpCode = ERPNum.Trim();
			int len = erpCode.Length;
			if (len > 3) erpCode = ERPNum.Substring(len - 3, 3);
			erpCode = erpCode.PadLeft(3, '0');

			int quantityEditInt = (int)(iturAnalyzes.QuantityEdit);
			string quantityEdit = quantityEditInt.ToString();
			string makat = iturAnalyzes.Makat;			   //Without Mask!

			//Date (col 1-8) - Format DDMMYYYY		=8
			//Const : “ “												=1
			//ERP Branch Code (col 10-12) 				=3
			//Const : “   “										   =3
			//Makat (col 16-27)									 =12
			//Const : “              “									= 14
			//Quantity Edit (col 42-46)						 =5
			//	All columns are aligned to the left
			string newRow =
				INVDate +													//Date (col 1-8) - Format DDMMYYYY		=8
				 @" " +															  //Const : “ “		
				String.Format("{0,-3}", erpCode) + 				//ERP Branch Code (col 10-12) 		
				 @"   " +
				  String.Format("{0,-12}", makat) + 				//Makat (col 16-27)									 =12
				 @"              " +												 //  = 14
				 String.Format("{0,-5}", quantityEdit) +						//Quantity Edit (col 50-61)		
				Environment.NewLine;									//CR/LF  
			sw.Write(newRow);
			//15042018 088   812045210030              1
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
