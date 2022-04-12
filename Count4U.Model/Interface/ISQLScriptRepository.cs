using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Interface
{
	public interface ISQLScriptRepository
	{
		SQLScripts GetScripts();
		SQLScripts GetScripts(int oldVer, int newVer);
		SQLScripts GetScripts(int ver);
		SQLScripts GetScripts(int ver ,  string dbName);
		SQLScripts GetScriptsInsertVerDB(int v = 22);
		void CreateReport();
	}
}
