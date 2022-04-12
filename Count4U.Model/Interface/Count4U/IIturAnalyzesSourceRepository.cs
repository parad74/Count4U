using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using System.Collections;

namespace Count4U.Model.Interface.Count4U
{
	public interface IIturAnalyzesSourceRepository
	{
		void InsertIturAnalyzes(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parms = null);
		void InsertIturAnalyzesSimple(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null);
		void InsertIturAnalyzesSumSimple(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null, bool addResult = true, List<ImportDomainEnum> importType = null);

		void InsertIturAnalyzesFamily(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null, bool addResult = true);

		void ClearIturAnalyzes(string pathDB);
		void AlterTableIturAnalyzes(string pathDB);
	//	IturAnalyzesRepositoryEnum CurrentIturAnalyzesRepository;
	}
}
