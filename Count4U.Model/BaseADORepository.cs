using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Audit;
using System.Data.SqlServerCe;
using NLog;

namespace Count4U.Model
{
	public abstract class BaseADORepository
	{
		private readonly ILog _log;

		private readonly IConnectionADO _connectionADO;
		private readonly IDBSettings _dbSettings;

		public readonly IServiceLocator _serviceLocator;
	//	public static IturAnalyzesRepositoryEnum CurrentIturAnalyzesRepository = IturAnalyzesRepositoryEnum.IturAnalyzesADORepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public BaseADORepository(
			IConnectionADO connectionADO,
			IDBSettings dbSettings,
			ILog log,
			IServiceLocator serviceLocator)
		{
			if (connectionADO == null) throw new ArgumentNullException("connectionADO");
			this._connectionADO = connectionADO;
			this._dbSettings = dbSettings;
			if (log == null) throw new ArgumentNullException("log");
			this._log = log;
			this._serviceLocator = serviceLocator;
		}

		public ILog Log
		{
			get { return this._log; }
		}


		public IConnectionADO ConnectionADO
		{
			get { return _connectionADO; }
		}

		public IDBSettings DbSettings
		{
			get { return _dbSettings; }
		} 


		public string BuildADOConnectionStringBySubFolder(string subFolder, string fileDB = "Count4UDB.sdf")
		{
			return this._connectionADO.GetADOConnectionStringBySubFolder(subFolder, fileDB);
		}

		public void AlterTableIturAnalyzes(string pathDB)
		{
			IAlterADOProvider alterAdoProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
			//IContextCBIRepository contextCBIRepository = this._serviceLocator.GetInstance<IContextCBIRepository>();
			List<string> relativePathList = new List<string>();
			//relativePathList.Add(contextCBIRepository.BuildRelativeDbPath(currentInventor));
			relativePathList.Add(pathDB);
			alterAdoProvider.AlterTableIturAnalyzesCount4UDBViaScript(relativePathList);
		}

		public void ClearIturAnalyzes(string pathDB)
		{
			//this.Log.Clear();
			string sql1 = "DELETE FROM  [IturAnalyzes]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Product", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "IturAnalyzes"));
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "IturAnalyzes"));
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearIturAnalyzes", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}

			long count = CountRow("IturAnalyzes", pathDB);
			//clear Cash in EF
			//IIturAnalyzesRepository iturAnalyzesRepository = _serviceLocator.GetInstance<IIturAnalyzesRepository>();
			//iturAnalyzesRepository.DeleteAll(pathDB);
			//long count1 = CountRow("IturAnalyzes", pathDB);
			//LogPrint();
		}

		protected long CountRow(string tableName, string toPathDB)
		{
			string connectionString = this.BuildADOConnectionStringBySubFolder(toPathDB);

			using (SqlCeConnection sourceConnection =
					   new SqlCeConnection(connectionString))
			{
				sourceConnection.Open();

				// Perform an initial count on the destination table.
				SqlCeCommand commandRowCount = new SqlCeCommand(
					"SELECT COUNT(*) FROM " + tableName + ";",
					sourceConnection);
				long count = System.Convert.ToInt32(
					commandRowCount.ExecuteScalar());
				sourceConnection.Close();
				return count;
			}

		}
	
	}
}
