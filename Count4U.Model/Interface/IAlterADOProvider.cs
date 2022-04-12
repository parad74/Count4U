using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Interface
{
	public interface IAlterADOProvider //: IImportLog
	{
		void CreateReport();
		void AlterTable(string subFolder, string fileDB, string sqlString);
		bool IsIncreasedVerDB(string subFolder, string fileDB, string tableTitle);
		bool IsIncreasedVerDB();
		void RunScripts(int oldVer, int newVer);
		void ClearTable(string subFolder, string tableTitle, string fileDB);
		void ImportMainReport(string script, bool clear = false, bool setupDB = false);
		void ImportAuditReport(string script, bool clear = false, bool setupDB = false);
		void ImportMainAdapterLink(string script, bool clear = false, bool setupDB = false);
		void ImportToMainDB(string script, string tableName, bool clear = false);
		void ImportToAuditDB(string script, string tableName, bool clear = false);
		void ImportToCount4UDB(string script, string tableName, string dbPath, bool clear = false);
		void ExportMainReport(string script);
		void ExportAuditReport(string script);
		int GetVerMainDB(string subFolder);
		int GetVerProcessDB(string subFolder);
		int GetVerAuditDB(string subFolder);
		int GetVerCount4UDB(string subFolder);
		int GetVerAnalyticDB(string subFolder);
		int GetVerDB(string subFolder, string fileDB, string tableTitle);
		void UpdateDBViaScript();
		void UpdateEmptyCount4UDBViaScript(int newVer, string fromFolder = "") ;
		void UpdateEmptyAnalyticDBViaScript(int newVer, string fromFolder = "");
		bool UpdateCount4UDBViaScript(List<string> dbPathList);
		bool UpdateAnalyticDBViaScript(List<string> dbPathList);
		void UpdateCount4UDBViaScript(int newVer)  ;
		void UpdateAnalyticDBViaScript(int newVer);
		void UpdateCount4UDBViaScript();
		void UpdateAnalyticDBViaScript();
		void UpdateAuditDBViaScript(int newVer, string fromFolder = "");
		void UpdateMainDBViaScript(int newVer, string fromFolder = "");
		void UpdateProcessDBViaScript(int newVer, string fromFolder = "");
		void AlterTableIturAnalyzesCount4UDBViaScript(List<string> dbPathList);
	}
}
