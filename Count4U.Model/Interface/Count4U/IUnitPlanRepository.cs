using System;
using Count4U.Model.Count4U;
using System.Collections.Generic;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Count4U
{
	public interface IUnitPlanRepository
	{
		void AddUnitPlanInDictionary(string unitPlanCode, UnitPlan unitPlan);
		void ClearUnitPlanDictionary();
		void Delete(UnitPlan unitPlan, string pathDB);
		void DeleteAll(string pathDB);
		Dictionary<string, UnitPlan> FillUnitPlanDictionary(string pathDB);
		long GetMaxUPCode(string pathDB);
		UnitPlan GetUnitPlanByCodeFromDictionary(string code);
		UnitPlan GetUnitPlanByUnitPlanCode(string unitPlanCode, string pathDB);
		List<string> GetUnitPlanCodeList(string pathDB);
		Dictionary<string, UnitPlan> GetUnitPlanDictionary(string pathDB, bool refill = false);
		UnitPlans GetUnitPlans(SelectParams selectParams, string pathDB);
		UnitPlans GetUnitPlans(string pathDB);
		void Insert(UnitPlan unitPlan, string pathDB);
		void Insert(Dictionary<string, UnitPlan> unitPlanToDBDictionary, string pathDB);
		bool IsExistUnitPlanInDictionary(string code);
		void RemoveUnitPlanFromDictionary(string code);
		void RepairUnitPlanCodeFromDB(string pathDB);
		void Update(UnitPlan unitPlan, string pathDB);
	}
}
