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
	public class ImportPropertyStrListSQLiteADORepository : BaseImportADORepository, IImportPropertyStrListSQLiteADORepository
	{
		private IPropertyStrListSQLiteParser _propertyStrListParser;
  	//	private readonly ILocationRepository _locationRepository;
 		//private Dictionary<string, Location> _locationDictionary;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportPropertyStrListSQLiteADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log)
			: base(connection, dbSettings, log, serviceLocator)
        {
	    }

		public void InsertPropertyStrList(string fromPathFile, 
			string toPathDB3,
			PropertyStrListSQLiteTableNameEnum tableName,
			PropStrCodeEnum columnCode,
			PropStrNameEnum columnName,
			PropertyStrListSQLiteParserEnum propertyStrListSQLiteParserEnum,
			DomainObjectTypeEnum domainObjectType,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._propertyStrListParser = this._serviceLocator.GetInstance<IPropertyStrListSQLiteParser>(propertyStrListSQLiteParserEnum.ToString());

			if (this._propertyStrListParser == null)
			{
				//Localization.Resources.Log_Error1009%"In  LocationParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1009, propertyStrListSQLiteParserEnum));
				return;
			}

			//if (File.Exists(fromPathFile) == false)
			//{
			//	this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
			//	return;
			//}

			toPathDB3 = this.ConnectionADO.CopyEmptyMobileDB(toPathDB3);	   //в ту же папку откуда берем файл источник
			Dictionary<string, string> propertyStrFromDBDictionary = new Dictionary<string, string>();
			if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
			{
				// работаем с БД SQLite - и читаем и пишем в нее
				propertyStrFromDBDictionary = this.GetPropertyStrListDictionary(
					tableName, columnCode, columnName, encoding, toPathDB3);//new Dictionary<string, string>();
			}
			this.Log.Add(MessageTypeEnum.TraceRepository, "ImportLocation Repository is [ImportPropertyStrListSQLiteADORepository]");
			//this.Log.Add("InventIturParser is [ " + inventProductSimpleParserEnum.ToString() + "]");
	
	 //	   INSERT INTO [PropertyStr10List]
	 //	  ([Uid]
	 //	  ,[PropStr10Code]
	 //	  ,[PropStr10Name])
	 //VALUES
	 //	  (<Uid, text,>
	 //	  ,<PropStr10Code, text,>
	 //	  ,<PropStr10Name, text,>);

			string sql1 = "INSERT INTO [" + tableName.ToString() +"](" +
			"[Uid]" +
		   ",[" + columnCode.ToString() + "]" +
		   ",[" + columnName.ToString() + "]" +
			")" + 
      " VALUES(" +
			":Uid" +
			",:Code" +
		   ",:Name" +
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
						"Code", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Name", SQLiteType.Text));

						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, tableName, toPathDB3));

						foreach (KeyValuePair<string, PropertyStr1> keyValuePair in
							this._propertyStrListParser.GetPropertyStrList(fromPathFile,
							encoding, separators,
							countExcludeFirstString,
							propertyStrFromDBDictionary,
							domainObjectType,
							parms))
						{
							k++;
							string key = keyValuePair.Key;
							PropertyStr1 val = keyValuePair.Value;
							cmd.Parameters["Uid"].Value = val.Uid;
							cmd.Parameters["Code"].Value = val.PropStr1Code;
							cmd.Parameters["Name"].Value = val.PropStr1Name;

							cmd.ExecuteNonQuery();
						}
					}
					//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportPropertyStrListSQLiteADORepository"));
					tran.Commit();
					//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
					this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, tableName, toPathDB3));
					//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}]"
					this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportPropertyStrListSQLiteADORepository"));
				}

				catch (Exception error)
				{
					_logger.ErrorException("InsertPropertyStrList", error);
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
			this.FillLogFromErrorBitList(this._propertyStrListParser.ErrorBitList);
			//LogPrint();
		}

	
		public void ClearPropertyStrList(string tableName, string pathDB)
		{
			if (File.Exists(pathDB) == false) return;
			string sql1 = "DELETE FROM  [" + tableName + "]";
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
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, tableName, pathDB));
						//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, tableName));
						cmd.ExecuteNonQuery();
					}
					//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, tableName));
					tran.Commit();
				}
				catch (Exception error)
				{
					_logger.ErrorException("ClearPropertyStrList", error);
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


		public Dictionary<string, string> GetPropertyStrListDictionary(
			PropertyStrListSQLiteTableNameEnum tableNameEnum,
			PropStrCodeEnum columnCodeEnum,
			PropStrNameEnum  columnNameEnum,
			Encoding encoding, string pathDB)
		{
			Dictionary<string, string> dictionaryPropertyStrList = new Dictionary<string, string>();
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
			string tableName = tableNameEnum.ToString();
			string[] separators = new string[] { "" };
			//string[] columnNames = new string[] { "PropStr10Code", "PropStr10Name" };
			string propStrCode = columnCodeEnum.ToString();
			string propStrName = columnNameEnum.ToString();
			string[] columnNames = new string[] { "Uid", propStrCode, propStrName };
			Dictionary<string, int> dictionaryColumnNumbers = new Dictionary<string, int>();
			int columCount = columnNames.Length;
			for (int i = 0; i < columCount; i++)
			{
				string columnName = columnNames[i];
				dictionaryColumnNumbers[columnName] = i;
			}

			int rowCount = 0;
			int indexPropStrCode = -1;
			int indexPropStrName = -1;

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
					indexPropStrCode = propStrCode.GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexPropStrName = propStrName.GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
				}

				string code = record[indexPropStrCode].Trim();
				string name = record[indexPropStrName].Trim();
				dictionaryPropertyStrList[code] = name;
			}
			return dictionaryPropertyStrList;
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
