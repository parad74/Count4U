using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductPriorityKedsShowRoomERPFileWriter : IExportInventProductFileWriter
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

//Record: (Each Record – 2 lines)
//First row:
//Type – Const1 (col 1)
//Makat-Part1 (col 3-9) -  Aligned to left (Cut first 7 chars from the Makat)
//Second row:
//Type – Const1 (col 2)
//Makat-Part2 (col 3-5) – Next 3 chars (Chars 8,9,10)
//Quantity Edit (col 8-12) - Aligned to right
			//Sample: 
//1 134548
//2 001      4
//1 134548
//2 208      3


			if (iturAnalyzes.IturCode != iturAnalyzes.Barcode) //добавление строки-заголовка для каждого Itur
			{
				string erpIturCode = iturAnalyzes.IturCode;
				if (iturAnalyzes.IturCode != null)
				{
					if (iturAnalyzes.IturCode.Length > 6) { erpIturCode = iturAnalyzes.IturCode.Substring(0, 6); }
				}

				string header = "0 " +
					String.Format("{0,8}", INVDate) +				//Date (col 3-10)  Format DD/MM/YY
					" " +
					String.Format("{0,6}", erpIturCode); 					 //Itur CodeERP (col 12-17) – attach to the right 
				sw.WriteLine(header);
			}
 
			string makat = iturAnalyzes.Makat;
			string makat1 = iturAnalyzes.Makat;
			string makat2 ="";
			if (makat.Length >= 7) {
				makat1 = makat.Substring(0, 7);
				//int len = makat.Length - 6;
				if (makat.Length >= 10)
				{
					makat2 = makat.Substring(7, 3);
				}
			}

			string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit)).ToString();

			string newRow1 = "1 " +			  //Type – Const1 (col 1)
			String.Format("{0,-6}", makat1); 	 //Makat-Part1 (col 3-8) -  Aligned to left (Cut first 6 chars from the Makat)

			string newRow2 = "2 " +			  //Type – Const1 (col 2)
			String.Format("{0,3}", makat2) + 	 //Makat-Part2 (col 3-5) – Next 3 chars (Chars 7,8,9)
			"  " +
			String.Format("{0,5}", quantityEdit);		 //Quantity Edit (col 8-12) - Aligned to right

	
			sw.WriteLine(newRow1);
			sw.WriteLine(newRow2);

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

//            Type - Const0 (col 1)
//Date (col 3-10) - Format DD/MM/YY 
//Code ERP (col 12-15) – attach to the right
			//Sample: 
			//0 28/01/13  900 
			//=======
			//string erpCode = ERPNum;
			//if (ERPNum.Length > 3) { erpCode = ERPNum.Substring(0, 3); }

			//string header = "0 " +
			//	String.Format("{0,8}", INVDate) +				//Date (col 3-10)  Format DD/MM/YY
			//	" " +
			//	String.Format("{0,6}", erpCode); 					 //Itur CodeERP (col 12-17) – attach to the right 
			//sw.WriteLine(header);

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
