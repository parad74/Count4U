using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model.Common;

namespace Count4U.Model.Interface.Count4U
{
   
	public interface IPropertyStrRepository
	{
		PropertyStrs GetPropertyStrs(string pathDB);
		PropertyStrs GetPropertyStrs(string domainObjectType, string pathDB);
		Dictionary<string, PropertyStr> GetFullPropertyStrDictionary(string pathDB);
		PropertyStrs GetPropertyStrs(int topCount, string pathDB);
		PropertyStrs GetPropertyStrs(SelectParams selectParams, string pathDB);
		void Delete(PropertyStr propertyStr, string pathDB);
		void DeleteAll(string pathDB);
		void DeleteAllByDomainObjectType(string domainObjectType, string pathDB);
		void Insert(PropertyStr propertyStr, string pathDB);
		void Update(PropertyStr propertyStr, string pathDB);
		PropertyStr GetPropertyStrByName(string name, string pathDB);
		PropertyStr GetPropertyStrByCode(string code, string pathDB);
		Dictionary<string, PropertyStr> GetPropertyStrDictionary(string domainObjectType, string pathDB, bool refill = false);
		Dictionary<string, string> GetPropertyStrDictionaryByCodeAsKey(string domainObjectType, string pathDB);
		Dictionary<string, string> GetNamePropertyStrDictionaryByDomainAndCodeAsKey(string pathDB);
		Dictionary<string, PropertyStr> GetPropertyStrDictionaryByDomainAndCodeAsKey(string pathDB);
		Dictionary<Pair<string, string, string>, PropertyStr> GetDictionaryDecoratorProperty(string domainObjectType, string pathDB);
		Dictionary<string, PropertyStr> GetPropertyExportErpDecoratorProperty(string domainObjectType, string pathDB);
		Dictionary<string, string> GetDictionaryProfileProperty(string domainObjectType, string XPath, string pathDB);
		string GetUIDKey_mINV(string pathDB);
		string GetUIDKey_Count4U(string pathDB);

		void ClearPropertyStrDictionary();
		void AddPropertyStrInDictionary(string code, PropertyStr propertyStr);
		void RemovePropertyStrFromDictionary(string code);
		bool IsExistPropertyStrInDictionary(string code);
		PropertyStr GetPropertyStrByCodeFromDictionary(string code);
		void FillPropertyStrDictionary(string domainObjectType, string pathDB);
	}
}
