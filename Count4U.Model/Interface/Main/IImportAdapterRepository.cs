using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Main
{

	public interface IImportAdapterRepository
    {
    	ImportAdapters GetImportAdapters();
		ImportAdapters GetImportAdapters(SelectParams selectParams);
		ImportAdapters GetAllowedImportAdapters(string customerCode, string branchCode,
			string inventorCode, ImportDomainEnum importDomainType);
		void Delete(long id);	
    }
}
