using System;
using Count4U.Model.Count4U;
using System.Collections.Generic;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface.Count4U
{
	public interface IShelfRepository
	{

		Shelfs GetShelves(string pathDB);
		Shelfs GetShelves(SelectParams selectParams, string pathDB);
		Shelfs GetShelvesByIturCode(string iturCode, string pathDB);
		void Delete(Shelf shelf, string pathDB);
		void DeleteAll(string pathDB);
		void Insert(Shelf shelf, string pathDB);
		void Insert(Dictionary<string, Shelf> dictionaryShelf, string pathDB);
		void Update(Shelf shelf, string pathDB);
		Shelf GetShelfByName(int num, string pathDB);
		Shelf GetShelfByCode(string code, string pathDB);
		int GetShelfsTotal(string pathDB);
		List<string> GetShelfsCodeList(string pathDB);
		Dictionary<string, Shelf> GetShelfDictionary(string pathDB, bool refill = false);
		void AddShelfInDictionary(string code, Shelf shelf);
		void RemoveShelfFromDictionary(string code);
		bool IsExistShelfInDictionary(string code);
		Shelf GetShelfByCodeFromDictionary(string code);
		void FillShelfDictionary(string pathDB);
		bool IsAnyInDb(string pathDB);

	}
}

