using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using System.Data.Objects;
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

namespace Count4U.Model.Count4U
{
	public class ImportPreviousInventorySQLiteADORepository : BaseImportADORepository, IImportPreviousInventorysSQLiteADORepository
	{
		private IPreviousInventorySQLiteParser _previousInventoryParser;
  	//	private readonly ILocationRepository _locationRepository;
 		//private Dictionary<string, Location> _locationDictionary;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportPreviousInventorySQLiteADORepository(
			IConnectionADO connection,
			IServiceLocator serviceLocator,
			ILog log)
			: base(connection, log, serviceLocator)
        {
	    }


		public void InsertPreviousInventorys(string fromPathFile, string pathDB, 
			PreviousInventorySQLiteParserEnum previousInventoryParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._previousInventoryParser = this._serviceLocator.GetInstance<IPreviousInventorySQLiteParser>(previousInventoryParserEnum.ToString());

			if (this._previousInventoryParser == null)
			{
				//Localization.Resources.Log_Error1009%"In  LocationParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1009, previousInventoryParserEnum));
				return;
			}

			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");


			pathDB = this._connectionADO.CopyEmptyMobileDB(pathDB);	   //в ту же папку откуда берем файл источник
			Dictionary<string, string> previousInventoryFromDBDictionary = this.GetPreviousInventoryCodeDictionary( encoding,  pathDB);//new Dictionary<string, string>();


			//string objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.InventorCode);
			//if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.BranchCode);
			//if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.CustomerCode);

			//string pathBD3 = Path.GetDirectoryName(fromPathFile) + @"\" + objectCode + ".db3";

			this.Log.Add(MessageTypeEnum.TraceRepository, "ImportPreviousInventory Repository is [ImportPreviousInventorySQLiteADORepository]");
			//this.Log.Add("InventIturParser is [ " + inventProductSimpleParserEnum.ToString() + "]");
	 //INSERT INTO [PreviousInventory]
	 //	  ([Uid]
	 //	  ,[SerialNumberLocal]
	 //	  ,[ItemCode]
	 //	  ,[SerialNumberSupplier]
	 //	  ,[Quantity]
	 //	  ,[PropertyStr1]
	 //	  ,[PropertyStr2]
	 //	  ,[PropertyStr3]
	 //	  ,[PropertyStr4]
	 //	  ,[PropertyStr5]
	 //	  ,[PropertyStr6]
	 //	  ,[PropertyStr7]
	 //	  ,[PropertyStr8]
	 //	  ,[PropertyStr9]
	 //	  ,[PropertyStr10]
	 //	  ,[PropertyStr11]
	 //	  ,[PropertyStr12]
	 //	  ,[PropertyStr13]
	 //	  ,[PropertyStr14]
	 //	  ,[PropertyStr15]
	 //	  ,[PropertyStr16]
	 //	  ,[PropertyStr17]
	 //	  ,[PropertyStr18]
	 //	  ,[PropertyStr19]
	 //	  ,[PropertyStr20]
	 //	  ,[LocationCode]
	 //	  ,[DateModified]
	 //	  ,[DateCreated])
	 //VALUES
	 //	  (<Uid, text,>
	 //	  ,<SerialNumberLocal, text,>
	 //	  ,<ItemCode, text,>
	 //	  ,<SerialNumberSupplier, text,>
	 //	  ,<Quantity, text,>
	 //	  ,<PropertyStr1, text,>
	 //	  ,<PropertyStr2, text,>
	 //	  ,<PropertyStr3, text,>
	 //	  ,<PropertyStr4, text,>
	 //	  ,<PropertyStr5, text,>
	 //	  ,<PropertyStr6, text,>
	 //	  ,<PropertyStr7, text,>
	 //	  ,<PropertyStr8, text,>
	 //	  ,<PropertyStr9, text,>
	 //	  ,<PropertyStr10, text,>
	 //	  ,<PropertyStr11, text,>
	 //	  ,<PropertyStr12, text,>
	 //	  ,<PropertyStr13, text,>
	 //	  ,<PropertyStr14, text,>
	 //	  ,<PropertyStr15, text,>
	 //	  ,<PropertyStr16, text,>
	 //	  ,<PropertyStr17, text,>
	 //	  ,<PropertyStr18, text,>
	 //	  ,<PropertyStr19, text,>
	 //	  ,<PropertyStr20, text,>
	 //	  ,<LocationCode, text,>
	 //	  ,<DateModified, text,>
	 //	  ,<DateCreated, text,>);


			string sql1 = "INSERT INTO [PreviousInventory](" +
			"[Uid]" +
		   ",[SerialNumberLocal]" +
		   ",[ItemCode]" +
			",[SerialNumberSupplier] " +
			",[Quantity]" +
			",[PropertyStr1]" +
			",[PropertyStr2]" +
			",[PropertyStr3]" +
			",[PropertyStr4]" +
			",[PropertyStr5]" +
			",[PropertyStr6]" +
			",[PropertyStr7]" +
			",[PropertyStr8]" +
			",[PropertyStr9]" +
			",[PropertyStr10] " +
			",[PropertyStr11] " +
			",[PropertyStr12]" +
			",[PropertyStr13]" +
			",[PropertyStr14]" +
			",[PropertyStr15]" +
			",[PropertyStr16]" +
			",[PropertyStr17]" +
			",[PropertyStr18]" +
			",[PropertyStr19]" +
			",[PropertyStr20] " +
			",[LocationCode]" +
			",[DateModified] " +
			",[DateCreated] " +
			")" + 
      " VALUES(" +
			":Uid" +
			",:SerialNumberLocal" +
		   ",:ItemCode" +
			",:SerialNumberSupplier" +
			",:Quantity" +
			",:PropertyStr1" +
			",:PropertyStr2" +
			",:PropertyStr3" +
			",:PropertyStr4" +
			",:PropertyStr5" +
			",:PropertyStr6" +
			",:PropertyStr7" +
			",:PropertyStr8" +
			",:PropertyStr9" +
			",:PropertyStr10" +
			",:PropertyStr11" +
			",:PropertyStr12" +
			",:PropertyStr13" +
			",:PropertyStr14" +
			",:PropertyStr15" +
			",:PropertyStr16" +
			",:PropertyStr17" +
			",:PropertyStr18" +
			",:PropertyStr19" +
			",:PropertyStr20" +
			",:LocationCode" +
			",:DateModified" +
			",:DateCreated" +
			")";

			SQLiteTransaction tran = null;
			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB;
				try
				{
					sqliteConnection.Open();
					tran = sqliteConnection.BeginTransaction();
					int k = 0;
					var cmd = new SQLiteCommand(sql1, sqliteConnection, tran);

					cmd.Parameters.Add(new SQLiteParameter(
					"Uid", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"SerialNumberLocal", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"ItemCode", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"SerialNumberSupplier", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"Quantity", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"PropertyStr1", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"PropertyStr2", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"PropertyStr3", SQLiteType.Text)); 
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr4", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr5", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr6", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr7", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr8", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr9", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr10", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr11", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr12", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr13", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr14", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr15", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr16", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr17", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr18", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr19", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "PropertyStr20", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "LocationCode", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "DateModified", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					 "DateCreated", SQLiteType.Text));
			
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "PreviousInventory", pathDB));

					foreach (PreviousInventory item in this._previousInventoryParser.GetPreviousInventory(fromPathFile,
					encoding, separators, countExcludeFirstString, previousInventoryFromDBDictionary, parms))
					{
						if (cancellationToken.IsCancellationRequested == true)
						{
							break;
						}
						k++;
						cmd.Parameters["Uid"].Value = item.Uid;
						cmd.Parameters["SerialNumberLocal"].Value = item.SerialNumberLocal;
						cmd.Parameters["ItemCode"].Value = item.ItemCode;
						cmd.Parameters["SerialNumberSupplier"].Value = item.SerialNumberSupplier;
						cmd.Parameters["Quantity"].Value = item.Quantity;
						cmd.Parameters["PropertyStr1"].Value = item.PropertyStr1;
						cmd.Parameters["PropertyStr2"].Value = item.PropertyStr2;
						cmd.Parameters["PropertyStr3"].Value = item.PropertyStr3;
						cmd.Parameters["PropertyStr4"].Value = item.PropertyStr4;
						cmd.Parameters["PropertyStr5"].Value = item.PropertyStr5;
						cmd.Parameters["PropertyStr6"].Value = item.PropertyStr6;
						cmd.Parameters["PropertyStr7"].Value = item.PropertyStr7;
						cmd.Parameters["PropertyStr8"].Value = item.PropertyStr8;
						cmd.Parameters["PropertyStr9"].Value = item.PropertyStr9;
						cmd.Parameters["PropertyStr10"].Value = item.PropertyStr10;
						cmd.Parameters["PropertyStr11"].Value = item.PropertyStr11;
						cmd.Parameters["PropertyStr12"].Value = item.PropertyStr12;
						cmd.Parameters["PropertyStr13"].Value = item.PropertyStr13;
						cmd.Parameters["PropertyStr14"].Value = item.PropertyStr14;
						cmd.Parameters["PropertyStr15"].Value = item.PropertyStr15;
						cmd.Parameters["PropertyStr16"].Value = item.PropertyStr16;
						cmd.Parameters["PropertyStr17"].Value = item.PropertyStr17;
						cmd.Parameters["PropertyStr18"].Value = item.PropertyStr18;
						cmd.Parameters["PropertyStr19"].Value = item.PropertyStr19;
						cmd.Parameters["PropertyStr20"].Value = item.PropertyStr20;
						cmd.Parameters["LocationCode"].Value = item.LocationCode;
						cmd.Parameters["DateModified"].Value = item.DateModified;
						cmd.Parameters["DateCreated"].Value = item.DateCreated;

						cmd.ExecuteNonQuery();
					}
					countAction(k);
					//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportPreviousInventorySQLiteADORepository"));
					tran.Commit();
					//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
					this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "PreviousInventory", pathDB));
					//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}]"
					this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportPreviousInventorySQLiteADORepository"));
				}

				catch (Exception error)
				{
					_logger.ErrorException("InsertPreviousInventorys", error);
					this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);

					tran.Rollback();
				}
				finally
				{
					sqliteConnection.Close();
				}
			}
			this.Log.Add(MessageTypeEnum.TraceRepository, "");
			this.FillLogFromErrorBitList(this._previousInventoryParser.ErrorBitList);
			//LogPrint();
		}



		public void ClearPreviousInventory(string pathDB)
		{
			if (File.Exists(pathDB) == false) return;
			string sql1 = "DELETE FROM  [PreviousInventory]";
			SQLiteTransaction tran = null;

			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB;
				try
				{
					sqliteConnection.Open();
					tran = sqliteConnection.BeginTransaction();
					var cmd = new SQLiteCommand(sql1, sqliteConnection, tran);

					//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "PreviousInventory", pathDB));
					//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "PreviousInventory"));
					cmd.ExecuteNonQuery();
					//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
					this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "PreviousInventory"));
					tran.Commit();
				}
				catch (Exception error)
				{
					_logger.ErrorException("ClearPreviousInventory", error);
					this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
					tran.Rollback();
				}
				finally
				{
					sqliteConnection.Close();
				}
			}
		}


		public Dictionary<string, string> GetPreviousInventoryCodeDictionary(Encoding encoding, string pathDB)
		{
			Dictionary<string, string> dictionaryPreviousInventory = new Dictionary<string, string>();
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
			string tableName = "PreviousInventory";
			string[] separators = new string[] { "" };
			string[] columnNames = new string[] { "Uid", "SerialNumberLocal" };
			Dictionary<string, int> dictionaryColumnNumbers = new Dictionary<string, int>();
			int columCount = columnNames.Length;
			for (int i = 0; i < columCount; i++)
			{
				string columnName = columnNames[i];
				dictionaryColumnNumbers[columnName] = i;
			}

			int rowCount = 0;
			int indexUid = -1;
			int indexSerialNumberLocal = -1;

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
					indexUid = "Uid".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexSerialNumberLocal = "SerialNumberLocal".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
				}

				string uid = record[indexUid].Trim();
				string serialNumberLocal = record[indexSerialNumberLocal].Trim();
				dictionaryPreviousInventory[serialNumberLocal] = uid;
			}
			return dictionaryPreviousInventory;
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
