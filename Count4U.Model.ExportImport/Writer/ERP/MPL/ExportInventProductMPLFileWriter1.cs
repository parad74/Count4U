using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductMPLFileWriter1 : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
			//OrenWarehouseName (Col 1-10)		//Const "IEL       " (10 chars)
			//Item Code (Col 11-30)						//Item Code (9 chars from item Code + "-" + Size) – If no size, drop the "-", Complete with Spaces on the right
			//Quantity Edit (Col 31-38)					//Complete with Zeros on the left
			//ERP Itur Code (Col 39-??)*				//Add the Itur code, no mask

			//string orenWarehouseName = "IEL       ";
			//string quantityEditString = Convert.ToInt32(iturAnalyzes.QuantityEdit).ToString().PadLeft(8, '0');
			//string itemCode = iturAnalyzes.Makat;
			////string size = "";
			//if (iturAnalyzes.Makat.Length >= 20)
			//{
			//	itemCode = iturAnalyzes.Makat.Substring(0, 20);
			//	//if (iturAnalyzes.Makat.Length >= 10)
			//	//{
			//	//	size = iturAnalyzes.Makat.Substring(9);
			//	//	if(string.IsNullOrWhiteSpace(size) == false)
			//	//	{
			//	//		size = size.Replace("V", ".5");
			//	//		size = "-" + size;
			//	//	}
			//	//}
			//}
			//string makat = itemCode; //+ size;
			//makat = makat.PadRight(20, ' ');

			//string newRow = orenWarehouseName +
			//	makat +
			//	quantityEditString +
			//	iturAnalyzes.ERPIturCode;

			//sw.WriteLine(newRow);
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//OrenWarehouseName (Col 1-10)		//Const "IEL       " (10 chars)
			//Item Code (Col 11-30)						//Item Code (9 chars from item Code + "-" + Size) – If no size, drop the "-", Complete with Spaces on the right
			//Quantity Edit (Col 31-38)					//Complete with Zeros on the left
			//ERP Itur Code (Col 39-??)*				//Add the Itur code, no mask

			string orenWarehouseName = "IEL       ";
			string quantityEditString = Convert.ToInt32(iturAnalyzes.QuantityEdit).ToString().PadLeft(8, '0');
			string itemCode = iturAnalyzes.Makat;
			//string size = "";
			if (iturAnalyzes.Makat.Length >= 20)
			{
				itemCode = iturAnalyzes.Makat.Substring(0, 20);
				//if (iturAnalyzes.Makat.Length >= 10)
				//{
				//	size = iturAnalyzes.Makat.Substring(9);
				//	if(string.IsNullOrWhiteSpace(size) == false)
				//	{
				//		size = size.Replace("V", ".5");
				//		size = "-" + size;
				//	}
				//}
			}
			string makat = itemCode; //+ size;
			makat = makat.PadRight(20, ' ');

			string newRow = orenWarehouseName +
				makat +
				quantityEditString +
				iturAnalyzes.IturCode;

			sw.WriteLine(newRow);
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
