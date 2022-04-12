using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductXtechMeuhedetXlsxERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		// If Items that exists in inventory 	   IP+
		// and exists in ERP Update File 	   tag not Empty
		// ADD in sfira_XXX.txt
		//if(tag is Empty) return
		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//IP , NotCatalog -> no
			//if (iturAnalyzes.TypeMakat == TypeMakatEnum.W.ToString())
			//{
				//if (String.IsNullOrWhiteSpace(iturAnalyzes.Tag) == true)
				//{
				//	return;
				//}
			//}

			if (string.IsNullOrWhiteSpace(iturAnalyzes.Tag) == true)	 //если пустая/ видимо нет update ERP
			{
				return;
			}

			string[] tags =  iturAnalyzes.Tag.Split('|');
			  
			String[] externals= { "", "", "", "" , "", ""};
			int len = tags.Length;
			if (len > 0) len = 6;
			for (int j = 0 ; j < tags.Length; j ++)
			{
				externals[j] = tags[j];
			}
			//DocumentERP (External1)	   tags[0]
			//Year (External2)					tags[1]
			//Date (External3)					tags[2]
			//Increment Row number			i++

			//Makat								 iturAnalyzes.Makat
			//Item Name						  iturAnalyzes.ProductName
			//BarcodeExtra (External4)	  tags[3]
			//Const "1000"
			//Const "קופת חולים מאוחדת"
			//BranchCodeERP				ERPNum
			//BranchName						  INVDate
			//	  "" - Leave empty
			//Quantity Edit 					 iturAnalyzes.QuantityEdit

			//Pack (External5)						tags[4]

			double quantityEditDouble = Convert.ToDouble(iturAnalyzes.QuantityEdit);
			string quantityEdit = quantityEditDouble.ToString("F3"); 

			string[] newRows = new string[] { 
				externals[0] ,
				externals[1] 	,
				externals[2] ,
				countRow.ToString(),
				iturAnalyzes.Makat,
				 iturAnalyzes.ProductName,
				 externals[3] ,
				 "1000", 
				 "קופת חולים מאוחדת",
				 ERPNum, 
				 INVDate,
				 "",
				quantityEdit,
				externals[4] };
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
