using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductMaccabiPharmSAPERPFileWriter : IExportInventProductFileWriter
	{
  		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ";")
		{
//			Field1: ERP Branch Code (4 Chars)
//Field2: Item Code (5 Chars)
//Field3: Quantity Expected (6.3 Chars)
//			Record Sample:
//2346;  349;    13.267


		//	string quantityEdit = String.Format("{0:0.000}", iturAnalyzes.QuantityEdit);

			long quantityEdit = Convert.ToInt64(iturAnalyzes.QuantityEdit);
			int quantityInPackEdit = iturAnalyzes.QuantityInPackEdit;

			string quantityEditStr = quantityEdit.ToString();
			if (quantityEditStr.Length > 6) { quantityEditStr = quantityEditStr.Substring(0, 6); }
			if (String.IsNullOrWhiteSpace(quantityEditStr) == true) quantityEditStr = "0";

			string quantityInPackEditStr = quantityInPackEdit.ToString();
			if (quantityInPackEditStr.Length > 3) { quantityInPackEditStr = quantityInPackEditStr.Substring(0, 3); }
			quantityInPackEditStr = quantityInPackEditStr.LeadingZero3();
			if (String.IsNullOrWhiteSpace(quantityInPackEditStr) == true) quantityInPackEditStr = "000";

			string makatNumber = iturAnalyzes.MakatLong.ToString();
			if (makatNumber.Length > 5) { makatNumber = makatNumber.Substring(0, 5); }

			string newRow =
			String.Format("{0,4}", ERPNum) +					 //ERP Branch Code (4 Chars)
			";" +
			String.Format("{0,5}", makatNumber) +			//Item Code (5 Chars)
			";" +
			String.Format("{0,6}", quantityEdit) +					//Quantity Expected (6.3 Chars)
			"." +
			quantityInPackEditStr;		
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
