using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface
{
	public interface IExportIturRepository
	{
		void WriteToFile(string fromPathDB, string toPathFile,
		IturParserEnum productParserEnum,
		WriterEnum iturWriter,
		Encoding encoding, string[] separators,
		 List<ImportDomainEnum> importType,
		Dictionary<ImportProviderParmEnum, object> parms = null
		);

		void DeleteFile(string toPathFile);
	}
}
