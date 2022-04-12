using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;

namespace Count4U.Model.Interface.Count4U
{

	public interface IPropertyStrToObjectRepository
	{
		PropertyStrToObjects GetPropertyStrToObjects(string pathDB);
		PropertyStrToObjects GetPropertyStrToObjects(int topCount, string pathDB);
		PropertyStrToObjects GetPropertyStrToObjects(SelectParams selectParams, string pathDB);
		void Delete(PropertyStrToObject propertyStrToObject, string pathDB);
		void DeleteAll(string pathDB);
		void Insert(PropertyStrToObject propertyStrToObject, string pathDB);
		void Update(PropertyStrToObject propertyStrToObject, string pathDB);
		PropertyStrToObject GetPropertyStrToObjectByCode(string code, string pathDB);
		Dictionary<string, PropertyStrToObject> GetPropertyStrToObjectDictionary(string pathDB, bool refill = false);
		void ClearPropertyStrToObjectDictionary();
		void AddPropertyStrToObjectInDictionary(string code, PropertyStrToObject propertyStrToObject);
		void RemovePropertyStrToObjectFromDictionary(string code);
		bool IsExistPropertyStrToObjectInDictionary(string code);
		PropertyStrToObject GetPropertyStrToObjectByCodeFromDictionary(string code);
		void FillPropertyStrToObjectDictionary(string pathDB);
		List<string> GetPropertyStrToObjectsCodeList(string pathDB);
	}
}
