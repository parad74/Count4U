using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductAS400AprilERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "",
			string separator = ",", object argument = null)
		{
			
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			long rest1000 = countRow % 999;
			if (countRow <= 999) rest1000 = 0;

			if (iturAnalyzes.IturCode != iturAnalyzes.Barcode // переход между Itur и надо вставить загаловок для нового итура (в Barcode хранится IturCode предыдущей записи в списке отсортированном по IturCode)
				|| rest1000 == 1)  //или началась новая 1000 row
			// см ExportInventProductNibitERPFileWriter - это для заголовка. Здесь не обрабатывается
			{
				//////Header for each Itur
				//////	Itur Code (col 1-4)				                    //Itur Suffix
				//////Const "0101" (col 5-8)								//"0101"
				//////Counter of Items In Itur (col 9-11)		//Const "000" 
				//////Inventory Date (col 12-19)Date Format: //YYYYMMDD
				//////Inventory Time (col 20-23)TIME Format: //HHMM
				//////BranchCode ERP (col 24-28)Add					// Leading zeros
				//////Const "00000000" (col 29-36)						//"00000000"
				//////Const "+" (col 37)											//"+"
				//==============
	
				//string erpCode = ERPNum;
				//if (ERPNum.Length > 4)
				//{
				//	erpCode = ERPNum.Substring(0, 4);
				//}
				//else
				//{
				//	erpCode = ERPNum.PadLeft(4, '0');
				//}

				//string _iturCode = "000000";
				//if (iturAnalyzes.IturCode.Length < 6)
				//{
				//	_iturCode = iturAnalyzes.IturCode.PadLeft(6,'0');
				//}

				//if (iturAnalyzes.IturCode.Length >= 8)
				//{
				//	_iturCode = iturAnalyzes.IturCode.Substring(2, 6);
				//}
				

				//if (rest1000 == 1)
				//{
				//	_iturCode = Int2Letter(_iturCode);
				//}

				//string invData = "00000000";
				//if (INVDate.Length > 8)
				//{
				//	invData = INVDate.Substring(0, 8);  //Date (col 46-53)								//Format: YYYYMMDD //Date (col 64-71)								//Format: YYYYMMDD
				//}
					
				////header
				//string newRow1 = _iturCode +							//	Itur Code (col 1-4)				                    //Itur Suffix
				//"0101000" +														//Const "0101" (col 5-8)								//"0101"
				//																		//Counter of Items In Itur (col 9-11)		//Const "000" 
				//invData +															//Inventory Date (col 12-19)Date Format: //YYYYMMDD
				//erpCode +														//BranchCode ERP (col 24-28)Add					// Leading zeros
				//"00000000+" +													//Const "00000000" (col 29-36)						//"00000000"				//Const "+" (col 37)											//"+"
				//Environment.NewLine;										//CR/LF  (col 33)
				//sw.Write(newRow1);
			}

			//-------------следующие записи в Itur 
			//Branch ERP_Code (col 1-4)				//Add leading zeros
			//IturCode (col 5-10)							//Cut first 2 digit (from prefix)
			//Const "000000" (11-16)
			//Counter of Items In Itur (17-19)**//Add leading zeros
			//Makat (col 20-34)							//Cut leading zeros
			//Quantity Edit (col 35-45)				//Add leading zeros (format 9.2 without the dot)
			//Date (col 46-53)								//Format: YYYYMMDD
			//Const "          " (col 54-63)
			//Date (col 64-71)								//Format: YYYYMMDD

			string erpCode = ERPNum;
			if (ERPNum.Length > 4)
			{
				erpCode = ERPNum.Substring(0, 4);
			}
			else
			{
				erpCode = ERPNum.PadLeft(4, '0');
			}

			string erpCode1 = erpCode.Substring(2, 2);
			erpCode = erpCode + erpCode1;

			string iturCode = "000000";
			if (iturAnalyzes.IturCode.Length < 8)
			{
				iturCode = iturAnalyzes.IturCode.PadLeft(4, '0');
			}
			else// >= 8
			{
				iturCode = iturAnalyzes.IturCode.Substring(4, 4);

			}

			//Makat (col 20-34)							//Cut leading zeros
			string makat = iturAnalyzes.Makat.TrimStart('0');  
			makat = makat.PadRight(15, ' ');

			//Quantity Edit (col 35-45)				//Add leading zeros (format 9.2 without the dot)
			double quantityEditDouble = Convert.ToDouble(iturAnalyzes.QuantityEdit);
			int quantityEdit00 = (int)(quantityEditDouble * 100);

			string quantityEdit = quantityEdit00.ToString().Trim();
			if (quantityEdit == "0") quantityEdit = "00";
			quantityEdit = quantityEdit.PadLeft(11, '0');

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

			//Branch ERP_Code (col 1-4)				//Add leading zeros
			//IturCode (col 5-10)							//Cut first 2 digit (from prefix)
			//Const "000000" (11-16)
			//Counter of Items In Itur (17-19)**//Add leading zeros
			//Makat (col 20-34)							//Cut leading zeros
			//Quantity Edit (col 35-45)				//Add leading zeros (format 9.2 without the dot)
			//Date (col 46-53)								//Format: YYYYMMDD
			//Const "          " (col 54-63)
			//Date (col 64-71)								//Format: YYYYMMDD

			string newRow = erpCode + 
				iturCode +
				"000000" +													
				row +														
				makat +													
				quantityEdit +		
				INVDate +
				"          " +
				INVDate	+
			Environment.NewLine;								
			sw.Write(newRow);
		}

		private string Int2Letter(string iturCode)
		{
			string letter1 = iturCode.Substring(0, 1);
			string letter5 = iturCode.Substring(1, 5);
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
			iturCode = letter1 + letter5;
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
