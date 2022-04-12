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
	public class ImportLocationSQLiteADORepository : BaseImportADORepository, IImportLocationSQLiteADORepository
	{
		private ILocationSQLiteParser _locationParser;
  	//	private readonly ILocationRepository _locationRepository;
 		//private Dictionary<string, Location> _locationDictionary;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportLocationSQLiteADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log)
			: base(connection, dbSettings,  log, serviceLocator)
        {
	    }


		public void InsertLocations(string fromPathFile, string toPathDB3,
			LocationSQLiteParserEnum locationParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._locationParser = this._serviceLocator.GetInstance<ILocationSQLiteParser>(locationParserEnum.ToString());

			if (this._locationParser == null)
			{
				//Localization.Resources.Log_Error1009%"In  LocationParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1009, locationParserEnum));
				return;
			}

			//if (File.Exists(fromPathFile) == false)
			//{
			//	this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
			//	return;
			//}

			toPathDB3 = this.ConnectionADO.CopyEmptyMobileDB(toPathDB3);	   //в ту же папку откуда берем файл источник
			Dictionary<string, string> locationFromDBDictionary = new Dictionary<string, string>();

			if (importType.Contains(ImportDomainEnum.ExistCode) == true)
			{
				locationFromDBDictionary = this.GetLocationCodeDictionary(encoding, toPathDB3);//new Dictionary<string, string>();
			}


			//string objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.InventorCode);
			//if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.BranchCode);
			//if (string.IsNullOrWhiteSpace(objectCode) == true) objectCode = parms.GetStringValueFromParm(ImportProviderParmEnum.CustomerCode);

			//string pathBD3 = Path.GetDirectoryName(fromPathFile) + @"\" + objectCode + ".db3";

			this.Log.Add(MessageTypeEnum.TraceRepository, "ImportLocation Repository is [ImportLocationSQLiteADORepository]");
			//this.Log.Add("InventIturParser is [ " + inventProductSimpleParserEnum.ToString() + "]");
	 //		 INSERT INTO [Location]
	 //	  ([Uid]
	 //	  ,[LocationCode]
	 //	  ,[Description]
	 //	  ,[Level1Code]
	 //	  ,[Level1Name]
	 //	  ,[Level2Code]
	 //	  ,[Level2Name]
	 //	  ,[Level3Code]
	 //	  ,[Level3Name]
	 //	  ,[Level4Code]
	 //	  ,[Level4Name]
	 //	  ,[InvStatus]
	 //	  ,[NodeType]
	 //	  ,[LevelNum]
	 //	  ,[Total]
	 //	  ,[DateModified]
			//)
	 //VALUES
	 //	  (<Uid, text,>
	 //	  ,<LocationCode, text,>
	 //	  ,<Description, text,>
	 //	  ,<Level1Code, text,>
	 //	  ,<Level1Name, text,>
	 //	  ,<Level2Code, text,>
	 //	  ,<Level2Name, text,>
	 //	  ,<Level3Code, text,>
	 //	  ,<Level3Name, text,>
	 //	  ,<Level4Code, text,>
	 //	  ,<Level4Name, text,>
	 //	  ,<InvStatus, text,>
	 //	  ,<NodeType, text,>
	 //	  ,<LevelNum, text,>
	 //	  ,<Total, text,>
	 //	  ,<DateModified, text,>);

			string sql1 = "INSERT INTO [Location](" +
			"[Uid]" +
		   ",[LocationCode]" +
		   ",[Description]" +
		   ",[Level1Code]" +
			",[Level1Name]" +					
			",[Level2Code]" +					
			",[Level2Name]" +					
			",[Level3Code]" +					
			",[Level3Name]" +					
			",[Level4Code]" +					
			",[Level4Name]" +					
			",[InvStatus]" +					
			",[NodeType]" +					
			",[LevelNum]" +					
			",[Total]" +					
			",[DateModified]" +
			",[IturCode]" +		
			")" + 
      " VALUES(" +
			":Uid" +
			",:LocationCode" +
		   ",:Description" +
		   ",:Level1Code" +
		   ",:Level1Name" +
			",:Level2Code" +
			",:Level2Name" +
			",:Level3Code" +
			",:Level3Name" +
			",:Level4Code" +
			",:Level4Name" +
			",:InvStatus" +
			",:NodeType" +
			",:LevelNum" +
			",:Total" +
			",:DateModified" +
			",:IturCode" +
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
						"LocationCode", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Description", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level1Code", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level1Name", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level2Code", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level2Name", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level3Code", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level3Name", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level4Code", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Level4Name", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"InvStatus", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"NodeType", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"LevelNum", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"Total", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"DateModified", SQLiteType.Text));
						cmd.Parameters.Add(new SQLiteParameter(
						"IturCode", SQLiteType.Text));
						
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Location", toPathDB3));

						foreach (KeyValuePair<string, LocationMobile> keyValuePair in
							this._locationParser.GetLocationMobiles(fromPathFile, encoding, separators,
							countExcludeFirstString,
							locationFromDBDictionary, parms))
						{
							k++;
							string key = keyValuePair.Key;
							LocationMobile val = keyValuePair.Value;
							cmd.Parameters["Uid"].Value = val.Uid;
							cmd.Parameters["LocationCode"].Value = val.LocationCode;
							cmd.Parameters["Description"].Value = val.Description;
							cmd.Parameters["Level1Code"].Value = val.Level1Code;
							cmd.Parameters["Level1Name"].Value = val.Level1Name;
							cmd.Parameters["Level2Code"].Value = val.Level2Code;
							cmd.Parameters["Level2Name"].Value = val.Level2Name;
							cmd.Parameters["Level3Code"].Value = val.Level3Code;
							cmd.Parameters["Level3Name"].Value = val.Level3Name;
							cmd.Parameters["Level4Code"].Value = val.Level4Code;
							cmd.Parameters["Level4Name"].Value = val.Level4Name;
							cmd.Parameters["InvStatus"].Value = val.InvStatus;
							cmd.Parameters["NodeType"].Value = val.NodeType;
							cmd.Parameters["LevelNum"].Value = val.LevelNum;
							cmd.Parameters["Total"].Value = val.Total;
							cmd.Parameters["DateModified"].Value = val.DateModified;
							cmd.Parameters["IturCode"].Value = val.IturCode;
							cmd.ExecuteNonQuery();
						}
					}
					//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportLocationSQLiteADORepository"));
					tran.Commit();
					//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
					this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "Location", toPathDB3));
					//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}]"
					this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportLocationSQLiteADORepository"));
				}

				catch (Exception error)
				{
					_logger.ErrorException("InsertLocationMobile", error);
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
			this.FillLogFromErrorBitList(this._locationParser.ErrorBitList);
			//LogPrint();
		}



		public void ClearLocations(string pathDB)
		{
			if (File.Exists(pathDB) == false) return;
			string sql1 = "DELETE FROM  [Location]";
			SQLiteTransaction tran = null;

			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB + @"; Pooling=false;"; 
				try
				{
					sqliteConnection.Open();
					tran = sqliteConnection.BeginTransaction();
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{
						//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Location", pathDB));
						//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Location"));
						cmd.ExecuteNonQuery();
					}
					//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Location"));
					tran.Commit();
				}
				catch (Exception error)
				{
					_logger.ErrorException("ClearLocations", error);
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

		public void VacuumLocation(string pathDB3)
		{
			if (File.Exists(pathDB3) == false) return;
			string sql1 = "VACUUM  [Location]";
			SQLiteTransaction tran = null;

			using (SQLiteConnection sqliteConnection = new SQLiteConnection())
			{
				sqliteConnection.ConnectionString = @"Data Source=" + pathDB3 + @";Read Uncommitted=true; Pooling=false;"; 
				try
				{
					sqliteConnection.Open();
					using (SQLiteCommand cmd = new SQLiteCommand(sql1, sqliteConnection, tran))
					{
						this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("VACUUM Location in [{0}] ", pathDB3));
						cmd.ExecuteNonQuery();
					}
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format("VACUUM  [{0}] ", "Location"));
				}
				catch (Exception error)
				{
					_logger.ErrorException("VacuumLocation", error);
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

		public Dictionary<string, string> GetLocationCodeDictionary(Encoding encoding, string pathDB)
		{
			Dictionary<string, string> dictionaryLocationCode = new Dictionary<string, string>();
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
			string tableName = "Location";
			string[] separators = new string[] { "" };
			string[] columnNames = new string[] { "Uid", "LocationCode", "Description" };
			Dictionary<string, int> dictionaryColumnNumbers = new Dictionary<string, int>();
			int columCount = columnNames.Length;
			for (int i = 0; i < columCount; i++)
			{
				string columnName = columnNames[i];
				dictionaryColumnNumbers[columnName] = i;
			}

			int rowCount = 0;
			int indexLocationCode = -1;
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
					indexLocationCode = "LocationCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexDescription = "Description".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
				}

				string locationCode = record[indexLocationCode].Trim();
				string description = record[indexDescription].Trim();
				dictionaryLocationCode[locationCode] = description;
			}
			return dictionaryLocationCode;
		}


		
		public LocationMobile GetLocationMobileByLocationCode(string locationCode, string pathDB)
		{
			LocationMobile locationMobile = null;
			using (SQLiteConnection con = new SQLiteConnection())
			{
				con.ConnectionString = @"Data Source=" + pathDB + @";Read Uncommitted=true; Pooling=false;";
				con.Open();
				string sql1 = String.Format(@"SELECT 
							Uid
							,LocationCode				 
							,Description
							,Level1Code
							,Level1Name
							,Level2Code
							,Level2Name
							,Level3Code
							,Level3Name
							,Level4Code
							,Level4Name
							,InvStatus
							,NodeType
							,LevelNum
							,Total
							,DateModified 
							FROM Location
							WHERE LocationCode = :LocationCode"); 
				using (SQLiteCommand command = new SQLiteCommand(sql1, con))
				{
					command.Parameters.Add(new SQLiteParameter(
					"LocationCode", SQLiteType.Text));
					Devart.Data.SQLite.SQLiteDataReader dataReader = null;

					try
					{
						command.Parameters["LocationCode"].Value = locationCode;
						dataReader = command.ExecuteReader();
						int getLocation = 0;
						while (dataReader.Read() && getLocation == 0)
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
							String[] aRecord = records.ToArray();					 //17
							try
							{
								locationMobile = new LocationMobile();
								locationMobile.Uid = aRecord[0] != null ? aRecord[0] : "";
								locationMobile.LocationCode = aRecord[1] != null ? aRecord[1] : "";
								locationMobile.Description = aRecord[2] != null ? aRecord[2] : "";
								locationMobile.Level1Code = aRecord[3] != null ? aRecord[3] : "";
								locationMobile.Level1Name = aRecord[4] != null ? aRecord[4] : "";
								locationMobile.Level2Code = aRecord[5] != null ? aRecord[5] : "";
								locationMobile.Level2Name = aRecord[6] != null ? aRecord[6] : "";
								locationMobile.Level3Code = aRecord[7] != null ? aRecord[7] : "";
								locationMobile.Level3Name = aRecord[8] != null ? aRecord[8] : "";
								locationMobile.Level4Code = aRecord[9] != null ? aRecord[9] : "";
								locationMobile.Level4Name = aRecord[10] != null ? aRecord[10] : "";
								locationMobile.InvStatus = aRecord[11] != null ? aRecord[11] : "";
								locationMobile.NodeType = aRecord[12] != null ? aRecord[12] : "";
								locationMobile.LevelNum = aRecord[13] != null ? aRecord[13] : "";
								locationMobile.Total = aRecord[14] != null ? aRecord[14] : "";
								locationMobile.DateModified = aRecord[15] != null ? aRecord[15] : "";
								//locationMobile.IturCode = aRecord[16] != null ? aRecord[16] : "";
								getLocation ++;
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
			return locationMobile;
		}

	


		public Dictionary<string, LocationMobile> GetLocationMobileDictionary(Encoding encoding, string pathDB)
		{
			Dictionary<string, LocationMobile> dictionaryLocation = new Dictionary<string, LocationMobile>();
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
			string tableName = "Location";
			string[] separators = new string[] { "" };
			string[] columnNames = new string[] { "Uid", "LocationCode", "Description", "Level1Code", "Level1Name", "Level2Code", "Level2Name" ,
			"Level3Code", "Level3Name", "Level4Code",  "Level4Name", "InvStatus", "NodeType", "LevelNum", "Total", "DateModified", "IturCode"};
			Dictionary<string, int> dictionaryColumnNumbers = new Dictionary<string, int>();
			int columCount = columnNames.Length;
			for (int i = 0; i < columCount; i++)
			{
				string columnName = columnNames[i];
				dictionaryColumnNumbers[columnName] = i;
			}

			int rowCount = 0;
			int indexLocationCode = -1;
			int indexDescription = -1;
			int indexLevel1Code = -1;
			int indexLevel1Name = -1;
			int indexLevel2Code = -1;
			int indexLevel2Name = -1;
			int indexLevel3Code = -1;
			int indexLevel3Name = -1;
			int indexLevel4Code = -1;
			int indexLevel4Name = -1;
			int indexInvStatus = -1;
			int indexNodeType = -1;
			int indexLevelNum = -1;
			int indexTotal = -1;
			int indexDateModified = -1;
			int indexIturCode = -1;


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
					int recordCount = record.Count();
					indexLocationCode = "LocationCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexDescription = "Description".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel1Code = "Level1Code".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel1Name = "Level1Name".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel2Code = "Level2Code".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel2Name = "Level2Name".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel3Code = "Level3Code".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel3Name = "Level3Name".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel4Code = "Level4Code".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel4Name = "Level4Name".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexInvStatus = "InvStatus".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexNodeType = "NodeType".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevelNum = "LevelNum".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexTotal = "Total".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexDateModified = "DateModified".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexIturCode = "IturCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					if (recordCount <= indexIturCode) indexIturCode = -1;
				}

				string locationCode = record[indexLocationCode].Trim();
				LocationMobile newLocationMobile = new LocationMobile();
				newLocationMobile.LocationCode = record[indexLocationCode].Trim();
				if (indexIturCode  != - 1) newLocationMobile.IturCode = record[indexIturCode].Trim();
				newLocationMobile.Description = record[indexDescription].Trim();
				newLocationMobile.Level1Code = record[indexLevel1Code].Trim();
				newLocationMobile.Level1Name = record[indexLevel1Name].Trim();
				newLocationMobile.Level2Code = record[indexLevel2Code].Trim();
				newLocationMobile.Level2Name = record[indexLevel2Name].Trim();
				newLocationMobile.Level3Code = record[indexLevel3Code].Trim();
				newLocationMobile.Level3Name = record[indexLevel3Name].Trim();
				newLocationMobile.Level4Code = record[indexLevel4Code].Trim();
				newLocationMobile.Level4Name = record[indexLevel4Name].Trim();
				newLocationMobile.InvStatus = record[indexInvStatus].Trim();
				newLocationMobile.NodeType = record[indexNodeType].Trim();
				newLocationMobile.LevelNum = record[indexLevelNum].Trim();
				newLocationMobile.Total = record[indexTotal].Trim();
				newLocationMobile.DateModified = record[indexDateModified].Trim();

				dictionaryLocation[locationCode] = newLocationMobile;
			}
			return dictionaryLocation;
		}

		public Dictionary<string, LocationMobile> GetLocationMobileDictionaryWithStatus1And2(Encoding encoding, string pathDB)
		{
			Dictionary<string, LocationMobile> dictionaryLocation = new Dictionary<string, LocationMobile>();
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
			string tableName = "Location";
			string[] separators = new string[] { "" };
			string[] columnNames = new string[] { "Uid", "LocationCode", "Description", "Level1Code", "Level1Name", "Level2Code", "Level2Name" ,
			"Level3Code", "Level3Name", "Level4Code",  "Level4Name", "InvStatus", "NodeType", "LevelNum", "Total", "DateModified", "IturCode"};
			Dictionary<string, int> dictionaryColumnNumbers = new Dictionary<string, int>();
			int columCount = columnNames.Length;
			for (int i = 0; i < columCount; i++)
			{
				string columnName = columnNames[i];
				dictionaryColumnNumbers[columnName] = i;
			}

			int rowCount = 0;
			int indexLocationCode = -1;
			int indexDescription = -1;
			int indexLevel1Code = -1;
			int indexLevel1Name = -1;
			int indexLevel2Code = -1;
			int indexLevel2Name = -1;
			int indexLevel3Code = -1;
			int indexLevel3Name = -1;
			int indexLevel4Code = -1;
			int indexLevel4Name = -1;
			int indexInvStatus = -1;
			int indexNodeType = -1;
			int indexLevelNum = -1;
			int indexTotal = -1;
			int indexDateModified = -1;
			int indexIturCode = -1;

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
					int recordCount = record.Count();
					indexLocationCode = "LocationCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexDescription = "Description".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel1Code = "Level1Code".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel1Name = "Level1Name".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel2Code = "Level2Code".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel2Name = "Level2Name".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel3Code = "Level3Code".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel3Name = "Level3Name".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel4Code = "Level4Code".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevel4Name = "Level4Name".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexInvStatus = "InvStatus".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexNodeType = "NodeType".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexLevelNum = "LevelNum".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexTotal = "Total".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexDateModified = "DateModified".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					indexIturCode = "IturCode".GetIndexColumnByName(dictionaryColumnNumbers, tableName, Log);
					if (recordCount <= indexIturCode) indexIturCode = -1;
				}

				string invStatus = record[indexInvStatus].Trim();
				if (invStatus == "2" || invStatus == "1")
				{
					string locationCode = record[indexLocationCode].Trim();
					LocationMobile newLocationMobile = new LocationMobile();
					newLocationMobile.LocationCode = record[indexLocationCode].Trim();
					if (indexIturCode != -1) newLocationMobile.IturCode = record[indexIturCode].Trim();
					newLocationMobile.Description = record[indexDescription].Trim();
					newLocationMobile.Level1Code = record[indexLevel1Code].Trim();
					newLocationMobile.Level1Name = record[indexLevel1Name].Trim();
					newLocationMobile.Level2Code = record[indexLevel2Code].Trim();
					newLocationMobile.Level2Name = record[indexLevel2Name].Trim();
					newLocationMobile.Level3Code = record[indexLevel3Code].Trim();
					newLocationMobile.Level3Name = record[indexLevel3Name].Trim();
					newLocationMobile.Level4Code = record[indexLevel4Code].Trim();
					newLocationMobile.Level4Name = record[indexLevel4Name].Trim();
					newLocationMobile.InvStatus = record[indexInvStatus].Trim();
					newLocationMobile.NodeType = record[indexNodeType].Trim();
					newLocationMobile.LevelNum = record[indexLevelNum].Trim();
					newLocationMobile.Total = record[indexTotal].Trim();
					newLocationMobile.DateModified = record[indexDateModified].Trim();

					dictionaryLocation[locationCode] = newLocationMobile;
				}
			}
			return dictionaryLocation;
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
