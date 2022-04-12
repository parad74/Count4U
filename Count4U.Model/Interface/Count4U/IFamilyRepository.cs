using System;
using Count4U.Model.Count4U;
using System.Collections.Generic;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface.Count4U
{
	public interface IFamilyRepository
	{
		Familys GetFamilys(string pathDB);
		Familys GetFamilys(SelectParams selectParams, string pathDB);
		void DeleteAll(string pathDB);
		void Delete(Family family, string pathDB);
		void Insert(Family family, string pathDB);
		void Insert(Dictionary<string, Family> dictionaryFamily, string pathDB);
		void Update(Family family, string pathDB);
		Family GetFamilyByCode(string code, string pathDB);
		int GetFamilysTotal(string pathDB);
		List<string> GetFamilysCodeList(string pathDB);
		void RepairCodeFromDB(string pathDB);
		Dictionary<string, Family> GetFamilyDictionary(string pathDB,   bool refill = false);
		Dictionary<string, Family> GetFamilyDictionary_DescriptionKey(string pathDB);
		void ClearFamilyDictionary();
		void AddFamilyInDictionary(string code, Family location);
		void RemoveFamilyFromDictionary(string code);
		bool IsExistFamilyInDictionary(string code);
		Family GetFamilyByCodeFromDictionary(string code);
		void FillFamilyDictionary(string pathDB);
		bool IsAnyInDb(string pathDB);

	}
}

