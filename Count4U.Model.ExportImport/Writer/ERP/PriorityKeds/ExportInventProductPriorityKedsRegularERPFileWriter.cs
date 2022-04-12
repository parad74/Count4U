using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductPriorityKedsRegularERPFileWriter : IExportInventProductFileWriter
	{
	 		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
//            Type – Const1 (col 1)
//Makat (col 2-20) -  Aligned to left
//Quantity Edit (col 24-28) - Aligned to right
//Const1 (col 69)
//Sample:
//1 18812602104              1                                        1
//1 18P13701310              2                                        1
//1 18P13701318              1"                                        1"


			string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit)).ToString();
			string newRow = "1 " +										//	 Type – Const1 (col 1)
				String.Format("{0,-18}", iturAnalyzes.Makat) +	 //Makat (col 2-20) -  Aligned to left
			"   " +
			String.Format("{0,5}", quantityEdit) +					//Quantity Edit (col 24-28) - Aligned to right
			"                                        1";						   //Const1 (col 69)
			
			sw.WriteLine(newRow);
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			//old
			//Header:
			//Date (col 1-8)
			//Format DD/MM/YY 
			//Code ERP (col 10-12) – If CodeERP is greater then 3 chars, cut the first 3
			//Const0 (col 15)
			//Const1 (col 45)
			//Const 1 (col 50)

			//Sample: 
			//15/04/12 305  0                             1    1
			/*DateTime date = DateTime.Now;
			string dateString =
			date.ToString("dd") + @"/" + date.ToString("MM") + @"/" + date.ToString("yy");//ToString("dd-MM-yy");
			string erpCode = ERPNum;
			if (ERPNum.Length > 3) { erpCode = ERPNum.Substring(0, 3); }

			string header = String.Format("{0,8}", dateString) +	//Date (col 1-8)  Format DD/MM/YY
				" " +
				String.Format("{0,-3}", erpCode) +					 //Code ERP (col 10-12) – If CodeERP is greater then 3 chars, cut the first 3
				"  0                             1    1";
			sw.WriteLine(header);*/

// type - Const0 (col 1)
//Date (col 3-10) - Format DD/MM/YY 
//Code ERP (col 12-15) – attach to the right
//Branch Name (17-27) – If greater – cut	  11
//Const 1 (col 49)
			//Sample:
			//0 21/02/13  061 …‡ keds                        1

			string erpCode = ERPNum;
			if (ERPNum.Length > 3) { erpCode = ERPNum.Substring(0, 3); }

			string branchName = parms.GetStringValueFromParm(ImportProviderParmEnum.BranchName);
			if (branchName.Length > 10) { branchName = branchName.Substring(0, 10); }

			string header = "0 " +
				String.Format("{0,8}", INVDate) +	                 //Date (col 3-10)  Format DD/MM/YY
				" " +
				String.Format("{0,4}", erpCode) +					 //Code ERP (col 12-15) – If CodeERP is greater then 3 chars, cut the first 3
				" " +
				String.Format("{0,10}", branchName) +			 //Branch Name (17-27) – If greater – cut	  10
				"                      1";
			sw.WriteLine(header);
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
