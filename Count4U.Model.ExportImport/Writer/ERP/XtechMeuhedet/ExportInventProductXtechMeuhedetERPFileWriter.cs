using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductXtechMeuhedetERPFileWriter : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
			//ERP Code Branch (col 1-4)
			//ITUR (col 5-10)

			//Num In ITUR (col 16-19)
			//Makat (col 20-25)
			//Quantity Edit (col 35-45)
			//INV Date (col 46-53)
			//INV Date (col 64-71)

			//Sample: 
			//Add leading zeros
			//Itur Format: 2 last digits from prefix and 4 digits of ITUR num
			//Add leading zeros
			//Date Format (YYYYMMDD)
			//Date Format (YYYYMMDD)

			//Record Sample:
			//5510030007       12143490         0000000500020101220          20101220     
			//2222290001       1   5224514820738L          1         23072011          23072011

			//ERP Code Branch (col 1-4)
			string erpCode = ERPNum;
			if (ERPNum.Length > 4) { erpCode = ERPNum.Substring(0, 4); }	   //ERP Code Branch (col 1-4)
			if (ERPNum.Length < 4) { erpCode = ERPNum.PadRight(4, '0');  }	 

			//ITUR (col 5-10)
			//string prefix = iturAnalyzes.Itur_NumberPrefix;
			//if (string.IsNullOrWhiteSpace(prefix) == true)
			/*{
				prefix = "00";
			}
			else if (prefix.Length <= 2)
			{
				prefix = "00";
			}
			else
			{
				if (prefix.Length > 4) { prefix = iturAnalyzes.Itur_NumberSufix.Substring(0, 4); }
				prefix = iturAnalyzes.Itur_NumberPrefix.Substring(2); 
			}*/

            string docNum = iturAnalyzes.DocNum.ToString();
            if (string.IsNullOrWhiteSpace(docNum) == true)
            {
                docNum = "001";
            }
            else if (docNum.Length == 1)
            {
                docNum = "00" + docNum;
            }
            else if (docNum.Length == 2)
            {
                docNum = "0" + docNum;
            }
            else
            {
                if (docNum.Length > 3)
                {
                    int len = docNum.Length;
                    int start = len - 3;
                    docNum = iturAnalyzes.DocNum.ToString().Substring(start);
                }
            }

            //its need to be Col 5-8 : Itur Suffix
            //Col 9-10 : Document Number

			string sufix = iturAnalyzes.Itur_NumberSufix;		  
			if (sufix.Length > 3) 
            {
                int len = sufix.Length;
                int start = len - 3;
                sufix = iturAnalyzes.Itur_NumberSufix.Substring(start, 3); 
            }
			if (sufix.Length < 3) { sufix = iturAnalyzes.Itur_NumberSufix.PadLeft(3, '0'); }
            string iturCode = (sufix + docNum);			  //ITUR (col 5-10)

			//Num In ITUR (col 16-19)
            
			string num = iturAnalyzes.IPNum.ToString();
			//if (num.Length > 4) { num = num.Substring(0, 4) ; }
			//string num0 = num.PadLeft(4, '0');			 //Num In ITUR (col 16-19)

			string iturAnalyzesMakat = iturAnalyzes.Makat;
			if (iturAnalyzesMakat.Length > 6) { iturAnalyzesMakat = iturAnalyzes.Makat.Substring(0, 6); }

	
			//ERP Code Branch (col 1-4)
			//ITUR (col 5-10)

			//Num In ITUR (col 16-19)
			//Makat (col 20-25)
			//Quantity Edit (col 35-45)
			//the format is :
			//Left side of the decimal point = col 35-42			 = 8
			//Right side of the decimal point = col 43-45			 = 3

			double quantityEdit = Convert.ToDouble(iturAnalyzes.QuantityEdit);
			long cel = Convert.ToInt64(Math.Truncate(quantityEdit));
			string quantityEditDrb = "0";
			quantityEditDrb = quantityEdit.Fraction();
			if (quantityEditDrb.Length > 1)
			{
				quantityEditDrb.TrimEnd('0');
				if (quantityEditDrb.Length > 3) { quantityEditDrb = quantityEditDrb.Substring(0, 3); }
			}
			
			//int drb = Convert.ToInt32((quantityEdit - cel) * 1000);
			//string quantityEditDrb0 = drb.ToString().PadLeft(3, '0');
		   
			string quantityEditCel0 = cel.ToString().PadLeft(8, '0');
			string quantityEditDrb0 = quantityEditDrb.PadRight(3, '0');
			string quantityEdit0 = quantityEditCel0 + quantityEditDrb0;

			//Sample for quantity = 2.427
			//Will look like that on the Export file:
			//5510030007 150178015 0000000242720101220 20101220 
			//INV Date (col 46-53)
			//INV Date (col 64-71)
			string newRow = erpCode +									//ERP Code Branch (col 1-4)
				 iturCode +														 //ITUR (col 5-10)
				 @"     " +
				String.Format("{0,4}", num)  +						    //Num In ITUR (col 16-19)
				String.Format("{0,6}", iturAnalyzesMakat) +	 //Makat (col 20-25)
				@"         " +
				String.Format("{0,11}", quantityEdit0) +			//Quantity Edit (col 35-45)
				String.Format("{0,8}", INVDate) +					//INV Date (col 46-53)
				 @"          " +
				String.Format("{0,8}", INVDate);				   //INV Date (col 64-71)
				sw.WriteLine(newRow);
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			throw new NotImplementedException();
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
