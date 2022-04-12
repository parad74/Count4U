using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductOrenMutagimERPFileWriter : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
//ERP Branch Code
//Inventory Created Date (DDMMYYY)
//Barcode
//Subsection Code
//Supplier Name
//Section Name
//Quantity Edit
//Makat


			string quantityEdit = String.Format("{0:0.00}", iturAnalyzes.QuantityEdit);
			string supplierCode = iturAnalyzes.SupplierCode;
			if (supplierCode == "none") supplierCode = "";
			string sectionCode = iturAnalyzes.SectionCode;
			if (sectionCode == "none") sectionCode = "";
			string subSectionCode = iturAnalyzes.SubSessionCode;
			if (subSectionCode == "none") subSectionCode = "";
			string barcode = "";
			if (iturAnalyzes.Barcode != iturAnalyzes.Makat) barcode = iturAnalyzes.Barcode;
			string[] newRows = new string[] { ERPNum, INVDate, barcode, subSectionCode, supplierCode, sectionCode, quantityEdit, iturAnalyzes.Makat };
			string newRow = string.Join(separator, newRows);
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
