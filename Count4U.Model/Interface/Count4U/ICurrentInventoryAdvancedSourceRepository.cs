using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using System.Collections;

namespace Count4U.Model.Interface.Count4U
{
	public interface ICurrentInventoryAdvancedSourceRepository
	{
		void InsertCurrentInventoryAdvanced(string pathDB, bool refill = true,
				bool refillCatalogDictionary = false, 
			SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null, List<ImportDomainEnum> importType = null);
	
		void ClearCurrentInventoryAdvanced(string pathDB);
		void AlterTableCurrentInventoryAdvanced(string pathDB);
	//	IturAnalyzesRepositoryEnum CurrentIturAnalyzesRepository;
	}
}
