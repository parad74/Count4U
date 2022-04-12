using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using System.IO;

namespace Count4U.Model.Interface
{
	public interface IExportInventProductSimpleRepository
	{
		void WriteToFile(string fromPathDB, string toPathFile,
		WriterEnum writerEnum,
		Encoding encoding, string[] separators,
		List<ImportDomainEnum> importType,
		StreamWriter sw,
		Dictionary<ImportProviderParmEnum, object> parms = null,
		bool refill = true
		);

		void DeleteFile(string toPathFile);
	}
}
