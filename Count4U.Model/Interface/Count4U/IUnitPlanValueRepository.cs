using System;	 
using Count4U.Model.Count4U;
using System.Collections.Generic;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Count4U
{
	public interface IUnitPlanValueRepository
	{
		void AddUnitPlanValueInDictionary(string unitPlanCode, UnitPlanValue unitPlanValue);
		void ClearUnitPlanValueDictionary();
		void Delete(UnitPlanValue unitPlanValue, string pathDB);
		void DeleteAll(string pathDB);
		void FillUnitPlanValueDictionary(string pathDB);
		UnitPlanValue GetUnitPlanValueByCodeFromDictionary(string code);
		UnitPlanValue GetUnitPlanValueByUnitPlanCode(string unitPlanCode, string pathDB);
		List<string> GetUnitPlanValueCodeList(string pathDB);
		Dictionary<string, UnitPlanValue> GetUnitPlanValueDictionary(string pathDB, bool refill = false);
		UnitPlanValues GetUnitPlanValues(SelectParams selectParams, string pathDB);
		UnitPlanValues GetUnitPlanValues(string pathDB);
		void Insert(UnitPlanValue unitPlanValue, string pathDB);
		bool IsExistUnitPlanValueInDictionary(string code);
		void RemoveUnitPlanValueFromDictionary(string unitPlanCode);
		void RepairUnitPlanValueCodeFromDB(string pathDB);
		void Update(UnitPlanValue unitPlanValue, string pathDB);
		void Update(UnitPlanValues unitPlanValues, string pathDB);
		void FillUnitPlanValues(string pathDB, SelectParams selectParams = null);
	}
}
