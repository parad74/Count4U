using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductRetalixNextERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "",
			string separator = ",", object argument = null)
		{
	
		}
//010100100001301071200000000000000
//		Header:
//Header Code (col 1-8)*
//LineNumber- Const “000”  (col 9-11)
//Date “YYMMDD” (col 12-17)
//Time "HHMM" (col 18-21)
//Const Zeros (col 22-33)
//CR/LF  (col 34)
//		Start from "01010001" 
////2Start from "01010010" 
//Always 000 (Line Number in Header)


//01010010001000000000000900002200+
//Record:
//Header Code (col 1-8)
//LineNumber- (col 9-11)
//Makat (col 12-24)
//Qty (col 25-32)
//Quantity sign (33)
//Line number Runs from 001 - 200
//Add leading zeros
//Type- 6.2 without the dot
//Default "+"
//In Case of negative QTY the “-“ sign will appear in the 33 position


		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			int num201 = (int)countRow / 201 + 1;
			int num200 = (int)countRow / 200 + 1;
			string numDiv201 = num201.ToString().LeadingZero3();
			string numDiv200 = num200.ToString().LeadingZero3();
			string seq = "0101" + numDiv200;

			long rest200 = countRow % 200;
			if (rest200 == 0)
			{
				rest200 = 200;
				seq = "0101" + numDiv201;
			}
			long rest201 = countRow % 201;
			string numRest201 = (rest201).ToString().LeadingZero4();
			string numRest200 = (rest200).ToString().LeadingZero4();
			//010100500001301071200000000000000

							//01010010001
			if (rest200 == 1)
			{
				string newRow1 =
				seq +
				"0000" +
				String.Format("{0,10}", INVDate) +	
				"000000000000"  +
				Environment.NewLine;										//CR/LF  
				sw.Write(newRow1);
				//sw.WriteLine(newRow1);
			}

			//string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit)).ToString().LeadingZero7();
			string sign = "+";
			if (iturAnalyzes.QuantityEdit < 0) sign = "-";
			string quantityEdit = String.Format("{0:000000.00}", iturAnalyzes.QuantityEdit);
			quantityEdit = quantityEdit.Replace(".","");
			quantityEdit = quantityEdit.Replace(",", "");
			string makat = iturAnalyzes.Makat.LeadingZero13();
			string newRow =
				seq +
				numRest200 +
				makat +
				quantityEdit +
				sign + 
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
