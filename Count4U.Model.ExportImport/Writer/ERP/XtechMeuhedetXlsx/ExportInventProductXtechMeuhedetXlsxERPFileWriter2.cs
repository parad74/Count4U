using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductXtechMeuhedetXlsxERPFileWriter2 : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		  
		//Items that exists in inventory and not exists in ERP Update File
		//Only (InventProduct + && Product.ExpextedERP -)

		// If Items that exists in inventory. IP+
		// and not exists in ERP Update File. Tag is Empty
		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			if (iturAnalyzes.Makat == "177")
			{
				string makat = iturAnalyzes.Makat;
			}
			if (iturAnalyzes.TypeMakat == TypeMakatEnum.W.ToString())
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(iturAnalyzes.Tag) == false)		 //если не пустая/ видимо есть update ERP, значит не надо вносить
			{
				return;
			}

			//if(iturAnalyzes.TypeMakat == TypeMakatEnum.W.ToString() )
			//{
			//	if (String.IsNullOrWhiteSpace(iturAnalyzes.Tag) == true)
			//	{
			//		return;
			//	}
			//}
		
			//	[A] Makat
			//[B] Item Name
			//[C] Quantity Edit 


			double quantityEditDouble = Convert.ToDouble(iturAnalyzes.QuantityEdit);
			string quantityEdit = quantityEditDouble.ToString("F2");

			string[] newRows = new string[] { iturAnalyzes.Makat, iturAnalyzes.ProductName, 
					iturAnalyzes.QuantityEdit.ToString() };
			string newRow = string.Join(separator, newRows);
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
