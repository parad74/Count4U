using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using System.Xml.Linq;
using Count4U.Model;

namespace Count4U.Model.Interface
{
	public interface IExportERPProvider //: IExportProvider
	{
		//string LocationCode { get; set; }
		//string IturCode { get; set; }
		void Clear();
		Dictionary<ImportProviderParmEnum, object> Parms { get; set; }
		string FromPathDB { get; set; }
		string ToPathFile { get; set; }
		Encoding ProviderEncoding { get; set; }
		string SetImportTypeExportByLocation(string locationCode, string toFilePath);
		string SetImportTypeExportByItur(string iturCode, string toFilePath);

		string SetImportTypeExportByModifyDate(DateTime? dateTime, string toFilePath);
		void ClearImportTypeExportByItur();
		void ClearImportTypeExportByLocation();
		void ClearImportTypeExportByModifyDate();
		void Export(bool full = true, bool isFilterByLocations = false, List<string> locationCodeList = null,
			bool IsFilterByItur = false, List<string> iturCodeList = null, bool onModifyDate = false, DateTime? modifyDate = null);
			//bool IsGroupByItur = false, List<string> iturCodeFullList = null );
		//void ExportByLocation(string locationCode);
		//void ExportByItur(string iturCode);
	}
}
