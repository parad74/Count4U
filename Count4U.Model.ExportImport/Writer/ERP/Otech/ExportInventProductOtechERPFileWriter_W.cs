using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductOtechERPFileWriter_W : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//Makat (col 1-30 Aligned to right)
			//Quantity Edit (col 40-43 Aligned to right)
			//Const2 (col 45)
			//Record Counter (col 47-50 Aligned to right)

			//Sample: 
			//			                    312300226Y            1 2    7
//			Add all data rows that starts with "1" or "2" (in the item code)
//Add to the Header "w" to the Branch code
//15/06/14 900W 0 1 1
			if (iturAnalyzes.Makat.Length > 1)
			{
				string startNum = iturAnalyzes.Makat.Substring(0, 1);
				if (startNum == "1" || startNum == "2")
				{
					string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit)).ToString();
					string newRow = String.Format("{0,30}", iturAnalyzes.Makat) +	 //Makat (col 1-30 Aligned to right)
						//"*********" +
					"         " +
					String.Format("{0,4}", quantityEdit) +
					" 2 " +
					String.Format("{0,4}", countRow + 1);
					sw.WriteLine(newRow);
				}
			}

		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			//Header:
			//DateInventor (col 1-8)
			//Format DD/MM/YY 
			//Code ERP (col 10-12) – If CodeERP is greater then 3 chars, cut the first 3
			//Const0 (col 15)
			//Const1 (col 45)
			//Const 1 (col 50)

			//Sample: 
			//15/04/12 305  0                             1    1
			DateTime date = DateTime.Now;
			string dateString = parms.GetStringValueFromParm(ImportProviderParmEnum.InventorDate1);
			//date.ToString("dd") + @"/" + date.ToString("MM") + @"/" + date.ToString("yy");//ToString("dd-MM-yy");
			string erpCode = ERPNum;
			if (ERPNum.Length > 3) { erpCode = ERPNum.Substring(0, 3); }

			string header = String.Format("{0,8}", dateString) +	//Date (col 1-8)  Format DD/MM/YY
				" " +
				String.Format("{0,-3}", erpCode) +					 //Code ERP (col 10-12) – If CodeERP is greater then 3 chars, cut the first 3
				"W 0                             1    1";
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
