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
using Devart.Data.SQLite;
using Count4U.Model.Count4Mobile;
using System.IO;
using System.Threading;
using System.Data.Common;
using Count4U.Model.Common;
using System.Data.Entity;

namespace Count4U.Model.Count4U
{
	public class ImportTemplateInventorySQLiteADORepository : BaseImportADORepository, IImportTemplateInventorySQLiteADORepository
	{
		private ITemplateInventorySQLiteParser _templateInventoryParser;
  	//	private readonly ILocationRepository _locationRepository;
 		//private Dictionary<string, Location> _locationDictionary;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportTemplateInventorySQLiteADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log)
			: base(connection, dbSettings, log, serviceLocator)
        {
			//Database.SetInitializer<AnalyticDBContext>(new AnalyticDBContextInitializer());
	    }


		public void InsertTemplateInventorys(string fromPathFile, 	 //GetDbPath		Count4U
			string toPathDB3, 															   //db3Path		  sqlite
			TemplateInventorySQLiteParserEnum templateInventoryParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._templateInventoryParser = this._serviceLocator.GetInstance<ITemplateInventorySQLiteParser>(templateInventoryParserEnum.ToString());

			if (this._templateInventoryParser == null)
			{
				//Localization.Resources.Log_Error1009%"In  LocationParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1009, templateInventoryParserEnum));
				return;
			}

			//if (File.Exists(fromPathFile) == false)
			//{
			//	this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
			//	return;
			//}

			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");


			toPathDB3 = this.ConnectionADO.CopyEmptyMobileDB(toPathDB3);	   //в ту же папку откуда берем файл источник
			//Dictionary<string, string> previousInventoryFromDBDictionary = this.GetPreviousInventoryCodeDictionary( encoding,  toPathDB3);//new Dictionary<string, string>();
			Dictionary<string, string> templateInventoryFromDBDictionary = new Dictionary<string, string>();


			string analyticDBFile = base.DbSettings.AnalyticDBFile;
			string connectionString = base.ConnectionADO.GetADOConnectionStringBySubFolder(fromPathFile, analyticDBFile);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);
		
			//string objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.InventorCode);
			//if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.BranchCode);
			//if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.CustomerCode);

			//string pathBD3 = Path.GetDirectoryName(fromPathFile) + @"\" + objectCode + ".db3";

			this.Log.Add(MessageTypeEnum.TraceRepository, "ImportTemplateInventory Repository is [ImportTemplateInventorySQLiteADORepository]");

//CREATE TABLE [TemplateInventory] (
//  [Uid] text NOT NULL
//, [Level1Code] text NULL
//, [Level2Code] text NULL
//, [Level3Code] text NULL
//, [Level4Code] text NULL
//, [ItemCode] text NOT NULL
			//, [QuantityExpected] text NOT NULL
			//, [Tag] text NOT NULL
			//, [Domain] text NOT NULL
//, CONSTRAINT [sqlite_autoindex_Templates_1] PRIMARY KEY ([Uid])
//);


	//INSERT INTO [TemplateInventory]
	//	   ([Uid]
	//	   ,[Level1Code]
	//	   ,[Level2Code]
	//	   ,[Level3Code]
	//	   ,[Level4Code]
	//	   ,[ItemCode]
	//	   ,[QuantityExpected]
	//	   ,[Tag]
	//	   ,[Domain])
	// VALUES
	//	   (<Uid, text,>
	//	   ,<Level1Code, text,>
	//	   ,<Level2Code, text,>
	//	   ,<Level3Code, text,>
	//	   ,<Level4Code, text,>
	//	   ,<ItemCode, text,>
	//	   ,<QuantityExpected, text,>
	//	   ,<Tag, text,>
	//	   ,<Domain, text,>);


			string sql1 = "INSERT INTO [TemplateInventory](" +
			"[Uid]" +
		   ",[Level1Code]" +
		   ",[Level2Code]" +
			",[Level3Code] " +
			",[Level4Code]" +
			",[ItemCode]" +
			",[QuantityExpected]" +
			",[Tag]" +
			",[Domain]" +
			")" + 
      " VALUES(" +
			":Uid" +
			",:Level1Code" +
		   ",:Level2Code" +
			",:Level3Code" +
			",:Level4Code" +
			",:ItemCode" +
			",:QuantityExpected" +
			",:Tag" +
			",:Domain" +
			")";

