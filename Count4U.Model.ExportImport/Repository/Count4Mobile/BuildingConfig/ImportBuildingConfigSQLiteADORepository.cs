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

namespace Count4U.Model.Count4U
{
	public class ImportBuildingConfigSQLiteADORepository : BaseImportADORepository, IImportBuildingConfigSQLiteADORepository
	{
		private IBuildingConfigParser _buildingConfigParser;
  	//	private readonly ILocationRepository _locationRepository;
 		//private Dictionary<string, Location> _locationDictionary;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportBuildingConfigSQLiteADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log)
			: base(connection, dbSettings,log, serviceLocator)
        {
	    }


		public void InsertBuildingConfig(string fromPathFile, string toPathDB3,
			BuildingConfigParserEnum buildingConfigParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._buildingConfigParser = this._serviceLocator.GetInstance<IBuildingConfigParser>(buildingConfigParserEnum.ToString());

			if (this._buildingConfigParser == null)
			{
				//Localization.Resources.Log_Error1009%"In  LocationParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1009, buildingConfigParserEnum));
				return;
			}

			//if (File.Exists(fromPathFile) == false)
			//{
			//	this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
			//	return;
			//}

			toPathDB3 = this.ConnectionADO.CopyEmptyMobileDB(toPathDB3);	   //в ту же папку откуда берем файл источник
			Dictionary<string, string> buildingConfigFromDBDictionary = new Dictionary<string, string>();
			if (importType.Contains(ImportDomainEnum.ExistCode) == true)
			{
				buildingConfigFromDBDictionary = this.GetBuildingConfigDictionary(encoding, toPathDB3);//new Dictionary<string, string>();
			}

			this.Log.Add(MessageTypeEnum.TraceRepository, "ImportBuildingConfig Repository is [ImportBuildingConfigSQLiteADORepository]");
			//this.Log.Add("InventIturParser is [ " + inventProductSimpleParserEnum.ToString() + "]");
	 //   INSERT INTO [BuildingConfig]
	 //	  ([Uid]
	 //	  ,[name]
	 //	  ,[ord])
	 //VALUES
	 //	  (<Uid, text,>
	 //	  ,<name, text,>
	 //	  ,<ord, int,>);

			string sql1 = "INSERT INTO [BuildingConfig](" +
			"[Uid]" +
		   ",[Name]" +					
			",[Ord]" +
			",[name_en]" +
			",[name_he]" +	
			")" + 
      " VALUES(" +
			":Uid" +
			",:Name" +
		   ",:Ord" +
			",:NameEn" +
			",:NameHe" +
			")";

			SQLiteTransaction tran = null;
			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + toPathDB3 + @";Pooling=false;"; 
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
						"Name", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Ord", SQLiteType.Int32));
						cmd.Parameters.Add(new SQLiteParameter(
					"NameEn", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
					"NameHe", SQLiteType.Text));

						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "BuildingConfig", toPathDB3));

						foreach (KeyValuePair<string, BuildingConfig> keyValuePair in
							this._buildingConfigParser.GetBuildingConfigs(fromPathFile, encoding, separators,
							countExcludeFirstString,
							buildingConfigFromDBDictionary,
							 DomainObjectTypeEnum.BuildingConfig,
							 parms))
						{
							k++;
							string key = keyValuePair.Key;
							BuildingConfig val = keyValuePair.Value;
							cmd.Parameters["Uid"].Value = val.Uid;
							cmd.Parameters["Name"].Value = val.Name;
							cmd.Parameters["Ord"].Value = val.Ord;
							cmd.Parameters["NameEn"].Value = val.NameEn;
							cmd.Parameters["NameHe"].Value = val.NameHe;

							cmd.ExecuteNonQuery();
						}
					}
					//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportBuildingConfigSQLiteADORepository"));
					tran.Commit();
					//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
					this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "BuildingConfig", toPathDB3));
					//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}]"
					this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportBuildingConfigSQLiteADORepository"));
				}

				catch (Exception error)
				{
					_logger.ErrorException("InsertBuildingConfig", error);
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
			this.FillLogFromErrorBitList(this._buildingConfigParser.ErrorBitList);
			//LogPrint();
		}



		public void ClearBuildingConfig(string pathDB3)
		{
			if (File.Exists(pathDB3) == false) return;
				
			string sql1 = "DELETE FROM  [BuildingConfig]";
			SQLiteTransaction tran = null;

			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB3 + @"; Pooling=false;"; 
				try
				{
					sqliteConnection.Open();
					tran = sqliteConnection.BeginTransaction();
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{
						//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "BuildingConfig", pathDB3));
						//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "BuildingConfig"));
						cmd.ExecuteNonQuery();
					}
					//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "BuildingConfig"));
					tran.Commit();
				}
				catch (Exception error)
				{
					_logger.ErrorException("ClearBuildingConfig", error);
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


		public Dictionary<string, string> GetBuildingConfigDictionary(Encoding encoding, string pathDB)
		{
			Dictionary<string, string> dictionaryBuildingConfigCode = new Dictionary<string, string>();
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
			string tableName = "BuildingConfig";
			string[] separators = new string[] { "" };
			string[] columnNames = new string[] { "Uid", "Name", "Ord" };
			Dictionary<string, int> dictionaryColumnNumbers = new Dictionary<string, int>();
			int columCount = columnNames.Length;
			for (int i = 0; i < columCount; i++)
			{
				string columnName = columnNames[i];
				dictionaryColumnNumbers[columnName] = i;
			}

			int rowCount = 0;
			int indexName = -1;
			int indexOrd = -1;

			foreach (object[] objects in fileParser.GetRecords(pathDB,
					encoding, separators, 0, tableName))
			{
				if (objects == null) continue;
				rowCount++;
				string[] record = new string[] { "rowCount = " + rowCount };


				try
				{
					record = objects as string[];
				}
				catch
				{
					continue;
				}
				if (record == null) continue;

				if (rowCount == 1) // header of Table
				{
					indexName = "Name".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexOrd = "Ord".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
				}

				string name = record[indexName].Trim();
				string ord = record[indexOrd].Trim();
				if (string.IsNullOrWhiteSpace(name) == false)
				{
					dictionaryBuildingConfigCode[name] = ord;
				}
			}
			return dictionaryBuildingConfigCode;
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
