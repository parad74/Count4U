using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface
{
	public interface IImportLocationSQLiteADORepository
	{
		void InsertLocations(string fromPathFile, string toPathDB3,
			LocationSQLiteParserEnum locationParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null);
		Dictionary<string, LocationMobile> GetLocationMobileDictionary(Encoding encoding, string pathDB);
		Dictionary<string, LocationMobile> GetLocationMobileDictionaryWithStatus1And2(Encoding encoding, string pathDB);

		Dictionary<string, string> GetLocationCodeDictionary(Encoding encoding, string pathDB);
		LocationMobile GetLocationMobileByLocationCode(string LocationCode, string pathDB);
		void ClearLocations(string pathDB);
		void VacuumLocation(string pathDB3);
	}
}
