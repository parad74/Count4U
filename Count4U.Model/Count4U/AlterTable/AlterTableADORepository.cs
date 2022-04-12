using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using NLog;
using Count4U.Model.Audit;
using System.Threading;
using System.IO;
using Count4U.Localization;

namespace Count4U.Model.Count4U
{
	public class AlterTableADORepository : BaseADORepository, IAlterADOProvider
	{
		protected readonly ISQLScriptRepository _sqlScriptRepository;
		private readonly IContextCBIRepository _contextCBIRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		protected IConnectionDB _connectionDB;

		public AlterTableADORepository(
			IConnectionDB connectionDB,
			IContextCBIRepository contextCBIRepository,
			IConnectionADO connection,
			IDBSettings dbSettings,
			ISQLScriptRepository sqlScriptRepository,
			IServiceLocator serviceLocator,
			ILog log
			)
			: base(connection, dbSettings, log, serviceLocator)
		{
			this._sqlScriptRepository = sqlScriptRepository;
			this._contextCBIRepository = contextCBIRepository;
			this._connectionDB = connectionDB;
		}

		public void ImportMainReport(string script, bool clear = false, bool setupDB = false)
		{
			string startFolder = "";
			if (setupDB == true)
			{
				startFolder = "SetupDb";
			}

			if (clear == true)
			{
				string sqlText = @"DELETE FROM  [Report];";
				this.AlterTable(startFolder, DBName.MainDB, sqlText);
			}
			//string sqlTextInsert = @"INSERT INTO [Report] ";
			//sqlTextInsert = sqlTextInsert + script;
			this.AlterTable(startFolder, DBName.MainDB, script);
	
		}

		public void ImportMainAdapterLink(string script, bool clear = false, bool setupDB = false)
		{
			string startFolder = "";
			if (setupDB == true)
			{
				startFolder = "SetupDb";
			}

			if (clear == true)
			{
				string sqlText = @"DELETE FROM  [ImportAdapter];";
				this.AlterTable(startFolder, DBName.MainDB, sqlText);
			}
			//string sqlTextInsert = @"INSERT INTO [Report] ";
			//sqlTextInsert = sqlTextInsert + script;
			this.AlterTable(startFolder, DBName.MainDB, script);

		}

		public void ImportAuditReport(string script, bool clear = false, bool setupDB = false)
		{
			string startFolder = "";
			if (setupDB == true)
			{
				startFolder = "SetupDb";
			}

			if (clear == true)
			{
				string sqlText = @"DELETE FROM  [AuditReport];";
				this.AlterTable(startFolder, DBName.AuditDB, sqlText);
			}
			//string sqlTextInsert = @"INSERT INTO [AuditReport] ";
			//sqlTextInsert = sqlTextInsert + script;
			
			this.AlterTable(startFolder, DBName.AuditDB, script);
		
		}


		//        @"INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',null,null,null,N'Itur\Doc\PDA',N'Comparative_Report.rdlc',null,N'From PDA',1,N'Corparative Report');
		//INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',N'Document Report',null,null,N'Itur\Doc\PDA',N'DOCUMENT_REPORT.rdlc',null,N'From PDA',1,N'Document Report');
		//INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',null,null,null,N'Itur\Doc\PDA',N'Detailed_Report_BranchXXX.rdlc',null,N'From PDA',1,N'Detailed Branch');
		//INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',N'Catalog1',null,null,N'Catalog',N'Catalog1.rdlc',null,N'Catalog',0,N'');
		//INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'2b588315-4b14-4709-bac9-55f5e303721b',null,null,null,N'Catalog',N'Catalog2.rdlc',null,N'Catalog',0,null);
		//INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',null,null,null,N'Itur\Doc\PDA',N'Summary_Report_BranchXXX.rdlc',null,N'From PDA',1,N'Summary Report Branch ');
		//INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',null,null,null,N'Itur\Doc\PDA',N'Summary_Report_BranchXXX_With_Departments-Sections.rdlc',null,N'From PDA',1,N'Summary Report Branch - Section');
		//"

