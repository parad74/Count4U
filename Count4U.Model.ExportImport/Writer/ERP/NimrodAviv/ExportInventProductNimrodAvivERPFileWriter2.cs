using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductNimrodAvivERPFileWriter2 : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
			string localBranchCode = ERPNum.PadLeft(3, '0');//3 chars – Add zero to the left
			string iturCodeSuffix = "0000";
			if (iturAnalyzes.IturCode.Length > 4)
			{
				iturCodeSuffix = iturAnalyzes.IturCode.Substring(4).PadLeft(4, '0');//4 chars – Add zero to the left
			}
			string makat = iturAnalyzes.Makat.PadRight(15, ' ');//15 chars – Add spaces to the right
			int quantityEditInt = (int)iturAnalyzes.QuantityEdit;
			string quantityEdit = quantityEditInt.ToString().PadLeft(10, '0');	//10 chars – Add zero to the left
			string makatType = "0";
			string section = "00";
			string brand = "00000";
			string color = "000";
			string size = "000";
			string newConst = "NEW  ";//Const "NEW" - 5 chars – Add spaces to the right
			
			//	1NR103226600260 -> 1|NR|10322|660|260 
			//(Char in Col 12 is const "0")
			//Type – 1 char (Col 1)
			//Section – 2 Chars (Col 2-3)
			//Brand – 5 Chars (Col 4-8) 
			//Color – 3 Chars (Col 9-11)
			//Size – 3 Chars (Col 13-15)

			makatType = makat.Substring(0, 1);
			section = makat.Substring(1, 2);
			brand = makat.Substring(3, 5);
			color = makat.Substring(8, 3);
			size = makat.Substring(12, 3);

			//Local Branch Code				//3 chars – Add zero to the left
			//Itur Code (Suffix only)		//4 chars – Add zero to the left
			//Item Code (Makat)				//15 chars – Add spaces to the right
			//Quantity Edit						//10 chars – Add zero to the left
			//Type (Makat Cut -1) *			//1 Char
			//Section (Makat Cut -2)		//2 Chars
			//Brand (Makat Cut -3)			//5 Chars
			//Color (Makat Cut -4)			//3 Chars
			//Size (Makat Cut -5)				//3 Chars
			//Const OLD							//Const "OLD" - 5 chars – Add spaces to the right
			//Item Name							//32 Chars (Add spaces to the right)
			string[] newRows = new string[] { localBranchCode, iturCodeSuffix, makat, quantityEdit, makatType, section, brand, color, size, newConst };
			string newRow = string.Join("|", newRows);
			sw.WriteLine(newRow);
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
