using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductNibitERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "",
			string separator = ",", object argument = null)
		{
			//string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit)).ToString().LeadingZero7();
			////Const “2” (col 1)
			////Makat (col 2-14)
			////Qty (col 15-21)
			////Const “00000000000”  (col 22-32)
			//string newRow = "2" +													//Const “2” (col 1)
			//String.Format("{0,13}", iturAnalyzes.Makat) +				//Makat (col 2-14)
			//String.Format("{0,7}", quantityEdit) +							//Qty (col 15-21)
			//"00000000000" +														//Const “00000000000”  (col 22-32)
			//Environment.NewLine;												//CR/LF  (col 33)
			//sw.Write(newRow);

		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			long rest1000 = countRow % 999;
			if (countRow <= 999) rest1000 = 0;

			if (iturAnalyzes.IturCode != iturAnalyzes.Barcode // переход между Itur и надо вставить загаловок для нового итура (в Barcode хранится IturCode предыдущей записи в списке отсортированном по IturCode)
				|| rest1000 == 1)  //или началась новая 1000 row
			{
				//Header for each Itur
				//	Itur Code (col 1-4)				                    //Itur Suffix
				//Const "0101" (col 5-8)								//"0101"
				//Counter of Items In Itur (col 9-11)		//Const "000" 
				//Inventory Date (col 12-19)Date Format: //YYYYMMDD
				//Inventory Time (col 20-23)TIME Format: //HHMM
				//BranchCode ERP (col 24-28)Add					// Leading zeros
				//Const "00000000" (col 29-36)						//"00000000"
				//Const "+" (col 37)											//"+"
				string _iturCode = "0000";
				if (iturAnalyzes.IturCode.Length >= 8)
				{
					_iturCode = iturAnalyzes.IturCode.Substring(4, 4);
				}

				if (rest1000 == 1)
				{
					_iturCode = Int2Letter(_iturCode);
				}

				string invData = "00000000";
				string invTime = "0000";
				if (INVDate.Length >= 12)
				{
					//dt.ToString("yyyyMMdd") + dt.ToString("HHmm");
					invData = INVDate.Substring(0, 8);  //Inventory Date (col 12-19)Date Format: //YYYYMMDD
					invTime = INVDate.Substring(8, 4);  //Inventory Time (col 20-23)TIME Format: //HHMM
				}
				string erpCode = ERPNum;
				if (ERPNum.Length > 5)
				{
					erpCode = ERPNum.Substring(0, 5);
				}

				erpCode = erpCode.PadLeft(5, '0');
				
				//header
				string newRow1 = _iturCode +							//	Itur Code (col 1-4)				                    //Itur Suffix
				"0101000" +														//Const "0101" (col 5-8)								//"0101"
																						//Counter of Items In Itur (col 9-11)		//Const "000" 
				invData +															//Inventory Date (col 12-19)Date Format: //YYYYMMDD
				invTime +															//Inventory Time (col 20-23)TIME Format: //HHMM
				erpCode +														//BranchCode ERP (col 24-28)Add					// Leading zeros
				"00000000+" +													//Const "00000000" (col 29-36)						//"00000000"				//Const "+" (col 37)											//"+"
				Environment.NewLine;										//CR/LF  (col 33)
				sw.Write(newRow1);
			}

			//-------------
			string iturCode = "0000";
			if (iturAnalyzes.IturCode.Length >= 8)
			{
				iturCode = iturAnalyzes.IturCode.Substring(4, 4);
			}
			string makat = iturAnalyzes.Makat.PadLeft(14,'0');

			double quantityEditDouble = Convert.ToDouble(iturAnalyzes.QuantityEdit);
			int quantityEdit00 = (int)(quantityEditDouble * 100);

			string quantityEdit = quantityEdit00.ToString().Trim();
			if (quantityEdit == "0") quantityEdit = "00";
			quantityEdit = quantityEdit.PadLeft(8, '0');

			string row = "***";
			if (countRow <= 999)
			{
				row = countRow.ToString().PadLeft(3, '0');
			}
			else
			{
				row = rest1000.ToString().PadLeft(3, '0');
				iturCode = Int2Letter(iturCode);
			}
			//			Record:
			//Itur Code (col 1-4)									Itur Suffix
			//Const "0101" (col 5-8)									
			//Counter of Items In Itur (col 9-11)**	     Count from 001-999
			//Makat (col 12-25)									Add Leading zeros
			//Const "0" (col 26)
			//Quantity Edit (col 27-34)						Type- 6.2 without the dot (Add Leading zeros)
			//Const "+" (col 35)

			string newRow = iturCode +												//Itur Code (col 1-4)									Itur Suffix
			"0101" +																			//Const "0101" (col 5-8)		
			row +																				//Counter of Items In Itur (col 9-11)**	     Count from 001-999
			makat +																			//Makat (col 12-25)									Add Leading zeros
			"0" +																				 	//Const "0" (col 26)
			quantityEdit +																	//Quantity Edit (col 27-34)						Type- 6.2 without the dot (Add Leading zeros)
			"+" +																					//Const "+" (col 35)
			Environment.NewLine;														//CR/LF  (col 33)
			sw.Write(newRow);
		}

		private string Int2Letter(string iturCode)
		{
			string letter1 = iturCode.Substring(0, 1);
			string letter3 = iturCode.Substring(1, 3);
			int i = 0;
			bool ret = Int32.TryParse(letter1, out i);

			if (i == 0) letter1 = "A";
			else if (i == 1) letter1 = "B";
			else if (i == 2) letter1 = "C";
			else if (i == 3) letter1 = "D";
			else if (i == 4) letter1 = "E";
			else if (i == 5) letter1 = "F";
			else if (i == 6) letter1 = "G";
			else if (i == 7) letter1 = "H";
			else if (i == 8) letter1 = "I";
			else if (i == 9) letter1 = "J";
			else letter1 = "N";
			iturCode = letter1 + letter3;
			return iturCode;
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