		public void ExportMainReport(string script)
		{

		}

		public void ExportAuditReport(string script)
		{

		}

		public void ImportToCount4UDB(string script, string tableName, string dbPath, bool clear = false)
		{
			if (clear == true)
			{
				string sqlText = @"DELETE FROM  " + tableName + ";";
				this.AlterTable(dbPath, DBName.Count4UDB, sqlText);
			}
			this.AlterTable(dbPath, DBName.Count4UDB, script);
		}

		public void ImportToMainDB(string script, string tableName, bool clear = false)
		{
			if (clear == true)
			{
				string sqlText = @"DELETE FROM  " + tableName + ";";
				this.AlterTable("", DBName.MainDB, sqlText);
			}
			//string sqlTextInsert = @"INSERT INTO [Report] ";
			//sqlTextInsert = sqlTextInsert + script;
			this.AlterTable("", DBName.MainDB, script);
		}

		public void ImportToAuditDB(string script, string tableName, bool clear = false)
		{
			if (clear == true)
			{
				string sqlText = @"DELETE FROM  " + tableName + ";";
				this.AlterTable("", DBName.AuditDB, sqlText);
			}
			//string sqlTextInsert = @"INSERT INTO [Report] ";
			//sqlTextInsert = sqlTextInsert + script;
			this.AlterTable("", DBName.AuditDB, script);
		}

		public void ImportMainImportAdapter(string script, bool clear = false)
		{
			if (clear == true)
			{
				string sqlText = @"DELETE FROM  [ImportAdapter];";
				this.AlterTable("", DBName.MainDB, sqlText);
			}
			//string sqlTextInsert = @"INSERT INTO [Report] ";
			//sqlTextInsert = sqlTextInsert + script;
			this.AlterTable("", DBName.MainDB, script);
		}

		public void AlterTable(string subFolder, string fileDB, string sqlString)
		{
			if (string.IsNullOrWhiteSpace(sqlString) == true) return;

			SqlCeTransaction tran = null;

			string connectionString = this.BuildADOConnectionStringBySubFolder(subFolder, fileDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);
			//Localization.Resources.Log_TraceRepositoryResult1001%"subFolder : [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1001, subFolder));
			//Localization.Resources.Log_TraceRepositoryResult1002%"fileDB : [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1002, fileDB));

			string[] sqlList = sqlString.Split(';');
			foreach (var sql in sqlList)
			{
				string sql1 = sql.Trim(" \r\n\t".ToCharArray());

				if (string.IsNullOrWhiteSpace(sql1) == false)
				{
					//Localization.Resources.Log_TraceRepositoryResult1003%"Sql : [{0}]"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, sql1));
					try
					{
						sqlCeConnection.Open();
						tran = sqlCeConnection.BeginTransaction();
						var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

						cmd.ExecuteNonQuery();
						//Localization.Resources.Log_TraceRepositoryResult1004%"Start Commit process "
						this.Log.Add(MessageTypeEnum.TraceRepositoryResult, Localization.Resources.Log_TraceRepositoryResult1004);
						tran.Commit();
						//Localization.Resources.Log_TraceRepositoryResult1005%"End Commit process"
						this.Log.Add(MessageTypeEnum.TraceRepositoryResult, Localization.Resources.Log_TraceRepositoryResult1005);
					}

					catch (Exception error)
					{
						_logger.ErrorException("AlterTable sql:[" + sql1 + "]", error);
						this.Log.Add(MessageTypeEnum.ErrorDB, "AlterTable" + error.Message + " SQL:" + sqlString + ":" + error.StackTrace);
						tran.Rollback();
#if DEBUG
#else
		throw new Exception("AlterTable [" + sql1 + "]", error);
#endif

					}
					finally
					{
						sqlCeConnection.Close();
					}
				}// IsNullOrWhiteSpace
			}	   //foreach

		}




