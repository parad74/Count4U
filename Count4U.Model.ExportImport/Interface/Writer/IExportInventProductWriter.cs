using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using System.IO;

namespace Count4U.Model.Interface
{
	public interface IExportInventProductFileWriter
	{
		void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow,
			string ERPNum = "", string INVDate = "", string separator = ",", object argument = null);
		void AddRowInventProduct(StreamWriter sw, InventProduct inventProduct, long countRow,
			string ERPNum = "", string INVDate = "", string separator = ",", object argument = null);
		void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow,
			string ERPNum = "", string INVDate = "", string separator = ",");
		void AddHeader(StreamWriter sw,
			string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null);
		void AddHeaderSum(StreamWriter sw, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null,
			string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null);
		void AddFooter(StreamWriter sw, long countRow,
			string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null);
		void AddFooterSum(StreamWriter sw, long countRow, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null, 
			string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null);

	}
}
