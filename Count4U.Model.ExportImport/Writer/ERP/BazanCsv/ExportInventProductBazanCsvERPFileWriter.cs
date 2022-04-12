using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductBazanCsvERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",")
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
//			Field1: ERP Branch Code (4 Chars)
//Field2: Item Code (5 Chars)
//Field3: Quantity Expected (6.3 Chars)
//			Record Sample:
//2346;  349;    13.267


		//	string quantityEdit = String.Format("{0:0.000}", iturAnalyzes.QuantityEdit);

			//ERP Itur Code 
			//Makat 
			//Boxes Quantity
			//Quantity Edit 
			//Expired Date

			string quantityEdit = String.Format("{0:0000.00}", iturAnalyzes.QuantityEdit);
			string boxesEdit = iturAnalyzes.QuantityInPackEdit.ToString().Trim();
			if (String.IsNullOrWhiteSpace(boxesEdit) == true) boxesEdit = "0";

			string[] newRows = new string[] { iturAnalyzes.IturCode, iturAnalyzes.Makat, iturAnalyzes.QuantityInPackEdit.ToString(),  
					quantityEdit,  iturAnalyzes.Barcode};
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

	}
}