		public void ClearTable(string subFolder, string fileDB, string tableTitle)
		{
			//this.Log.Clear();
			string sql1 = "DELETE FROM  [" + tableTitle + "]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(subFolder, fileDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

				//Localization.Resources.Log_TraceRepositoryResult1006%"[{0}] in DB {1}\\{2}"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1006, tableTitle, subFolder, fileDB));
				//Localization.Resources.Log_TraceRepository1001% "Start process: Clear Table [{0}]  Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, tableTitle));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1007%"End process Clear "
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, Localization.Resources.Log_TraceRepositoryResult1007);
				tran.Commit();
			}
			catch (Exception error)
			{
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
#if DEBUG
#else
				throw new Exception("ClearTable [" + sql1 + "]", error);
#endif
			}
			finally
			{
				sqlCeConnection.Close();
			}
			//LogPrint();
		}

		//14         15
		public void RunScripts(int oldVer, int newVer)
		{
			oldVer++;
			if (newVer < oldVer) return;
			for (int i = oldVer; i <= newVer; i++)
			{
				//SQLScripts scripts = this._sqlScriptRepository.GetScripts(oldVer, newVer);
				SQLScripts scripts = this._sqlScriptRepository.GetScripts(i);

				SQLScripts scriptsMain = this._sqlScriptRepository.GetScripts(i, DBName.MainDB);
				foreach (var script in scriptsMain)
				{
					this.AlterTable("", DBName.MainDB, script.Text);
				}

				SQLScripts scriptsAudit = this._sqlScriptRepository.GetScripts(i, DBName.AuditDB);
				foreach (var script in scriptsAudit)
				{
					this.AlterTable("", DBName.AuditDB, script.Text);
				}

				SQLScripts scriptsProcess = this._sqlScriptRepository.GetScripts(i, DBName.ProcessDB);
				foreach (var script in scriptsProcess)
				{
					this.AlterTable("", DBName.ProcessDB, script.Text);
				}

				SQLScripts scriptsCount4U = this._sqlScriptRepository.GetScripts(i, DBName.Count4UDB);
				foreach (var script in scriptsCount4U)
				{
					List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathFolderList();
					foreach (string pathDB in dbPathList)
					{
						this.AlterTable(pathDB, DBName.Count4UDB, script.Text);
					}
				}

				SQLScripts scriptsEmptyCount4UDB = this._sqlScriptRepository.GetScripts(i, DBName.EmptyCount4UDB);
				foreach (var script in scriptsEmptyCount4UDB)
				{
					this.AlterTable("", DBName.EmptyCount4UDB, script.Text);
				}


				SQLScripts scriptsAnalyticDB = this._sqlScriptRepository.GetScripts(i, DBName.AnalyticDB);
				foreach (var script in scriptsAnalyticDB)
				{
					List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathFolderList();
					foreach (string pathDB in dbPathList)
					{
						this.AlterTable(pathDB, DBName.AnalyticDB, script.Text);
					}
				}

				SQLScripts scriptsEmptyAnalyticDB = this._sqlScriptRepository.GetScripts(i, DBName.EmptyAnalyticDB);
				foreach (var script in scriptsEmptyAnalyticDB)
				{
					this.AlterTable("", DBName.EmptyAnalyticDB, script.Text);
				}
				//foreach (var script in scripts)
				//{
				//switch (script.DBType)
				//{
				//    case DBName.AuditDB:
				//        this.AlterTable("", DBName.AuditDB, script.Text);
				//        break;
				//    case DBName.Count4UDB:
				//        List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathList();
				//        foreach (string pathDB in dbPathList)
				//        {
				//            this.AlterTable(pathDB, DBName.Count4UDB, script.Text);
				//        }
				//        break;
				//    case DBName.EmptyCount4UDB:
				//        this.AlterTable("", DBName.EmptyCount4UDB, script.Text);
				//        break;
				//    case DBName.MainDB:
				//        this.AlterTable("", DBName.MainDB, script.Text);
				//        break;
				//}

			}
		}

		public void UpdateDBViaScript()
		{
			int newVer = PropertiesSettings.DBVer;
			_logger.Info("UpdateDBViaScript newVer : [" + newVer + "]");

			this.UpdateCount4UDBViaScript(newVer);

			this.AlterTableIturAnalyzesCount4UDBViaScript();

			this.UpdateAnalyticDBViaScript(newVer);
			
			this.UpdateEmptyCount4UDBViaScript(newVer, "");

			this.UpdateEmptyAnalyticDBViaScript(newVer, "");

			this.UpdateMainDBViaScript(newVer, "");

			this.UpdateAuditDBViaScript(newVer, "");

			this.UpdateProcessDBViaScript(newVer, "");
		}

		public void UpdateEmptyCount4UDBViaScript(int newVer, string fromFolder = "")
		{
			_logger.Info("UpdateEmptyCount4UDBViaScript newVer : [" + newVer + "]");
			int oldVer = this.GetVerEmptyCount4UDB(fromFolder);
			if (newVer < oldVer) return;
			_logger.Info("pathCount4UDB [" + fromFolder + "] oldVer  [" + oldVer + "] to newVer [" + newVer + "]");
			oldVer++;
			for (int i = oldVer; i <= newVer; i++)
			{
				SQLScripts scriptsEmptyCount4UDB = this._sqlScriptRepository.GetScripts(i, DBName.EmptyCount4UDB);
				foreach (var script in scriptsEmptyCount4UDB)
				{
					try
					{
						this.AlterTable(fromFolder, DBName.EmptyCount4UDB, script.Text);
					}
					catch { }//test TODO: убрать после апдейта всех
				}
			}
		}

		public void UpdateEmptyAnalyticDBViaScript(int newVer, string fromFolder = "")
		{
			_logger.Info("UpdateEmptyCount4UDBViaScript newVer : [" + newVer + "]");
			int oldVer = this.GetVerEmptyAnalyticDB(fromFolder);							
			if (newVer < oldVer) return;
			_logger.Info("pathEmptyAnalyticDB [" + fromFolder + "] oldVer  [" + oldVer + "] to newVer [" + newVer + "]");
			oldVer++;
			for (int i = oldVer; i <= newVer; i++)
			{
				SQLScripts scriptsEmptyAnalyticDB = this._sqlScriptRepository.GetScripts(i, DBName.EmptyAnalyticDB);
				foreach (var script in scriptsEmptyAnalyticDB)
				{
					try
					{
						this.AlterTable(fromFolder, DBName.EmptyAnalyticDB, script.Text);
					}
					catch { }//test TODO: убрать после апдейта всех
				}
			}
		}

		public void UpdateCount4UDBViaScript(int newVer)
		{
			bool refilledAllCBIInventorConfig = false;
			List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathDBList();
			refilledAllCBIInventorConfig = UpdateCount4UDBViaScript(newVer, refilledAllCBIInventorConfig, dbPathList);
		}

		public void UpdateAnalyticDBViaScript(int newVer)
		{
			List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathDBList();
			 UpdateAnalyticDBViaScript(newVer, dbPathList);
		}

		public void AlterTableIturAnalyzesCount4UDBViaScript()
		{
			List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathDBList();
			AlterTableIturAnalyzesCount4UDBViaScript(dbPathList);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newVer"></param>
		/// <param name="dbPathList">"\Customer\a0ab472d-28e6-4c94-8087-eb3b2bd75fee\"</param>
		public bool UpdateCount4UDBViaScript(List<string> dbPathList)
		{
			int newVer = PropertiesSettings.DBVer;
			bool refilledAllCBIInventorConfig = false;
			return UpdateCount4UDBViaScript(newVer, refilledAllCBIInventorConfig, dbPathList);
		}

		private bool UpdateCount4UDBViaScript(int newVer, bool refilledAllCBIInventorConfig, List<string> dbPathList)
		{
			foreach (string pathDB in dbPathList)
			{
				int oldVer = this.GetVerCount4UDB(pathDB);
				//if (82 < 83)
				if (newVer < oldVer) return true; 
				_logger.Info("pathCount4UDB [" + pathDB + "] oldVer  [" + oldVer + "] to newVer [" + newVer + "]");
				oldVer++;
				for (int i = oldVer; i <= newVer; i++)
				{
					SQLScripts scriptsCount4U = this._sqlScriptRepository.GetScripts(i, DBName.Count4UDB);
					if (i == 41)
					{
						if (refilledAllCBIInventorConfig == false)
						{
							IContextCBIRepository contextCBIRepository = this._serviceLocator.GetInstance<IContextCBIRepository>();
							contextCBIRepository.RefillAllCBIInventorConfigs();
							refilledAllCBIInventorConfig = true;
						}
					}
					foreach (var script in scriptsCount4U)
					{
						try
						{
							this.AlterTable(pathDB, DBName.Count4UDB, script.Text);
						}
						catch { }//test TODO: убрать после апдейта всех
					}
				}
			}
			return refilledAllCBIInventorConfig;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="newVer"></param>
		/// <param name="dbPathList">"\Customer\a0ab472d-28e6-4c94-8087-eb3b2bd75fee\"</param>
		public bool UpdateAnalyticDBViaScript(List<string> dbPathList)
		{
			int newVer = PropertiesSettings.DBVer;
			return UpdateAnalyticDBViaScript(newVer, dbPathList);
		}

		private bool UpdateAnalyticDBViaScript(int newVer,  List<string> dbPathList)
		{
			foreach (string pathDB in dbPathList)
			{
				string pathDB1 = pathDB.Replace('/', '\\');
				string analyticDBFilePath = this._connectionDB.BuildAnalyticDBFilePath(pathDB1);
				if (File.Exists(analyticDBFilePath) == false)
				{
					continue;
				}

				int oldVer = this.GetVerAnalyticDB(pathDB);
				//if (82 < 83)
				if (newVer < oldVer) return true;
				_logger.Info("pathAnalyticDB [" + pathDB + "] oldVer  [" + oldVer + "] to newVer [" + newVer + "]");
				oldVer++;
				for (int i = oldVer; i <= newVer; i++)
				{
					SQLScripts scriptsAnalyticDB = this._sqlScriptRepository.GetScripts(i, DBName.AnalyticDB);

					foreach (var script in scriptsAnalyticDB)
					{
						try
						{
							this.AlterTable(pathDB, DBName.AnalyticDB, script.Text);
						}
						catch { }//test TODO: убрать после апдейта всех
					}
				}
			}
			return true;
		}

		public void AlterTableIturAnalyzesCount4UDBViaScript(List<string> dbPathList)
		{
			foreach (string pathDB in dbPathList)
			{
				_logger.Info("AlterTableIturAnalyzesCount4UDBViaScript [" + pathDB + "] ");
				SQLScripts scriptsCount4U = this._sqlScriptRepository.GetScripts(-5, DBName.Count4UDB);
				foreach (var script in scriptsCount4U)
				{
					try
					{
						this.AlterTable(pathDB, DBName.Count4UDB, script.Text);
					}
					catch (Exception error)
					{
						this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
						_logger.Error("AlterTableIturAnalyzesCount4UDBViaScript [" + pathDB + "] " + error.Message + error.StackTrace);
					}
				}
			}
			Thread.Sleep(200);
		}

		public void UpdateCount4UDBViaScript()
		{
			int newVer = PropertiesSettings.DBVer;
			this.UpdateCount4UDBViaScript(newVer);
		}


		public void UpdateAnalyticDBViaScript()
		{
			int newVer = PropertiesSettings.DBVer;
			this.UpdateAnalyticDBViaScript(newVer);
		}

		public void UpdateAuditDBViaScript(int newVer, string fromFolder = "")
		{
			int oldVer = this.GetVerAuditDB(fromFolder);
			if (newVer < oldVer) return; 
			_logger.Info("AuditDB oldVer  [" + oldVer + "] to newVer [" + newVer + "]");
			oldVer++;
			for (int i = oldVer; i <= newVer; i++)
			{
				SQLScripts scriptsAudit = this._sqlScriptRepository.GetScripts(i, DBName.AuditDB);
				foreach (var script in scriptsAudit)
				{
					try
					{
						this.AlterTable(fromFolder, DBName.AuditDB, script.Text);
					}
					catch { }//test TODO: убрать после апдейта всех
				}
			}
		}

		public void UpdateMainDBViaScript(int newVer, string fromFolder = "")
		{
			int oldVer = this.GetVerMainDB(fromFolder);
			if (newVer < oldVer) return;
			_logger.Info("MainDB oldVer  [" + oldVer + "] to newVer [" + newVer + "]");
			oldVer++;
			for (int i = oldVer; i <= newVer; i++)
			{
				//SQLScripts scripts = this._sqlScriptRepository.GetScripts(oldVer, newVer);
				SQLScripts scripts = this._sqlScriptRepository.GetScripts(i);

				SQLScripts scriptsMain = this._sqlScriptRepository.GetScripts(i, DBName.MainDB);
				foreach (var script in scriptsMain)
				{
					try
					{
						this.AlterTable(fromFolder, DBName.MainDB, script.Text);
					}
					catch { }//test TODO: убрать после апдейта всех
				}
			}
		}

		public void UpdateProcessDBViaScript(int newVer, string fromFolder = "")
		{
			int oldVer = this.GetVerProcessDB(fromFolder);
			if (newVer < oldVer) return;
			_logger.Info("ProcessDB oldVer  [" + oldVer + "] to newVer [" + newVer + "]");
			oldVer++;
			for (int i = oldVer; i <= newVer; i++)
			{
				//SQLScripts scripts = this._sqlScriptRepository.GetScripts(oldVer, newVer);
				SQLScripts scripts = this._sqlScriptRepository.GetScripts(i);

				SQLScripts scriptsProcessDB = this._sqlScriptRepository.GetScripts(i, DBName.ProcessDB);
				foreach (var script in scriptsProcessDB)
				{
					try
					{
						this.AlterTable(fromFolder, DBName.ProcessDB, script.Text);
					}
					catch { }//test TODO: убрать после апдейта всех
				}
			}
		}


		public bool IsIncreasedVerDB(string subFolder, string fileDB, string tableTitle)
		{
			bool isIncreased = false;
			int currentDBVer = PropertiesSettings.DBVer;
			//this.Log.Clear();

			int maxVer = GetVerDB(subFolder, fileDB, tableTitle);
			//LogPrint();
			if (maxVer < currentDBVer) isIncreased = true;
			return isIncreased;
		}


		public int GetVerMainDB(string subFolder)
		{
			return this.GetVerDB(subFolder, DBName.MainDB, "MainDBIni");
		}

		public int GetVerAuditDB(string subFolder)
		{
			return this.GetVerDB(subFolder, DBName.AuditDB, "AuditDBIni");
		}


		public int GetVerProcessDB(string subFolder)
		{
			return this.GetVerDB(subFolder, DBName.ProcessDB, "ProcessDBIni");
		}

		public int GetVerCount4UDB(string subFolder)
		{
			return this.GetVerDB(subFolder, DBName.Count4UDB, "Count4UDBIni");
		}


		public int GetVerAnalyticDB(string subFolder)
		{
			if (ExistsTable(subFolder, DBName.AnalyticDB, "AnalyticDBIni") == false)
			{
				this._connectionDB.ReplaceEmptyAnaliticDB(subFolder);
				//Удалить AnalyticDB - совсем старая бд
			}
			return this.GetVerDB(subFolder, DBName.AnalyticDB, "AnalyticDBIni");
		}


		public int GetVerEmptyCount4UDB(string subFolder)
		{
			return this.GetVerDB(subFolder, DBName.EmptyCount4UDB, "Count4UDBIni");
		}

		public int GetVerEmptyAnalyticDB(string subFolder)
		{
			return this.GetVerDB(subFolder, DBName.EmptyAnalyticDB, "AnalyticDBIni");
		}



		protected bool ExistsTable(string subFolder, string fileDB, string tableName)
		{
			string connectionString = this.BuildADOConnectionStringBySubFolder(subFolder, fileDB);
			SqlCeConnection sourceConnection = new SqlCeConnection(connectionString);
			try
			{
					sourceConnection.Open();
					// Perform an initial count on the destination table.
					SqlCeCommand commandRowCount = new SqlCeCommand(
						"SELECT COUNT(*) FROM " + tableName + ";",
						sourceConnection);
					long count = System.Convert.ToInt32(
						commandRowCount.ExecuteScalar());
					sourceConnection.Close();
					return true;
			}
			catch (Exception error)
			{
				return false;
			}
			finally
			{
				sourceConnection.Close();
			}
		}

		public int GetVerDB(string subFolder, string fileDB, string tableTitle)
		{
			int maxVer = 6;
			string sql = "SELECT ID, VER, CODE  FROM  [" + tableTitle + "] ORDER BY ID DESC;";

			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(subFolder, fileDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				SqlCeCommand cmd = new SqlCeCommand(sql, sqlCeConnection, tran);

				//Localization.Resources.Log_TraceRepositoryResult1006%"[{0}] in DB {1}\\{2}"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1006, tableTitle, subFolder, fileDB));
				//Localization.Resources.Log_TraceRepository1005% "Start process: Select Ver DB [{0}]  Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1005, tableTitle));
				//LogPrint();


				SqlCeDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					string rowReader = "";
					try { rowReader = reader[1].ToString(); }
					catch
					{
						_logger.Info("GetVerDB - reader[1] : rowReader [" + rowReader + "] " + "-  maxVer [" + maxVer + "] subFolder [" + subFolder + "]");
					}
					int ver = 0;
					bool isParsing = Int32.TryParse(rowReader, out ver);
					if (ver >= maxVer)
					{
						maxVer = ver;
					}
				}
				reader.Close();
				//Localization.Resources.Log_TraceRepository1006%"End process: Select Ver DB "
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, Localization.Resources.Log_TraceRepository1006 + " maxVer [" + maxVer + "]");

				//Localization.Resources.Log_Trace_End_process_Select_Ver_DB%#%End process: Select Ver DB
				Log.Add(MessageTypeEnum.Trace, "End process: Select Ver DB");


				tran.Commit();
			}
			catch (Exception error)
			{
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				_logger.Error("GetVerDB maxVer [" + maxVer + "] subFolder [" + subFolder + "]" + error.Message + error.StackTrace);
				tran.Rollback();
#if DEBUG
#else
				throw new Exception("Select Ver DB [" + sql + "]", error);
#endif
			}
			finally
			{
				sqlCeConnection.Close();
			}
			//_logger.Info("GetVerDB " + subFolder + "  "+ fileDB + " oldVer  [" + maxVer + "]");
			return maxVer;
		}

		public bool IsIncreasedVerDB()
		{
			bool ret = false;
			//bool isIncreasedCountDB = false;
			//IContextCBIRepository contextCBIRepository = this._serviceLocator.GetInstance<IContextCBIRepository>();
			//AuditConfig auditConfig = contextCBIRepository.GetProcessCBIConfig(CBIContext.History);
			//if (auditConfig != null)
			//{
			//    Inventor inventorProcess = contextCBIRepository.GetInventorByCode(auditConfig.InventorCode);
			//    if (inventorProcess != null)
			//    {
			//        string pathDB = contextCBIRepository.GetDBPath(inventorProcess);
			//        isIncreasedCountDB = this.IsIncreasedVerDB(pathDB, DBName.Count4UDB, "Count4UDBIni");
			//    }
			//    else
			//    {
			//        isIncreasedCountDB = true;
			//    }
			//}
			//else
			//{
			//    isIncreasedCountDB = true;
			//}

			bool isIncreasedAuditDB = this.IsIncreasedVerDB("", DBName.AuditDB, "AuditDBIni");
			bool isIncreasedMainDB = this.IsIncreasedVerDB("", DBName.MainDB, "MainDBIni");
			bool isIncreasedProcessDB = this.IsIncreasedVerDB("", DBName.ProcessDB, "ProcessDBIni");
			ret = isIncreasedAuditDB || isIncreasedMainDB || isIncreasedProcessDB; // || isIncreasedCountDB;
			return ret;
		}

		public void CreateReport()
		{
			this._sqlScriptRepository.CreateReport();
		}


	}
}

