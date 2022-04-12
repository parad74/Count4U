using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;

namespace Count4U.Model.Interface
{
	public interface IExportConfigIniRepository
	{
		void WriteToFile(string fromPathFile, string toPathFile,
			//ExportProviderEnum exportProviderEnum,
			WriterEnum writerEnum,
			Encoding encoding, string[] separators, 
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null	
			);

		void DeleteFile(string toPathFile);
	}
}
