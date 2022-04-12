using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.UserSettings;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using NLog;

namespace Count4U.Modules.Prepare.ViewModules
{
	public class SqlScriptSettingsViewModel : NotificationObject, INavigationAware
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly InteractionRequest<OpenFileDialogNotification> _fileChooseDilogRequest;
		private readonly InteractionRequest<SaveFileDialogNotification> _fileSaveDialogRequest;

		private readonly IContextCBIRepository _contextCBIRepository;
		private readonly ILog _logImport;
		private readonly IDBSettings _dbSettings;
		protected readonly IAlterADOProvider _alterADOProvider;
		protected readonly ISQLScriptRepository _sqlScriptRepository;
		private readonly IUserSettingsManager _userSettingsManager;

		private string _dbTypeSelectedItem;
		private readonly DelegateCommand _runCommand;
		private readonly DelegateCommand _setVerCommand;
		private readonly DelegateCommand _saveCommand;
		private readonly DelegateCommand _loadCommand;

		private string _sql;
		private string _log;

		private bool _isDbVersionMode;
		private bool _isDbVersionSetupMode;
		private bool _isSqlScriptMode;

		private int _dbVersionFrom;
		private int _dbVersionTo;
		private int _dbVersionSetupTo;

		public SqlScriptSettingsViewModel(
				IContextCBIRepository contextCBIRepository,
				IAlterADOProvider alterADOProvider,
				ISQLScriptRepository sqlScriptRepository,
				ILog logImport,
				IDBSettings dbSettings,
				IUserSettingsManager userSettingsManager)
		{
			this._userSettingsManager = userSettingsManager;
			this._dbSettings = dbSettings;
			this._logImport = logImport;
			this._contextCBIRepository = contextCBIRepository;
			this._alterADOProvider = alterADOProvider;
			this._sqlScriptRepository = sqlScriptRepository;
			this._setVerCommand = new DelegateCommand(this.SetVerCommandExecuted);
			this._runCommand = new DelegateCommand(this.RunCommandExecuted);
			this._saveCommand = new DelegateCommand(this.SaveCommandExecuted);
			this._loadCommand = new DelegateCommand(this.LoadCommandExecuted);

			this._isSqlScriptMode = true;
			this._dbVersionFrom = 0;
			this._dbVersionTo = 0;
			this._dbVersionSetupTo = 23;

			this._fileChooseDilogRequest = new InteractionRequest<OpenFileDialogNotification>();
			this._fileSaveDialogRequest = new InteractionRequest<SaveFileDialogNotification>();
		}

		public ObservableCollection<string> DbTypes
		{
			get
			{
				return new ObservableCollection<string>() { DBName.AuditDB, DBName.Count4UDB,
					DBName.EmptyCount4UDB, DBName.MainDB, DBName.ProcessDB,  DBName.AnalyticDB, DBName.EmptyAnalyticDB};
			}
		}

		public string DbTypeSelectedItem
		{
			get { return this._dbTypeSelectedItem; }
			set
			{
				this._dbTypeSelectedItem = value;
				this.RaisePropertyChanged(() => this.DbTypeSelectedItem);
			}
		}

		public DelegateCommand RunCommand
		{
			get { return this._runCommand; }
		}

		public DelegateCommand SetVerCommand
		{
			get { return this._setVerCommand; }
		}

		public DelegateCommand SaveCommand
		{
			get { return this._saveCommand; }
		}

		public DelegateCommand LoadCommand
		{
			get { return this._loadCommand; }
		}

		public string Sql
		{
			get { return this._sql; }
			set
			{
				this._sql = value;
				this.RaisePropertyChanged(() => this.Sql);
			}
		}

		public string Log
		{
			get { return this._log; }
			set
			{
				this._log = value;
				this.RaisePropertyChanged(() => this.Log);
			}
		}

