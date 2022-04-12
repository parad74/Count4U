using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using System.Data.Entity.Core.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model;
using Count4U.Model.Count4U.Validate;
using NLog;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4Mobile;
using ErikEJ.SqlCe;
using Count4U.Model.Common;
using System.IO;
using System.Data.Entity;

namespace Count4U.Model.Count4U
{
	//Old for DbSet
	public class ImportTemplateInventoryDbSetRepository : BaseCopyBulkRepository, IImportTemplateInventoryRepository
	{
		private ITemplateInventorySQLiteParser _templateInventoryParser;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportTemplateInventoryDbSetRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log)
			: base(connection, dbSettings,  log, serviceLocator)
        {
			//Database.SetInitializer<AnalyticDBContext>(new AnalyticDBContextInitializer());
	    }


		public void InsertTemplateInventorys(string fromPathFile, string pathDB,
			TemplateInventorySQLiteParserEnum templateInventoryParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null,
			List<string[]> ColumnMappings = null)
		{
			this._templateInventoryParser = this._serviceLocator.GetInstance<ITemplateInventorySQLiteParser>(templateInventoryParserEnum.ToString());

			if (this._templateInventoryParser == null)
			{
				//Localization.Resources.Log_Error1009%"In  LocationParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1009, templateInventoryParserEnum));
				return;
			}

			if (File.Exists(fromPathFile) == false)
			{
				this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
				return;
			}

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			 Dictionary<string, string> templateInventoryFromDBDictionary = new Dictionary<string,string>();
			 this.Log.Add(MessageTypeEnum.TraceRepository, "ImportTemplateInventoryRepository is [ImportTemplateInventoryDbSetRepository]");
	
			//SqlCeTransaction tran = null;
			 //string analyticDBFile = DbSettings.AnalyticDBFile;
			 //string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB, analyticDBFile);
			 //SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);
			 //AnalyticDBContext db = new AnalyticDBContext(sqlCeConnection);
			 //string analyticPathDB = base.ConnectionADO.BuildPathFileADO(pathDB, analyticDBFile);
			 //db.TryCreateDB();

			 string analyticDBFile = base.DbSettings.AnalyticDBFile;
			 string connectionString = base.ConnectionADO.GetADOConnectionStringBySubFolder(pathDB, analyticDBFile);
			 SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);
			 using (AnalyticDBContext db = new AnalyticDBContext(sqlCeConnection))
			 {
				 db.TryCreateTemplateInventory();
			 }
			
			  try
			  {

				countAction(0);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "TemplateInventory", connectionString));
				int k = 0;
				TemplateInventorys templateInventorys = new TemplateInventorys();
				foreach (TemplateInventory item in this._templateInventoryParser.GetTemplateInventorys(fromPathFile,
					encoding, separators, countExcludeFirstString, templateInventoryFromDBDictionary, parms))
					{
						k++;
						templateInventorys.Add(item);
						if (k % 100 == 0)	countAction(k);
				}

				if (k > 0)
				{
					string tableName = @"TemplateInventory";
					List<string[]> columnMappings = null;
					DoBulkCopyMapping<TemplateInventory>(tableName, templateInventorys, true, pathDB, columnMappings, analyticDBFile);
					long count = base.CountRow(tableName, pathDB, analyticDBFile);
					countAction(count);
				}
				else
				{
					countAction(k);
				}

				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();

	
				//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportTemplateInventoryDbSetRepository"));
			
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "TemplateInventory", connectionString));
				//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportTemplateInventoryDbSetRepository"));
			}

			catch (Exception error)
			{
				_logger.ErrorException("InsertTemplateInventory", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);

			}


			this.Log.Add(MessageTypeEnum.TraceRepository, "");
		
		}


		public IEnumerable<TemplateInventory> GetTemplateInventorys(string pathDB)
		{
			string analyticDBFile = DbSettings.AnalyticDBFile;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB, analyticDBFile);

			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);
			AnalyticDBContext db = new AnalyticDBContext(sqlCeConnection);

			string analyticPathDB = base.ConnectionADO.BuildPathFileADO(pathDB, analyticDBFile);
			db.TryCreateDB();

			IEnumerable<TemplateInventory> templateInventorySet = db.TemplateInventoryDatas.AsEnumerable();
			return templateInventorySet;
		
			//foreach(PreviousInventory previousInventory in previousInventorySet)
			//{
			//	if (previousInventory == null) continue;
			//	yield return previousInventory;
			//}
		}

		public void ClearTemplateInventorys(string pathDB)
		{
			string analyticDBFile = DbSettings.AnalyticDBFile;
			string analyticPathDB = base.ConnectionADO.BuildPathFileADO(pathDB, analyticDBFile);
			if (File.Exists(analyticPathDB) == false) return;

			//this.Log.Clear();

			string sql1 = @"DELETE FROM [TemplateInventory]";
			
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB, analyticDBFile);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "TemplateInventory", connectionString));
				//Localization.Resources.Log_TraceRepository1001%"Start Process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "TemplateInventory"));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "TemplateInventory"));
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearTemplateInventory", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
		}

	
	
		public void FillLogFromErrorBitList(List<BitAndRecord> errorBitList)
		{
			if (errorBitList == null) return;
			if (errorBitList.Count == 0) return;
			//Localization.Resources.Log_TraceRepositoryResult1050%"Parser Error And Message : "
			this.Log.Add(MessageTypeEnum.TraceRepository, Localization.Resources.Log_TraceRepositoryResult1050);
			foreach (BitAndRecord bitAndRecord in errorBitList)
			{
				int bit = bitAndRecord.Bit;
				string record = bitAndRecord.Record;
				MessageTypeEnum errorType = bitAndRecord.ErrorType;
				if (errorType == MessageTypeEnum.Error)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.Error, //bitAndRecord.ErrorType.ToString() + " : " +
							LocationValidate.ConvertDataErrorCode2ErrorMessage(b) + " [ " + record + " ] ");
					}
				}

				if (errorType == MessageTypeEnum.WarningParser)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.WarningParser, //bitAndRecord.ErrorType.ToString() + " : " +
							 LocationValidate.ConvertDataErrorCode2WarningMessage(b) + " [ " + record + " ] ");
					}
				}
			}
		}





	
	}
}
