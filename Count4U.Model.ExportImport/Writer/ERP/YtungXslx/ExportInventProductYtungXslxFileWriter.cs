using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductYtungXslxFileWriter : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "",
			string separator = "^", object argument = null)
		{

			int quantityEdit = (int)iturAnalyzes.IPValueFloat5 != null ? (int)iturAnalyzes.IPValueFloat5 : 0;
			int quantityInPackEdit = (int)iturAnalyzes.QuantityInPackEdit != null ? (int)iturAnalyzes.QuantityInPackEdit : 0;

			double priceBuy = iturAnalyzes.PriceBuy != null ? (double)iturAnalyzes.PriceBuy : 0.0;
			double priceSale = iturAnalyzes.PriceSale != null ? (double)iturAnalyzes.PriceSale : 0.0;

			//	QuantityEdit * PriceBuy
			double valueEdit = quantityEdit * priceBuy;
			//QuantityPartial * PriceSell
			double valueInPackEdit = quantityInPackEdit * priceSale;
			//QuantityEdit * PriceBuy + QuantityPartial * PriceSell
			double sumValue = valueEdit + valueInPackEdit;


			//	0	BranchCode
			//1	IturCode
			//2	ItemCode
			//3	ProductName
			//4	PriceBuy
			//5	SectionName
			//6	PriceSell
			//7	SupplierName
			//8	QuantityEdit
			//9	QuantityPartial
			//10	QuantityEdit * PriceBuy
			//11	QuantityPartial * PriceSell
			//12	QuantityEdit * PriceBuy + QuantityPartial * PriceSell

			string quantityEditString = String.Format("{0:0.##}", iturAnalyzes.QuantityEdit);
			string[] newRows = new string[13];
			newRows[0] = ERPNum;												   	//	0	BranchCode
			newRows[1] = iturAnalyzes.IturCode;							//1	IturCode
			newRows[2] = iturAnalyzes.Makat;								 //2	ItemCode
			newRows[3] = iturAnalyzes.ProductName;					 //3	ProductName
			newRows[4] = String.Format("{0:0.##}", priceBuy);		 //4	PriceBuy
			newRows[5] = iturAnalyzes.SectionCode;						//5	SectionName
			newRows[6] = String.Format("{0:0.##}", priceSale);		 //6	PriceSell
			newRows[7] = iturAnalyzes.SupplierCode;							//7	SupplierName
			newRows[8] = quantityEdit.ToString();							   //8	QuantityEdit
			newRows[9] = quantityInPackEdit.ToString();				   //9	QuantityPartial
			newRows[10] = String.Format("{0:0.##}", valueEdit);							//10	QuantityEdit * PriceBuy
			newRows[11] = String.Format("{0:0.##}", valueInPackEdit);					//11	QuantityPartial * PriceSell
			newRows[12] = String.Format("{0:0.##}", sumValue);							//12	QuantityEdit * PriceBuy + QuantityPartial * PriceSell



			//string[] newRows = new string[] { iturAnalyzes.Makat, iturAnalyzes.ProductName, iturAnalyzes.Price.ToString(), 
			//	countInParentPack.ToString(), 
			//	quantityEdit.ToString(), quantityEditXcountInParentPack.ToString(), balanceQuantityERP.ToString(),  quantityDifference.ToString(),
			//	inventoryValue.ToString(), ERPValue.ToString(),  valueDifference.ToString()};

			string newRow = string.Join("^", newRows);
			sw.WriteLine(newRow);
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow,
			string ERPNum = "", string INVDate = "", string separator = "^")
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
