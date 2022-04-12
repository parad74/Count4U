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

namespace Count4U.Model.Count4U
{
	public class ImportCatalogSQLiteADORepository : BaseImportADORepository, IImportCatalogSQLiteADORepository
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private ICatalogSQLiteParser _productParser;

		public ImportCatalogSQLiteADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log)
			: base(connection, dbSettings, log, serviceLocator)
        {
	
	    }


	public	void InsertCatalogs(string fromPathFile,   //GetDbPath		Count4U
			string toPathDB3,									//db3Path		  sqlite
			CatalogSQLiteParserEnum productParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null )
		{
			this._productParser = this._serviceLocator.GetInstance<ICatalogSQLiteParser>(productParserEnum.ToString());

			if (this._productParser == null)
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

			//string objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.InventorCode);
			//if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.BranchCode);
			//if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.CustomerCode);

			//string pathBD3 = Path.GetDirectoryName(fromPathFile) + @"\" + objectCode + ".db3";
			

			//Localization.Resources.Log_TraceRepository1040%"[{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "ImportCatalogRepository", "ImportCatalogSQLiteADORepository"));
			//Localization.Resources.Log_TraceRepository1040%"[{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "CatalogSQLiteParser", productParserEnum.ToString()));
	 //			 INSERT INTO [Catalog]
	 //	  ([Uid]
	 //	  ,[ItemCode]
	 //	  ,[ItemName]
	 //	  ,[ItemType]
	 //	  ,[FamilyCode]
	 //	  ,[FamilyName]
	 //	  ,[SectionCode]
	 //	  ,[SectionName]
	 //	  ,[SubSectionCode]
	 //	  ,[SubSectionName]
	 //	  ,[PriceBuy]
	 //	  ,[PriceSell]
	 //	  ,[SupplierCode]
	 //	  ,[SupplierName]
	 //	  ,[UnitTypeCode]
	 //	  ,[Description]
			//)
	 //VALUES
	 //	  (<Uid, text,>
	 //	  ,<ItemCode, text,>
	 //	  ,<ItemName, text,>
	 //	  ,<ItemType, text,>
	 //	  ,<FamilyCode, text,>
	 //	  ,<FamilyName, text,>
	 //	  ,<SectionCode, text,>
	 //	  ,<SectionName, text,>
	 //	  ,<SubSectionCode, text,>
	 //	  ,<SubSectionName, text,>
	 //	  ,<PriceBuy, text,>
	 //	  ,<PriceSell, text,>
	 //	  ,<SupplierCode, text,>
	 //	  ,<SupplierName, text,>
	 //	  ,<UnitTypeCode, text,>
	 //	  ,<Description, text,>);

			string sql1 = "INSERT INTO [Catalog] ("
				+ "[Uid]"
				+ ",[ItemCode]  "
				+ ",[ItemName] "
				+ ",[ItemType]	 "
				+ ",[FamilyCode] "
				+ ",[FamilyName] "
				+ ",[SectionCode] "
				+ ",[SectionName] "
				+ ",[SubSectionCode] "
				+ ",[SubSectionName] "
				+ ",[PriceBuy] "
				+ ",[PriceSell] "
				+ ",[SupplierCode] "
				+ ",[SupplierName] "
				+ ",[UnitTypeCode] "
				+ ",[Description] "
				+ ") "
				+  "VALUES ( "
				+ ":Uid"
				+ ",:ItemCode"
				+ ",:ItemName"
				+ ",:ItemType"
				+ ",:FamilyCode"
				+ ",:FamilyName"
				+ ",:SectionCode"
				+ ",:SectionName"
				+ ",:SubSectionCode"
				+ ",:SubSectionName"
				+ ",:PriceBuy"
				+ ",:PriceSell"
				+ ",:SupplierCode"
				+ ",:SupplierName"
				+ ",:UnitTypeCode"
				+ ",:Description"
				+ ") ";
			
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
						"Uid",SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"ItemCode", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
						"ItemName", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"ItemType", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"FamilyCode", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"FamilyName", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"SectionCode", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"SectionName", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"SubSectionCode", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"SubSectionName", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"PriceBuy", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"PriceSell", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"SupplierCode", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"SupplierName", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"UnitTypeCode", SQLiteType.Text));
					cmd.Parameters.Add(new SQLiteParameter(
					"Description", SQLiteType.Text));
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Catalog", toPathDB3));


					if (importType.Contains(ImportDomainEnum.ImportCatalog) == true)
					{
						foreach (Catalog catalogItem in this._productParser.GetCatalogs(fromPathFile,  //GetDbPath		Count4U
							encoding, separators, countExcludeFirstString, catalogMakatDictionary, importType, parms))
						{

							if (cancellationToken.IsCancellationRequested == true)
							{
								break;
							}
							k++;
							cmd.Parameters["Uid"].Value = catalogItem.Uid;
							cmd.Parameters["ItemCode"].Value = catalogItem.ItemCode;
							cmd.Parameters["ItemName"].Value = catalogItem.ItemName;
							cmd.Parameters["ItemType"].Value = catalogItem.ItemType;
							cmd.Parameters["FamilyCode"].Value = catalogItem.FamilyCode;
							cmd.Parameters["FamilyName"].Value = catalogItem.FamilyName;
							cmd.Parameters["SectionCode"].Value = catalogItem.SectionCode;
							cmd.Parameters["SectionName"].Value = catalogItem.SectionName;
							cmd.Parameters["SubSectionCode"].Value = catalogItem.SubSectionCode;
							cmd.Parameters["SubSectionName"].Value = catalogItem.SubSectionName;
							cmd.Parameters["PriceBuy"].Value = catalogItem.PriceBuy;
							cmd.Parameters["PriceSell"].Value = catalogItem.PriceSell;
							cmd.Parameters["SupplierCode"].Value = catalogItem.SupplierCode;
							cmd.Parameters["SupplierName"].Value = catalogItem.SupplierName;
							cmd.Parameters["UnitTypeCode"].Value = catalogItem.UnitTypeCode;
							cmd.Parameters["Description"].Value = catalogItem.Description;

							cmd.ExecuteNonQuery();
						}
					}
					countAction(k);
				}
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "Catalog", toPathDB3));


					//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportCatalogSQLiteADORepository"));
					tran.Commit();
					//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}] "
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportCatalogSQLiteADORepository"));
				}
				catch (Exception error)
				{
					_logger.ErrorException("InserCatalogs", error);
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
				this.Log.Add(MessageTypeEnum.TraceRepository, "");
				this.FillLogFromErrorBitList(this._productParser.ErrorBitList);
			}
	
		}

		public Dictionary<string, Catalog> GetCatalogMobileDictionary(Encoding encoding, string pathDB)
		{

			Dictionary<string, Catalog> dictionaryCatalog = new Dictionary<string, Catalog>();
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
			string tableName = "Catalog";
			string[] separators = new string[] { "" };
					
			string[] columnNames = new string[] { "Uid", "ItemCode", "ItemName", "ItemType", "FamilyCode", "FamilyName", "SectionCode", "SectionName", 
			"SubSectionCode", "SubSectionName", "PriceBuy", "PriceSell", "SupplierCode", "SupplierName", "UnitTypeCode", "Description"};
			Dictionary<string, int> dictionaryColumnNumbers = new Dictionary<string, int>();
			int columCount = columnNames.Length;
			for (int i = 0; i < columCount; i++)
			{
				string columnName = columnNames[i];
				dictionaryColumnNumbers[columnName] = i;
			}
			int rowCount = 0;
		 	int indexUid = -1;
			int indexItemCode  = -1;
			int indexItemName  = -1;
			int indexItemType  = -1;
			int indexFamilyCode	 = -1;
			int indexFamilyName= -1;
			int indexSectionCode  = -1;
			int indexSectionName   = -1;
			int indexSubSectionCode	 = -1;
			int indexSubSectionName	 = -1;
			int indexPriceBuy = -1;
			int indexPriceSell = -1;
			int indexSupplierCode  = -1;
			int indexSupplierName	= -1;
			int indexUnitTypeCode	= -1;
			int indexDescription = -1;
			
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
					indexItemCode = "ItemCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexItemName = "ItemName".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexItemType = "ItemType".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexFamilyCode = "FamilyCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexFamilyName = "FamilyName".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexSectionCode = "SectionCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexSectionName = "SectionName".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexSubSectionCode = "SubSectionCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexSubSectionName = "SubSectionName".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexPriceBuy = "PriceBuy".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexPriceSell = "PriceSell".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexSupplierCode = "SupplierCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexSupplierName = "SupplierName".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexUnitTypeCode = "UnitTypeCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexDescription = "Description".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
				}

				string itemCode = record[indexItemCode].Trim();
				Catalog catalogMobile = new Catalog();
				catalogMobile.ItemCode = record[indexItemCode].Trim();
				catalogMobile.Uid = record[indexUid].Trim();
				catalogMobile.ItemName = record[indexItemName].Trim();
				catalogMobile.ItemType = record[indexItemType].Trim();
				catalogMobile.FamilyCode = record[indexFamilyCode].Trim();
				catalogMobile.FamilyName = record[indexFamilyName].Trim();
				catalogMobile.SectionCode = record[indexSectionCode].Trim();
				catalogMobile.SectionName = record[indexSectionName].Trim();
				catalogMobile.SubSectionCode = record[indexSubSectionCode].Trim();
				catalogMobile.SubSectionName = record[indexSubSectionName].Trim();
				catalogMobile.PriceBuy = record[indexPriceBuy].Trim();
				catalogMobile.PriceSell = record[indexPriceSell].Trim();
				catalogMobile.SupplierCode = record[indexSupplierCode].Trim();
				catalogMobile.SupplierName = record[indexSupplierName].Trim();
				catalogMobile.UnitTypeCode = record[indexUnitTypeCode].Trim();
				catalogMobile.Description = record[indexDescription].Trim();

				dictionaryCatalog[itemCode] = catalogMobile;
			}
			return dictionaryCatalog;
		}
		

	public Catalog GetCatalogMobileByItemCode(string itemCode, string pathDB)
	{
		Catalog catalogMobile = null;
		using (SQLiteConnection con = new SQLiteConnection())
		{
			con.ConnectionString = @"Data Source=" + pathDB + @";Read Uncommitted=true; Pooling=false;";
			con.Open();
			string sql1 = String.Format(@"SELECT 
			Uid
			,ItemCode
			,ItemName
			,ItemType
			,FamilyCode
			,FamilyName
			,SectionCode
			,SectionName
			,SubSectionCode
			,SubSectionName
			,PriceBuy
			,PriceSell
			,SupplierCode
			,SupplierName
			,UnitTypeCode
			,Description
			 FROM Catalog
			WHERE ItemCode = :ItemCode");
			using (SQLiteCommand command = new SQLiteCommand(sql1, con))
			{
				command.Parameters.Add(new SQLiteParameter(
				"ItemCode", SQLiteType.Text));
				Devart.Data.SQLite.SQLiteDataReader dataReader = null;

				try
				{
					command.Parameters["ItemCode"].Value = itemCode;
					dataReader = command.ExecuteReader();
					int getCatalog = 0;
					while (dataReader.Read() && getCatalog == 0)
					{
						int countColumn = dataReader.FieldCount;
						List<string> records = new List<string>();

						for (int i = 0; i < countColumn; i++)
						{
							if (dataReader.IsDBNull(i) == false)
							{
								records.Add(dataReader.GetValue(i).ToString());
							}
							else
							{
								records.Add("");
							}
						}
						String[] aRecord = records.ToArray();
						try
						{
							catalogMobile = new Catalog();
							catalogMobile.Uid = aRecord[0] != null ? aRecord[0] : "";
							catalogMobile.ItemCode = aRecord[1] != null ? aRecord[1] : "";
							catalogMobile.ItemName = aRecord[2] != null ? aRecord[2] : "";
							catalogMobile.ItemType = aRecord[3] != null ? aRecord[3] : "";
							catalogMobile.FamilyCode = aRecord[4] != null ? aRecord[4] : "";
							catalogMobile.FamilyName = aRecord[5] != null ? aRecord[5] : "";
							catalogMobile.SectionCode = aRecord[6] != null ? aRecord[6] : "";
							catalogMobile.SectionName = aRecord[7] != null ? aRecord[7] : "";
							catalogMobile.SubSectionCode = aRecord[8] != null ? aRecord[8] : "";
							catalogMobile.SubSectionName = aRecord[9] != null ? aRecord[9] : "";
							catalogMobile.PriceBuy = aRecord[10] != null ? aRecord[10] : "";
							catalogMobile.PriceSell = aRecord[11] != null ? aRecord[11] : "";
							catalogMobile.SupplierCode = aRecord[12] != null ? aRecord[12] : "";
							catalogMobile.SupplierName = aRecord[13] != null ? aRecord[13] : "";
							catalogMobile.UnitTypeCode = aRecord[14] != null ? aRecord[14] : "";
							catalogMobile.Description = aRecord[15] != null ? aRecord[15] : "";
							getCatalog++;
						}
						catch (Exception exp)
						{
							this.Log.Add(MessageTypeEnum.Error, exp.Message);
						}
					}
				}
				catch (Devart.Data.SQLite.SQLiteException exception)
				{
					this.Log.Add(MessageTypeEnum.Error, exception.Message);
					if (dataReader != null)
					{
						dataReader.Close();
						dataReader.Dispose();
						dataReader = null;
					}
				}
				if (dataReader != null)
				{
					dataReader.Close();
					dataReader.Dispose();
					dataReader = null;
				}
			}
			con.Close();
			con.Dispose();
		}
		return catalogMobile;
	}

		public void ClearCatalogs(string pathDB3)
		{
			if (File.Exists(pathDB3) == false) return;
			string sql1 = "DELETE FROM  [Catalog]";
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
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Catalog", pathDB3));
						//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Catalog"));
						cmd.ExecuteNonQuery();
					}
					//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Catalog"));
					tran.Commit();
				}
				catch (Exception error)
				{
					_logger.ErrorException("ClearCatalogs", error);
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

		public void VacuumCatalogs(string pathDB3)
		{
			if (File.Exists(pathDB3) == false) return;
			string sql1 = "VACUUM  [Catalog]";
			SQLiteTransaction tran = null;

			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB3 + @";Pooling=false;"; 
				try
				{
					sqliteConnection.Open();
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("VACUUM Catalog in [{0}] ", pathDB3));
						cmd.ExecuteNonQuery();
					}
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("VACUUM  [{0}] ", "Catalog"));
				}
				catch (Exception error)
				{
					_logger.ErrorException("VacuumCatalogs", error);
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

		//	DROP TABLE IF EXISTS fts_CatalogTable
		public void DropCatalogIndexTable(string pathDB3)
		{
			if (File.Exists(pathDB3) == false) return;
		//	string sql1 = "VACUUM  [Catalog]";
			string sql1 = "DROP TABLE IF EXISTS [fts_CatalogTable]";
			SQLiteTransaction tran = null;

			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB3 + @";Pooling=false;";
				try
				{
					sqliteConnection.Open();
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("DROP TABLE IF EXISTS [fts_CatalogTable] in [{0}] ", pathDB3));
						cmd.ExecuteNonQuery();
					}
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("DROPED TABLE IF EXISTS [fts_CatalogTable]  [{0}] ", "Catalog"));
				}
				catch (Exception error)
				{
					_logger.ErrorException("DropCatalogIndexTable", error);
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


		//CREATE VIRTUAL TABLE IF NOT EXISTS fts_CatalogTable USING fts4 (content='Catalog', ItemCode)
		//INSERT INTO fts_CatalogTable(fts_CatalogTable) VALUES('rebuild')
		public void CreateCatalogIndexTable(string pathDB3)
		{
			if (File.Exists(pathDB3) == false) return;
			//string sql1 = "CREATE VIRTUAL TABLE IF NOT EXISTS [fts_CatalogTable] USING fts4 (content='Catalog', ItemCode)";
			string sql1 = @"	CREATE VIRTUAL TABLE IF NOT EXISTS [fts_CatalogTable] USING fts4 (tokenize=unicode61 'tokenchars=-/ .' , content='Catalog', ItemCode)";
			string sql2 = @"INSERT INTO fts_CatalogTable(fts_CatalogTable) VALUES('rebuild')";
			SQLiteTransaction tran = null;

			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB3 + @";Pooling=false;";
				try
				{
					sqliteConnection.Open();
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("CREATE VIRTUAL TABLE [fts_CatalogTable] in [{0}] ", pathDB3));
						cmd.ExecuteNonQuery();
					}
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("CREATED VIRTUAL TABLE [fts_CatalogTable]  [{0}] ", "Catalog"));

					using (SQLiteCommand cmd = new SQLiteCommand(sql2, sqliteConnection, tran))
					{
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("INSERT INTO [fts_CatalogTable] in [{0}] ", pathDB3));
						cmd.ExecuteNonQuery();
					}
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("INSERTED INTO [fts_CatalogTable]  [{0}] ", "Catalog"));
				}
				catch (Exception error)
				{
					_logger.ErrorException("DropCatalogIndexTable", error);
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
