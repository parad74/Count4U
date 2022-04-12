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
using System.Threading;
using NLog;
using Count4U.Model.Count4Mobile;
using Devart.Data.SQLite;
using Count4U.Model.Interface.Count4Mobile;
using System.IO;
using Count4U.Model.Common;
using System.Data.Entity;

namespace Count4U.Model.Count4U
{
	public class ImportCurrentInventorSQLiteADORepository : BaseImportADORepository, IImportCurrentInventorSQLiteADORepository
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private ICurrentInventorSQLiteParser _сurrentInventorParser;

		public ImportCurrentInventorSQLiteADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log)
			: base(connection, dbSettings, log, serviceLocator)
        {
			//Database.SetInitializer<AnalyticDBContext>(new AnalyticDBContextInitializer());
	    }


		public void InsertCurrentInventors(string fromPathFile,   //GetDbPath		Count4U
			string toPathDB3,									//db3Path		  sqlite
			CurrentInventorSQLiteParserEnum productParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null )
		{
			this._сurrentInventorParser = this._serviceLocator.GetInstance<ICurrentInventorSQLiteParser>(productParserEnum.ToString());

			if (this._сurrentInventorParser == null)
			{
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, productParserEnum.ToString()));
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

			toPathDB3 = this.ConnectionADO.CopyEmptyMobileDB(toPathDB3);	

			Dictionary<string, string> catalogMakatDictionary = new Dictionary<string, string>();

			if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
			{
				catalogMakatDictionary = this.GetMakatName(encoding, toPathDB3);
			}

			string analyticDBFile = base.DbSettings.AnalyticDBFile;
			string connectionString = base.ConnectionADO.GetADOConnectionStringBySubFolder(fromPathFile, analyticDBFile);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);
		
			//Localization.Resources.Log_TraceRepository1040%"[{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "ImportCurrentInventorRepository", "ImportCurrentInventorSQLiteADORepository"));
			//Localization.Resources.Log_TraceRepository1040%"[{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "CurrentInventorSQLiteParser", productParserEnum.ToString()));
	//INSERT INTO [CurrentInventory]
	//	   ([Uid]
	//	   ,[SerialNumberLocal]
	//	   ,[ItemCode]
	//	   ,[SerialNumberSupplier]
	//	   ,[Quantity]
	//	   ,[PropertyStr1]
	//	   ,[PropertyStr2]
	//	   ,[PropertyStr3]
	//	   ,[PropertyStr4]
	//	   ,[PropertyStr5]
	//	   ,[PropertyStr6]
	//	   ,[PropertyStr7]
	//	   ,[PropertyStr8]
	//	   ,[PropertyStr9]
	//	   ,[PropertyStr10]
	//	   ,[PropertyStr11]
	//	   ,[PropertyStr12]
	//	   ,[PropertyStr13]
	//	   ,[PropertyStr14]
	//	   ,[PropertyStr15]
	//	   ,[PropertyStr16]
	//	   ,[PropertyStr17]
	//	   ,[PropertyStr18]
	//	   ,[PropertyStr19]
	//	   ,[PropertyStr20]
	//	   ,[LocationCode]
	//	   ,[DateModified]
	//	   ,[DateCreated]
	//	   ,[ItemStatus])
	// VALUES
	//	   (<Uid, text,>
	//	   ,<SerialNumberLocal, text,>
	//	   ,<ItemCode, text,>
	//	   ,<SerialNumberSupplier, text,>
	//	   ,<Quantity, text,>
	//	   ,<PropertyStr1, text,>
	//	   ,<PropertyStr2, text,>
	//	   ,<PropertyStr3, text,>
	//	   ,<PropertyStr4, text,>
	//	   ,<PropertyStr5, text,>
	//	   ,<PropertyStr6, text,>
	//	   ,<PropertyStr7, text,>
	//	   ,<PropertyStr8, text,>
	//	   ,<PropertyStr9, text,>
	//	   ,<PropertyStr10, text,>
	//	   ,<PropertyStr11, text,>
	//	   ,<PropertyStr12, text,>
	//	   ,<PropertyStr13, text,>
	//	   ,<PropertyStr14, text,>
	//	   ,<PropertyStr15, text,>
	//	   ,<PropertyStr16, text,>
	//	   ,<PropertyStr17, text,>
	//	   ,<PropertyStr18, text,>
	//	   ,<PropertyStr19, text,>
	//	   ,<PropertyStr20, text,>
	//	   ,<LocationCode, text,>
	//	   ,<DateModified, text,>
	//	   ,<DateCreated, text,>
	//	   ,<ItemStatus, text,>);

			string sql1 = "INSERT INTO [CurrentInventory] (" +
			"[Uid]" +
			",[SerialNumberLocal]  " +
			",[ItemCode] " +
			",[SerialNumberSupplier]	 " +
			",[Quantity] " +
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
			",[ItemStatus] " +
			")" +
			  " VALUES(" +
			 ":Uid" +
			",:SerialNumberLocal" +
			",:ItemCode"	+
			",:SerialNumberSupplier"  +
			 ",:Quantity"  +
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
			",:ItemStatus" +
			")";
					
			SQLiteTransaction tran = null;
			int k = 0;
			string uid = "";
			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + toPathDB3 + @"; Pooling=false;"; 
				try
				{
					sqliteConnection.Open();
					tran = sqliteConnection.BeginTransaction();
				
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{
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
						cmd.Parameters.Add(new SQLiteParameter(
					 "ItemStatus", SQLiteType.Text));


						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "СurrentInventory", toPathDB3));


						if (importType.Contains(ImportDomainEnum.ImportСurrentInventory) == true)
						{
							foreach (CurrentInventory сurrentInventoryItem in this._сurrentInventorParser.GetCurrentInventorys(fromPathFile,  //GetDbPath		Count4U
								encoding, separators, countExcludeFirstString, catalogMakatDictionary, importType, parms))
							{

								if (cancellationToken.IsCancellationRequested == true)
								{
									break;
								}
								k++;
								if (k == 41091)
								{
									uid = сurrentInventoryItem.Uid;
								}
								cmd.Parameters["Uid"].Value = сurrentInventoryItem.Uid;
								cmd.Parameters["SerialNumberLocal"].Value = сurrentInventoryItem.SerialNumberLocal;
								cmd.Parameters["ItemCode"].Value = сurrentInventoryItem.ItemCode;
								cmd.Parameters["SerialNumberSupplier"].Value = сurrentInventoryItem.SerialNumberSupplier;
								cmd.Parameters["Quantity"].Value = сurrentInventoryItem.Quantity;
								cmd.Parameters["PropertyStr1"].Value = сurrentInventoryItem.PropertyStr1;
								cmd.Parameters["PropertyStr2"].Value = сurrentInventoryItem.PropertyStr2;
								cmd.Parameters["PropertyStr3"].Value = сurrentInventoryItem.PropertyStr3;
								cmd.Parameters["PropertyStr4"].Value = сurrentInventoryItem.PropertyStr4;
								cmd.Parameters["PropertyStr5"].Value = сurrentInventoryItem.PropertyStr5;
								cmd.Parameters["PropertyStr6"].Value = сurrentInventoryItem.PropertyStr6;
								cmd.Parameters["PropertyStr7"].Value = сurrentInventoryItem.PropertyStr7;
								cmd.Parameters["PropertyStr8"].Value = сurrentInventoryItem.PropertyStr8;
								cmd.Parameters["PropertyStr9"].Value = сurrentInventoryItem.PropertyStr9;
								cmd.Parameters["PropertyStr10"].Value = сurrentInventoryItem.PropertyStr10;
								cmd.Parameters["PropertyStr11"].Value = сurrentInventoryItem.PropertyStr11;
								cmd.Parameters["PropertyStr12"].Value = сurrentInventoryItem.PropertyStr12;
								cmd.Parameters["PropertyStr13"].Value = сurrentInventoryItem.PropertyStr13;
								cmd.Parameters["PropertyStr14"].Value = сurrentInventoryItem.PropertyStr14;
								cmd.Parameters["PropertyStr15"].Value = сurrentInventoryItem.PropertyStr15;
								cmd.Parameters["PropertyStr16"].Value = сurrentInventoryItem.PropertyStr16;
								cmd.Parameters["PropertyStr17"].Value = сurrentInventoryItem.PropertyStr17;
								cmd.Parameters["PropertyStr18"].Value = сurrentInventoryItem.PropertyStr18;
								cmd.Parameters["PropertyStr19"].Value = сurrentInventoryItem.PropertyStr19;
								cmd.Parameters["PropertyStr20"].Value = сurrentInventoryItem.PropertyStr20;
								cmd.Parameters["LocationCode"].Value = сurrentInventoryItem.LocationCode;
								cmd.Parameters["DateModified"].Value = сurrentInventoryItem.DateModified;
								cmd.Parameters["DateCreated"].Value = сurrentInventoryItem.DateCreated;
								cmd.Parameters["ItemStatus"].Value = сurrentInventoryItem.ItemStatus;

								try
								{
									cmd.ExecuteNonQuery();
								}
								catch (Exception error)
								{
									_logger.ErrorException("InserСurrentInventorys", error);
									this.Log.Add(MessageTypeEnum.ErrorDB, "k = " + k + " ;  uid = " + uid + " ; " + error.Message);
								}
							}
						}
						countAction(k);
					}
					this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "CurrentInventory", toPathDB3));


					//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportCurrentInventorSQLiteADORepository"));
					tran.Commit();
					//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}] "
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportCurrentInventorSQLiteADORepository"));
				}
				catch (Exception error)
				{
					_logger.ErrorException("InserСurrentInventorys", error);
					this.Log.Add(MessageTypeEnum.ErrorDB, "k = " + k + " ; " + error.Message + ":" + error.StackTrace);

					tran.Rollback();
				}
				finally
				{
					sqliteConnection.Close();
					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();
				}
				this.Log.Add(MessageTypeEnum.TraceRepository, "");
				this.FillLogFromErrorBitList(this._сurrentInventorParser.ErrorBitList);
			}
	
		}

		public void ClearCurrentInventors(string pathDB3)
		{
			if (File.Exists(pathDB3) == false) return;
			string sql1 = "DELETE FROM  [CurrentInventory]";
			SQLiteTransaction tran = null;

			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB3 + @";Pooling=false;"; 
				try
				{
					sqliteConnection.Open();
					tran = sqliteConnection.BeginTransaction();
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{

						//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "CurrentInventory", pathDB3));
						//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "CurrentInventory"));
						cmd.ExecuteNonQuery();
					}
					//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "CurrentInventory"));
					tran.Commit();
				}
				catch (Exception error)
				{
					_logger.ErrorException("ClearСurrentInventorys", error);
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

		public void VacuumCurrentInventory(string pathDB3)
		{
			if (File.Exists(pathDB3) == false) return;
			string sql1 = "VACUUM  [CurrentInventory]";
			SQLiteTransaction tran = null;

			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB3 + @"; Pooling=false;"; 
				try
				{
					sqliteConnection.Open();
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("VACUUM CurrentInventory in [{0}] ", pathDB3));
						cmd.ExecuteNonQuery();
					}
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("VACUUM  [{0}] ", "CurrentInventory"));
				}
				catch (Exception error)
				{
					_logger.ErrorException("VacuumCurrentInventory", error);
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

		public Dictionary<string, string> GetMakatName(Encoding encoding, string pathDB)
		{
			Dictionary<string, string> dictionaryMakat = new Dictionary<string, string>();
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
			string tableName = "Catalog";
			string[] separators = new string[] { "" };
			string[] columnNames = new string[] { "Uid", "ItemCode", "ItemName" };
			Dictionary<string, int> dictionaryColumnNumbers = new Dictionary<string, int>();
			int columCount = columnNames.Length;
			for (int i = 0; i < columCount; i++)
			{
				string columnName = columnNames[i];
				dictionaryColumnNumbers[columnName] = i;
			}
			
			int rowCount = 0;
	 		int indexItemCode = -1;
			int indexItemName = -1;

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
					indexItemCode = "ItemCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexItemName = "ItemName".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
				}

				string makat = record[indexItemCode].Trim();
				string name = record[indexItemName].Trim();
				dictionaryMakat[makat] = name; 
			}
			return dictionaryMakat;
		}
	 
		public void FillLogFromErrorBitList(List<BitAndRecord> errorBitList)
		{
			if (errorBitList == null) return;
			if (errorBitList.Count == 0) return;
			//Log_TraceParser1001%"Parser Error And Message : "
			this.Log.Add(MessageTypeEnum.TraceParser, Localization.Resources.Log_TraceParser1001);
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
							 ProductValidate.ConvertDataErrorCode2ErrorMessage(b) + " [ " + record + " ] ");
					}
				}

				if (errorType == MessageTypeEnum.WarningParser)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.WarningParser, //bitAndRecord.ErrorType.ToString() + " : "  +
							 ProductValidate.ConvertDataErrorCode2WarningMessage(b) + " [ " + record + " ] ");
					}
				}
			}
		}

	


	}
}