		public bool IsDbVersionMode
		{
			get { return this._isDbVersionMode; }
			set
			{
				this._isDbVersionMode = value;
				this.RaisePropertyChanged(() => this.IsDbVersionMode);

				this._isSqlScriptMode = !this._isDbVersionMode;
				this._isDbVersionSetupMode = !this._isDbVersionMode;
				this.RaisePropertyChanged(() => this.IsSqlScriptMode);
				this.RaisePropertyChanged(() => this.IsDbVersionSetupMode);
			}
		}

		public bool IsDbVersionSetupMode
		{
			get { return this._isDbVersionSetupMode; }
			set
			{
				this._isDbVersionSetupMode = value;
				this.RaisePropertyChanged(() => this.IsDbVersionSetupMode);

				this._isSqlScriptMode = !this._isDbVersionSetupMode;
				this._isDbVersionMode = !this._isDbVersionSetupMode;
				this.RaisePropertyChanged(() => this.IsSqlScriptMode);
				this.RaisePropertyChanged(() => this.IsDbVersionMode);
			}
		}

		public bool IsSqlScriptMode
		{
			get { return this._isSqlScriptMode; }
			set
			{
				this._isSqlScriptMode = value;
				this.RaisePropertyChanged(() => this.IsSqlScriptMode);

				this._isDbVersionMode = !this._isSqlScriptMode;
				this._isDbVersionSetupMode = !this._isSqlScriptMode;
				this.RaisePropertyChanged(() => this.IsDbVersionMode);
				this.RaisePropertyChanged(() => this.IsDbVersionSetupMode);
			}
		}

		public int DbVersionFrom
		{
			get { return this._dbVersionFrom; }
			set
			{
				this._dbVersionFrom = value;
				this.RaisePropertyChanged(() => this.DbVersionFrom);
			}
		}

		public int DbVersionTo
		{
			get { return this._dbVersionTo; }
			set
			{
				this._dbVersionTo = value;
				this.RaisePropertyChanged(() => this.DbVersionTo);
			}
		}

		public int DbVersionSetupTo
		{
			get { return this._dbVersionSetupTo; }
			set
			{
				this._dbVersionSetupTo = value;
				this.RaisePropertyChanged(() => this.DbVersionSetupTo);
			}
		}
		public InteractionRequest<OpenFileDialogNotification> FileChooseDilogRequest
		{
			get { return this._fileChooseDilogRequest; }
		}

		public InteractionRequest<SaveFileDialogNotification> FileSaveDialogRequest
		{
			get { return this._fileSaveDialogRequest; }
		}

		#region Implementation of INavigationAware

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			this.DbTypeSelectedItem = this.DbTypes.FirstOrDefault();
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return false;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{

		}

		#endregion

		private void LoadCommandExecuted()
		{
			OpenFileDialogNotification notification = new OpenFileDialogNotification();

			notification.Filter =
				"All files (*.*)|*.*";

			this._fileChooseDilogRequest.Raise(notification, r =>
			   {
				   if (r.IsOK == true)
				   {
					   if (File.Exists(r.FileName) == true)
					   {
						   try
						   {
							   this.Sql = File.ReadAllText(r.FileName);
						   }
						   catch (Exception exc)
						   {
							   _logger.ErrorException("LoadCommandExecuted", exc);
						   }
					   }
				   }
			   });
		}

		private void SaveCommandExecuted()
		{
			SaveFileDialogNotification notification = new SaveFileDialogNotification();

			notification.Filter =
				"All files (*.*)|*.*";

			this._fileSaveDialogRequest.Raise(notification, r =>
				  {
					  if (r.IsOK == true)
					  {
						  File.WriteAllText(r.FileName, Sql);
					  }
				  });
		}

