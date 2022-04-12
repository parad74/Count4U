using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Interface
{
	public interface IImportLocationProvider //: IImportLog
	{
		void InsertLocations(string fromPathFile, string pathDB, 
			Encoding encoding, string[] separators, int countExcludeFirstString);
		void ClearLocations(string pathDB);
	}
}
