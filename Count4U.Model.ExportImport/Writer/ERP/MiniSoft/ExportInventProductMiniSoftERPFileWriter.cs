using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductMiniSoftERPFileWriter : IExportInventProductFileWriter
	{

			public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//Quantity Edit (col 26-35)				QuantityEdit (Format: nnnnnnn.sss) 
			string quantityEdit = String.Format("{0:0.00}", iturAnalyzes.QuantityEdit);


			//DocType : 1-3 (Always "003")
			//ERPNum : 5-10
			//11
			//Makat : 12-29
			//30 - 46
			//Quantity : 47-55 (5.3)
										


//Const (col 1-6)			 "00001"
//Const (col 7-11)			  " 0001"
//Item Code (col 13-24)					  Makat (12 Digits max) 
//Quantity Edit (col 26-35)				QuantityEdit (Format: nnnnnnn.sss) 
//Date (col 37-49)							  Format: DD/MM/YYHH:MM	 HH:MM – always zero "00:00")
//Const (col 50-53)							   "    " (4 spaces)
//000001 0001 000031605519       6.00 30/12/1200:00     
//000001 0001 011543334118     5.00021/07/1100:00  
			string newRow = "000001 0001 " + 							// (col 1-6)+(col 7-11)   "00001" +  " 0001"
				String.Format("{0,-12}", iturAnalyzes.Makat) +		// (col 13-24)  Makat (12 Digits max) 
				" " +
				String.Format("{0,10}", quantityEdit) + 				//Quantity Edit (col 26-35)				QuantityEdit (Format: nnnnnnn.sss) 
				" " +
				INVDate +														    // Format: DD/MM/YYHH:MM	 HH:MM – always zero "00:00")
				"    "+																	//"    " (4 spaces)  
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