		private void SetVerCommandExecuted()
		{
			this._logImport.Clear();
			if (DbVersionSetupTo < 23) DbVersionSetupTo = 23;
			int from = 23;
			this.RunScriptsInsertVerDB(from, DbVersionSetupTo); //(DbVersionSetupTo);
			//this._alterADOProvider.UpdateDBViaScript();

			//bool isIncreasedAuditDB = this._alterADOProvider.IsIncreasedVerDB("", DBName.AuditDB, "AuditDBIni");
			//bool isIncreasedMainDB = this._alterADOProvider.IsIncreasedVerDB("", DBName.MainDB, "MainDBIni");
			//bool isIncreased = this._alterADOProvider.IsIncreasedVerDB();
			Log = this._logImport.PrintLog();
		}

		private void RunCommandExecuted()
		{
			this._logImport.Clear();
			if (this._isDbVersionMode == true)
			{
				int from = this._dbVersionFrom;
				int to = this._dbVersionTo;
				//this._alterADOProvider.RunScripts(from, to);
				this.RunScripts(from, to);
			}
			else if (this._isSqlScriptMode == true)
			{
				switch (this.DbTypeSelectedItem)
				{
					case DBName.AuditDB:
						this.RunAudit(this.Sql);
						break;
					case DBName.Count4UDB:
						this.RunCount4U(this.Sql);
						break;
					case DBName.EmptyCount4UDB:
						this.RunEmptyCount4U(this.Sql);
						break;
					 	case DBName.AnalyticDB:
						this.RunAnalyticDB(this.Sql);
						break;
					case DBName.EmptyAnalyticDB:
						this.RunEmptyAnalyticDB(this.Sql);
						break;
					case DBName.MainDB:
						this.RunMain(this.Sql);
						break;
					case DBName.ProcessDB:
						this.RunProcess(this.Sql);
						break;
				}
			}
			else if (this._isDbVersionSetupMode == true)
			{
				//if (DbVersionSetupTo < 23) DbVersionSetupTo = 23;
				//?? хз - исправить
				//int from = this._dbVersionFrom;
				//int to = this._dbVersionTo;
				int from = 23;
				if (DbVersionSetupTo < 23) DbVersionSetupTo = 23;
				this.RunScriptsInsertVerDB(from, DbVersionSetupTo);
			}
			Log = this._logImport.PrintLog();
		}

		private void RunMain(string inSql)
		{
			try
			{
				//string[] sqlList = inSql.Split(';');
				//foreach (var sql in sqlList)
				//{
				//    string sql1 = sql.Trim("\r\n ".ToCharArray());
				this._alterADOProvider.AlterTable("", DBName.MainDB, inSql);
			}
			catch { }
			try
			{
				//SetupDbFolder
				this._alterADOProvider.AlterTable("SetupDb", DBName.MainDB, inSql);
				//}
			}
			catch { }
		}


		private void RunProcess(string inSql)
		{
			try
			{
				//string[] sqlList = inSql.Split(';');
				//foreach (var sql in sqlList)
				//{
				//    string sql1 = sql.Trim("\r\n ".ToCharArray());
				this._alterADOProvider.AlterTable("", DBName.ProcessDB, inSql);
			}
			catch { }
			try
			{
				//SetupDbFolder
				this._alterADOProvider.AlterTable("SetupDb", DBName.ProcessDB, inSql);
				//}
			}
			catch { }
		}

		private void RunAudit(string inSql)
		{
			try
			{
				//string[] sqlList = inSql.Split(';');
				//foreach (var sql in sqlList)
				//{
				//    string sql1 = sql.Trim("\r\n ".ToCharArray());
				this._alterADOProvider.AlterTable("", DBName.AuditDB, inSql);
			}
			catch { }
			try
			{
				this._alterADOProvider.AlterTable("SetupDb", DBName.AuditDB, inSql);
				//}
			}
			catch { }
		}

		private void RunEmptyCount4U(string inSql)
		{
			try
			{
				//string[] sqlList = inSql.Split(';');
				//foreach (var sql in sqlList)
				//{
				//    string sql1 = sql.Trim("\r\n ".ToCharArray());
				this._alterADOProvider.AlterTable("", DBName.EmptyCount4UDB, inSql);
			}
			catch { }
			try
			{
				this._alterADOProvider.AlterTable("SetupDb", DBName.EmptyCount4UDB, inSql);
				//}
			}
			catch { }
		}

