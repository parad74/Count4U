using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;

namespace Count4U.Model.Interface
{
	public interface IExportCatalogSimpleRepository
	{
		void WriteToFile(string fromPathDB, string toPathFile,
			ProductSimpleParserEnum productParserEnum,
			WriterEnum 	productWriter,
			//ExportProviderEnum exportProviderEnum,
			Encoding encoding, string[] separators, 
			List<ImportDomainEnum> importType,
			bool trimEndOrAddSeparator = true,
			Dictionary<ImportProviderParmEnum, object> parms = null	
			);

		void DeleteFile(string toPathFile);
	}
}
