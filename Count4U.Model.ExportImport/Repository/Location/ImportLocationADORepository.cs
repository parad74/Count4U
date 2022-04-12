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
using System.IO;

namespace Count4U.Model.Count4U
{
	public class ImportLocationADORepository :  BaseImportADORepository, IImportLocationRepository
	{
		private ILocationParser _locationParser;
  		private readonly ILocationRepository _locationRepository;
 		private Dictionary<string, Location> _locationDictionary;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportLocationADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			ILocationRepository locationRepository)
			: base(connection, dbSettings, log, serviceLocator)
        {
			//if (locationParser == null) throw new ArgumentNullException("locationParser");
			//if (serviceLocator == null) throw new ArgumentNullException("serviceLocator");
			if (locationRepository == null) throw new ArgumentNullException("locationRepository");

			this._locationRepository = locationRepository;
			this._locationDictionary = new Dictionary<string, Location>();
	    }

		private Dictionary<string, Location> GetLocationDictionary(string pathDB)
		{
			//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "LocationDictionary"));
			this._locationRepository.ClearLocationDictionary();
			//Localization.Resources.Log_TraceRepository1045%"Clear [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1045, "LocationDictionary"));
			Dictionary<string, Location> locationFromDBDictionary =
				this._locationRepository.GetLocationDictionary(pathDB, true);
			//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill  [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "LocationDictionary"));
			return locationFromDBDictionary;
		}

		public void InsertLocations(string fromPathFile, string pathDB, LocationParserEnum locationParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._locationParser = this._serviceLocator.GetInstance<ILocationParser>(locationParserEnum.ToString());

			if (this._locationParser == null)
			{
				//Localization.Resources.Log_Error1009%"In  LocationParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1009, locationParserEnum));
				return;
			}

			if (File.Exists(fromPathFile) == false)
			{
				if (fromPathFile != pathDB)
				{
					this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
					return;
				}
			}

			Dictionary<string, Location> locationFromDBDictionary = this.GetLocationDictionary(pathDB);

			this.Log.Add(MessageTypeEnum.TraceRepository, "ImportLocation Repository is [ImportLocationADORepository]");
			//this.Log.Add("InventIturParser is [ " + inventProductSimpleParserEnum.ToString() + "]");

			string sql1 = "INSERT INTO [Location](" +
		   "[Name]" + 
		   ",[Description]" + 
		   ",[BackgroundColor]" + 
           ",[Code]" +
		   ",[Restore]" +
			",[ParentLocationCode]" + 
			",[TypeCode]" + 
			",[Level1]" + 
			",[Level2]" + 
			",[Level3]" + 
			",[Level4]" + 
			",[Name1]" + 
			",[Name2]" + 
			",[Name3]" + 
			",[Name4]" + 
			",[NodeType]" + 
			",[LevelNum]" + 
			",[Total]" +
			",[Tag]" + 
			",[InvStatus]" +
			",[Disabled]" + 
			",[DateModified]" + 
			")" + 
      " VALUES(" +
			"@Name" +
		   ",@Description" +
		   ",@BackgroundColor" + 
           ",@Code" +
		   ",@Restore" +
			",@ParentLocationCode" +
			",@TypeCode" +
			",@Level1" +
			",@Level2" +
			",@Level3" +
			",@Level4" +
			",@Name1" +
			",@Name2" +
			",@Name3" +
			",@Name4" +
			",@NodeType" +
			",@LevelNum" +
			",@Total" +
			",@Tag" +
			",@InvStatus" +
			",@Disabled" + 
			",@DateModified" + 
			")";
	

			SqlCeTransaction tran = null;

			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

				cmd.Parameters.Add(new SqlCeParameter(
					"@Name", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Description", SqlDbType.NVarChar, 500));
				cmd.Parameters.Add(new SqlCeParameter(
					"@BackgroundColor", SqlDbType.NVarChar, 50));			
				cmd.Parameters.Add(new SqlCeParameter(
					"@Code", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Restore", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ParentLocationCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@TypeCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Level1", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Level2", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Level3", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Level4", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Name1", SqlDbType.NVarChar, 250));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Name2", SqlDbType.NVarChar, 250));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Name3", SqlDbType.NVarChar, 250));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Name4", SqlDbType.NVarChar, 250));
			  	cmd.Parameters.Add(new SqlCeParameter(
					"@NodeType", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@LevelNum", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Total", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
				"@Tag", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
				"@InvStatus", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Disabled", SqlDbType.Bit));
				cmd.Parameters.Add(new SqlCeParameter(
				 "@DateModified", SqlDbType.DateTime));

				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Location", pathDB));
				int k = 0;
				foreach (KeyValuePair<string, Location> keyValuePair in
					this._locationParser.GetLocations(fromPathFile, encoding, separators, 
					countExcludeFirstString, 				
					locationFromDBDictionary, parms))
				{
					k++;
					string key = keyValuePair.Key;	
					Location val = keyValuePair.Value; 
					cmd.Parameters["@Code"].Value = val.Code;
					cmd.Parameters["@Restore"].Value = val.Restore != null ? val.Restore : ""; 
					cmd.Parameters["@Name"].Value = val.Name;
					cmd.Parameters["@Description"].Value = val.Description;
					cmd.Parameters["@BackgroundColor"].Value = val.BackgroundColor;
					cmd.Parameters["@ParentLocationCode"].Value = val.ParentLocationCode;
					cmd.Parameters["@TypeCode"].Value = val.TypeCode;
					cmd.Parameters["@Level1"].Value = val.Level1;
					cmd.Parameters["@Level2"].Value = val.Level2;
					cmd.Parameters["@Level3"].Value = val.Level3;
					cmd.Parameters["@Level4"].Value = val.Level4;
					cmd.Parameters["@Name1"].Value = val.Name1;
					cmd.Parameters["@Name2"].Value = val.Name2;
					cmd.Parameters["@Name3"].Value = val.Name3;
					cmd.Parameters["@Name4"].Value = val.Name4;
					cmd.Parameters["@NodeType"].Value = val.NodeType;
					cmd.Parameters["@LevelNum"].Value = val.LevelNum;
					cmd.Parameters["@Total"].Value = val.Total;
					cmd.Parameters["@Tag"].Value = val.Tag;
					cmd.Parameters["@InvStatus"].Value = val.InvStatus != null ? val.InvStatus : 0;
					cmd.Parameters["@Disabled"].Value = val.Disabled != null ? val.Disabled : false;
					cmd.Parameters["@DateModified"].Value = val.DateModified;


					cmd.ExecuteNonQuery();
				}
				//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportLocationADORepository"));
				tran.Commit();
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "Location", pathDB));
				//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportLocationADORepository"));
			}

			catch (Exception error)
			{
				_logger.ErrorException("InsertLocations", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);

				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}

			this.Log.Add(MessageTypeEnum.TraceRepository, "");
			this.FillLogFromErrorBitList(this._locationParser.ErrorBitList);
			//LogPrint();
		}

		

		public void ClearLocations(string pathDB)
		{
			//this.Log.Clear();
			string sql1 = "DELETE FROM  [Location]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Location", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start Process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Location"));
				//LogPrint();
				cmd.ExecuteNonQuery();
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
