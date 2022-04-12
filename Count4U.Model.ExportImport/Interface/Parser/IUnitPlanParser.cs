using System;
using System.Collections.Generic;
using Count4U.Model.Count4U;
using System.Text;

namespace Count4U.Model.Interface.Count4U
{
	public interface IUnitPlanParser
	{
		Dictionary<string, UnitPlan> GetUnitPlans(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			List<ImportDomainEnum> importType,
		Dictionary<string, UnitPlan> unitPlanFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, UnitPlan> UnitPlanDictionary { get; }
		List<BitAndRecord> ErrorBitList { get; }
	}
}
