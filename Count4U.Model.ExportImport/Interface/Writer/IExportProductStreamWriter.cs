using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using System.IO;

namespace Count4U.Model.Interface
{
	public interface IExportProductStreamWriter
	{
		void AddRow(StreamWriter sw, Product product,
			Dictionary<string, ProductMakat> productMakatDictionary,
			Dictionary<string, string> productMakatBarcodesDictionary,
			bool makatWithoutMask, bool barcodeWithoutMask, 
			ExportFileType fileType, 
			int maxLen,
			bool invertLetter, bool rt2lf, string separator = ",");
		void AddRowSimple(StreamWriter sw, Product product, 
			Dictionary<string, ProductMakat> productMakatDictionary,
			Dictionary<string, string> productMakatBarcodesDictionary,
			Dictionary<string, Family> familyDictionary ,
			bool makatWithoutMask, bool barcodeWithoutMask, 
			ExportFileType fileType, 
			int maxLen,
			bool invertLetter, bool rt2lf, bool cutLf2Rt = false, string separator = ",", bool trimEndOrAddSeparator = true);
	
		void AddHeader(StreamWriter sw,
			string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null);
		void AddHeaderSum(StreamWriter sw, 
			string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null);

	}
}
