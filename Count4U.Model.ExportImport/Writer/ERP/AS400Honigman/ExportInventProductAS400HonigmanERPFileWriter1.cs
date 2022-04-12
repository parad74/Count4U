using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductAS400HonigmanERPFileWriter1 : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{

		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//ERPBranchCode(col  1-3)
			//Makat (col 4-9)
			//Qty (col 16-20)
			//Date (21-28)

			//ERP Branch Code
			//Align to the left 
			//Align to the right (add leading zeros – integers only)
			//Format YYYYMMDD

			//Record Sample:
			//	145200003      0000520150610
			//
			//Colum 1-3 store:  store number, 3 numbers example:071
			//Colum 4-15 part number: 12 digits aligned to the left without zeros
			//Colum 16-20 quantity: two leading zeros  example: 001 002	   col16-20 (add leading zeros)
			//Colum 21-28 date: YYYYMMDD example: 20170206  

			double quantityEditDouble  = Convert.ToDouble(iturAnalyzes.QuantityEdit);
			int quantityEditInt = (int)(quantityEditDouble);

			string quantityEdit = quantityEditInt.ToString().Trim();
			quantityEdit = quantityEdit.PadLeft(5, '0');
			if (quantityEdit == "0") quantityEdit = "00000";  

			string erpCode = ERPNum.LeadingZero3();
			string makatOriginal = iturAnalyzes.Makat;  //Without Mask!


			string newRow = String.Format("{0,3}", erpCode) +				//Colum 1-3 store:  store number, 3 numbers example:071
				String.Format("{0,-12}", makatOriginal) +							 //Colum 4-15 part number: 12 digits aligned to the left without zeros
				String.Format("{0,5}", quantityEdit) +								//Colum 16-20 quantity: two leading zeros  example: 001 002	   col16-20 (add leading zeros)
				INVDate;																			//YYYYMMDD
			sw.WriteLine(newRow);
			//Environment.NewLine;										//CR/LF  
			//sw.Write(newRow);

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
