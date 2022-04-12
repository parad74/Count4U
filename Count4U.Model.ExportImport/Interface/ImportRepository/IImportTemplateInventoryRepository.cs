using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface
{
	public interface IImportTemplateInventoryRepository
	{
		void InsertTemplateInventorys(string fromPathFile, string pathDB,
		   TemplateInventorySQLiteParserEnum templateInventoryParserEnum,
		   Encoding encoding, string[] separators, int countExcludeFirstString,
		   List<ImportDomainEnum> importType,
		   Dictionary<ImportProviderParmEnum, object> parms = null,
			List<string[]> ColumnMappings = null);
		//IEnumerable<TemplateInventory> GetTemplateInventorys(string pathDB);
		void ClearTemplateInventorys(string pathDB);
	}
}
