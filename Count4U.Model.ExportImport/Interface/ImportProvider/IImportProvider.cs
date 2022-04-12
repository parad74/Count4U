using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using System.Xml.Linq;
using Count4U.Model;
using ErikEJ.SqlCe;

namespace Count4U.Model.Interface
{
	public interface IImportProvider 
	{
		void Import();
		void Clear();
		Dictionary<ImportProviderParmEnum, object> Parms { get; set; }
		string ToPathDB { get; set; }
		bool FastImport { get; set; }
		string FromPathFile { get; set; }
		Encoding ProviderEncoding { get; set; }
		List<string[]> ColumnMappings { set; get; }
		void SetColumnMappings<T>();
		//MaskPackage ParserMaskPackage { get; set; }
	}
}