		private void RunCount4U(string inSql)
		{
			//string[] sqlList = inSql.Split(';');
			//foreach (var sql in sqlList)
			//{
			//    string sql1 = sql.Trim("\r\n ".ToCharArray());

			//List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathFolderList();
			List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathList_FromMainDBAndAuditDB();
			foreach (string pathDB in dbPathList)
			{
				try
				{
					this._alterADOProvider.AlterTable(pathDB, DBName.Count4UDB, inSql);
				}
				catch { }
			}
			try
			{
				this._alterADOProvider.AlterTable("SetupDb", DBName.Count4UDB, inSql);
			}
			catch { }

			//}
		}

		private void RunEmptyAnalyticDB(string inSql)
		{
			try
			{
				//string[] sqlList = inSql.Split(';');
				//foreach (var sql in sqlList)
				//{
				//    string sql1 = sql.Trim("\r\n ".ToCharArray());
				this._alterADOProvider.AlterTable("", DBName.EmptyAnalyticDB, inSql);
			}
			catch { }
			try
			{
				this._alterADOProvider.AlterTable("SetupDb", DBName.EmptyAnalyticDB, inSql);
				//}
			}
			catch { }
		}

		private void RunAnalyticDB(string inSql)
		{
			//string[] sqlList = inSql.Split(';');
			//foreach (var sql in sqlList)
			//{
			//    string sql1 = sql.Trim("\r\n ".ToCharArray());

			//List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathFolderList();
			List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathList_FromMainDBAndAuditDB();
			foreach (string pathDB in dbPathList)
			{
				try
				{
					this._alterADOProvider.AlterTable(pathDB, DBName.AnalyticDB, inSql);
				}
				catch { }
			}
			try
			{
				this._alterADOProvider.AlterTable("SetupDb", DBName.AnalyticDB, inSql);
			}
			catch { }

			//}
		}

