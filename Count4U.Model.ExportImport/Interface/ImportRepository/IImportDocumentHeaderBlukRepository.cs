using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using ErikEJ.SqlCe;

namespace Count4U.Model.Interface
{
	public interface IImportDocumentHeaderBlukRepository
	{
		void InsertDocumentHeaders(string fromPathFile, string pathDB,
		DocumentHeaderParseEnum documentHeaderParseEnumParserEnum,
		Encoding encoding, string[] separators, int countExcludeFirstString,
		List<ImportDomainEnum> importType,
		Dictionary<ImportProviderParmEnum, object> parms = null,
		List<string[]> columnMappings = null);

		void ClearDocumentHeaders(string pathDB);
		void InsertDocumentHeaders(DocumentHeaders documentHeaders, string pathDB);
	}
}