			SQLiteTransaction tran = null;
			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + toPathDB3 + @"; Pooling=false;"; 
				try
				{
					sqliteConnection.Open();
					tran = sqliteConnection.BeginTransaction();
					int k = 0;
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{

						cmd.Parameters.Add(new SQLiteParameter(
						"Uid", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level1Code", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level2Code", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level3Code", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level4Code", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"ItemCode", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"QuantityExpected", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Tag", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						 "Domain", SQLiteType.Text));

						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "TemplateInventory", toPathDB3));

						foreach (TemplateInventory item in this._templateInventoryParser.GetTemplateInventorys(fromPathFile,
						encoding, separators, countExcludeFirstString, templateInventoryFromDBDictionary, parms))
						{
							if (cancellationToken.IsCancellationRequested == true)
							{
								break;
							}
							k++;
							cmd.Parameters["Uid"].Value = item.Id;
							cmd.Parameters["Level1Code"].Value = item.Level1Code;
							cmd.Parameters["Level2Code"].Value = item.Level2Code;
							cmd.Parameters["Level3Code"].Value = item.Level3Code;
							cmd.Parameters["Level4Code"].Value = item.Level4Code;
							cmd.Parameters["ItemCode"].Value = item.ItemCode;
							cmd.Parameters["QuantityExpected"].Value = item.QuantityExpected;
							cmd.Parameters["Tag"].Value = item.Tag;
							cmd.Parameters["Domain"].Value = item.Domain;
						
							cmd.ExecuteNonQuery();
						}

					}
					countAction(k);
					//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportTemplateInventorySQLiteADORepository"));
					tran.Commit();
					//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
					this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "TemplateInventory", toPathDB3));
					//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}]"
					this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportTemplateInventorySQLiteADORepository"));
				}

				catch (Exception error)
				{
					_logger.ErrorException("InsertTemplateInventorys", error);
					this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);

					tran.Rollback();
				}
				finally
				{
					sqliteConnection.Close();
					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();
				}
			}
			this.Log.Add(MessageTypeEnum.TraceRepository, "");
			this.FillLogFromErrorBitList(this._templateInventoryParser.ErrorBitList);
			//LogPrint();
		}



		public void ClearTemplateInventory(string pathDB)
		{
			if (File.Exists(pathDB) == false) return;
			string sql1 = "DELETE FROM  [TemplateInventory]";
			SQLiteTransaction tran = null;

			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB + @";Pooling=false;"; 
				try
				{
					sqliteConnection.Open();
					tran = sqliteConnection.BeginTransaction();
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{

						//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "TemplateInventory", pathDB));
						//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "TemplateInventory"));
						cmd.ExecuteNonQuery();
					}
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
					sqliteConnection.Close();
					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();
				}
			}
		}

		public void VacuumTemplateInventory(string pathDB3)
		{
			if (File.Exists(pathDB3) == false) return;
			string sql1 = "VACUUM  [TemplateInventory]";
			SQLiteTransaction tran = null;

			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB3 + @";Pooling=false;"; 
				try
				{
					sqliteConnection.Open();
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("VACUUM TemplateInventory in [{0}] ", pathDB3));
						cmd.ExecuteNonQuery();
					}
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("VACUUM  [{0}] ", "TemplateInventory"));
				}
				catch (Exception error)
				{
					_logger.ErrorException("VacuumTemplateInventory", error);
					this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				}
				finally
				{
					sqliteConnection.Close();
					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();
				}
			}
		}


		//public Dictionary<string, string> GetTemplateInventoryCodeDictionary(Encoding encoding, string pathDB)
		//{
		//	Dictionary<string, string> dictionaryPreviousInventory = new Dictionary<string, string>();
		//	IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
		//	string tableName = "TemplateInventory";
		//	string[] separators = new string[] { "" };
		//	string[] columnNames = new string[] { "Uid", "SerialNumberLocal" };
		//	Dictionary<string, int> dictionaryColumnNumbers = new Dictionary<string, int>();
		//	int columCount = columnNames.Length;
		//	for (int i = 0; i < columCount; i++)
		//	{
		//		string columnName = columnNames[i];
		//		dictionaryColumnNumbers[columnName] = i;
		//	}

		//	int rowCount = 0;
		//	int indexUid = -1;
		//	int indexSerialNumberLocal = -1;

		//	foreach (object[] objects in fileParser.GetRecords(pathDB,
		//			encoding, separators, 0, tableName))
		//	{
		//		if (objects == null) continue;
		//		rowCount++;
		//		string[] record = new string[] { "rowCount = " + rowCount };


		//		try
		//		{
		//			record = objects as string[];
		//		}
		//		catch
		//		{
		//			continue;
		//		}
		//		if (record == null) continue;

		//		if (rowCount == 1) // header of Table
		//		{
		//			indexUid = "Uid".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
		//			indexSerialNumberLocal = "SerialNumberLocal".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
		//		}

		//		string uid = record[indexUid].Trim();
		//		string serialNumberLocal = record[indexSerialNumberLocal].Trim();
		//		dictionaryPreviousInventory[serialNumberLocal] = uid;
		//	}
		//	return dictionaryPreviousInventory;
		//}

	
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