		//12         15
		public void RunScripts(int oldVer, int newVer, string pathFolder = "")
		{
			oldVer++;
			if (newVer < oldVer) return;
			for (int i = oldVer; i <= newVer; i++)
			{
				//SQLScripts scripts = this._sqlScriptRepository.GetScripts(oldVer, newVer);
				SQLScripts scriptsMain = this._sqlScriptRepository.GetScripts(i, DBName.MainDB);
				foreach (var script in scriptsMain)
				{
					try
					{
						this._alterADOProvider.AlterTable(pathFolder, DBName.MainDB, script.Text);
					}
					catch { }
				}

				SQLScripts scriptsAudit = this._sqlScriptRepository.GetScripts(i, DBName.AuditDB);
				foreach (var script in scriptsAudit)
				{
					try
					{
						this._alterADOProvider.AlterTable(pathFolder, DBName.AuditDB, script.Text);
					}
					catch { }
				}

				SQLScripts scriptsProcessDB = this._sqlScriptRepository.GetScripts(i, DBName.ProcessDB);
				foreach (var script in scriptsProcessDB)
				{
					try
					{
						this._alterADOProvider.AlterTable(pathFolder, DBName.ProcessDB, script.Text);
					}
					catch { }
				}

				SQLScripts scriptsCount4U = this._sqlScriptRepository.GetScripts(i, DBName.Count4UDB);
				foreach (var script in scriptsCount4U)
				{
					//List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathFolderList();
					List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathList_FromMainDBAndAuditDB();
					foreach (string pathDB in dbPathList)
					{
						try
						{
							this._alterADOProvider.AlterTable(pathDB, DBName.Count4UDB, script.Text);
						}
						catch { }
					}
				}

				SQLScripts scriptsEmptyCount4UDB = this._sqlScriptRepository.GetScripts(i, DBName.EmptyCount4UDB);
				foreach (var script in scriptsEmptyCount4UDB)
				{
					try
					{
						this._alterADOProvider.AlterTable(pathFolder, DBName.EmptyCount4UDB, script.Text);
					}
					catch { }
				}


				SQLScripts scriptsAnalyticDB = this._sqlScriptRepository.GetScripts(i, DBName.AnalyticDB);
				foreach (var script in scriptsAnalyticDB)
				{
					//List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathFolderList();
					List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathList_FromMainDBAndAuditDB();
					foreach (string pathDB in dbPathList)
					{
						try
						{
							this._alterADOProvider.AlterTable(pathDB, DBName.AnalyticDB, script.Text);
						}
						catch { }
					}
				}

				SQLScripts scriptsEmptyAnalyticDB = this._sqlScriptRepository.GetScripts(i, DBName.EmptyAnalyticDB);
				foreach (var script in scriptsEmptyAnalyticDB)
				{
					try
					{
						this._alterADOProvider.AlterTable(pathFolder, DBName.EmptyAnalyticDB, script.Text);
					}
					catch { }
				}

				//foreach (var script in scripts)
				//{
				//    switch (script.DBType)
				//    {
				//        case DBName.AuditDB:
				//            try
				//            {
				//                this._alterADOProvider.AlterTable("", DBName.AuditDB, script.Text);
				//            }
				//            catch { }
				//            break;
				//        case DBName.Count4UDB:
				//            List<string> dbPathList = this._contextCBIRepository.GetCount4UDBPathList();
				//            foreach (string pathDB in dbPathList)
				//            {
				//                try
				//                {
				//                    this._alterADOProvider.AlterTable(pathDB, DBName.Count4UDB, script.Text);
				//                }
				//                catch { }
				//            }
				//            break;
				//        case DBName.EmptyCount4UDB:
				//            try
				//            {
				//                this._alterADOProvider.AlterTable("", DBName.EmptyCount4UDB, script.Text);
				//            }
				//            catch { }
				//            break;
				//        case DBName.MainDB:
				//            try
				//            {
				//                this._alterADOProvider.AlterTable("", DBName.MainDB, script.Text);
				//            }
				//            catch { }
				//            break;
				//    }
			}
		}


		public void RunScriptsInsertVerDB(int oldVer, int newVer)//(int lastVerToSet = 23)
		{
			SQLScripts scripts = this._sqlScriptRepository.GetScripts(oldVer, newVer);
			//SQLScripts scripts1 = this._sqlScriptRepository.GetScriptsInsertVerDB(newVer);
			//foreach (var script in scripts1)
			//{
			//    scripts.Add(script);
			//}

			foreach (var script in scripts)
			{
				switch (script.DBType)
				{
					case DBName.MainDB:
						try { this._alterADOProvider.AlterTable("SetupDb", DBName.MainDB, script.Text);	} catch { }
					break;

					case DBName.AuditDB:
						try {	this._alterADOProvider.AlterTable("SetupDb", DBName.AuditDB, script.Text);  } catch { }
					break;

					case DBName.ProcessDB:
					try { this._alterADOProvider.AlterTable("SetupDb", DBName.ProcessDB, script.Text); }
					catch { }
					break;

					case DBName.Count4UDB:
						try	{ this._alterADOProvider.AlterTable("SetupDb", DBName.Count4UDB, script.Text); }	catch { }
					break;

					case DBName.EmptyCount4UDB:
						try	{ this._alterADOProvider.AlterTable("SetupDb", DBName.EmptyCount4UDB, script.Text);	} catch { }
					break;


					case DBName.AnalyticDB:
					try { this._alterADOProvider.AlterTable("SetupDb", DBName.AnalyticDB, script.Text); }
					catch { }
					break;

					case DBName.EmptyAnalyticDB:
					try { this._alterADOProvider.AlterTable("SetupDb", DBName.EmptyAnalyticDB, script.Text); }
					catch { }
					break;
				}
			}
			
		}
	}
 
}





