using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using System.Threading;
using Count4U.Model.Main;

namespace Count4U.Model.Interface
{
	public interface IImportUnitPlanRepository
	{
		void InsertUnitPlans(string fromPathFile, string pathDB, UnitPlanParserEnum unitPlanParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
			void ClearUnitPlans(string pathDB);
	}
}
