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
	public class ImportUnitPlanADORepository :  BaseImportADORepository, IImportLocationRepository
	{
		private ILocationParser _locationParser;
  		private readonly ILocationRepository _locationRepository;
 		private Dictionary<string, Location> _locationDictionary;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportUnitPlanADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			ILocationRepository locationRepository)
			: base(connection,dbSettings, log, serviceLocator)
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
				this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
				return;
			}

			Dictionary<string, Location> locationFromDBDictionary = this.GetLocationDictionary(pathDB);

			this.Log.Add(MessageTypeEnum.TraceRepository, "ImportLocation Repository is [ImportLocationADORepository]");
			//this.Log.Add("InventIturParser is [ " + inventProductSimpleParserEnum.ToString() + "]");

			string sql1 = "INSERT INTO [Location](" +
		   "[Name]" + 
		   ",[Description]" + 
		   ",[BackgroundColor]" + 
           ",[Code]" + 
			")" + 
      " VALUES(" +
			"@Name" +
		   ",@Description" +
		   ",@BackgroundColor" + 
           ",@Code" + 
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
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Location", pathDB));
				int k = 0;
				foreach (KeyValuePair<string, Location> keyValuePair in
					this._locationParser.GetLocations(fromPathFile, encoding, separators, 
					countExcludeFirstString, 				
					locationFromDBDictionary))
				{
					k++;
					string key = keyValuePair.Key;	
					Location val = keyValuePair.Value; 
					cmd.Parameters["@Code"].Value = val.Code;
					cmd.Parameters["@Name"].Value = val.Name;
					cmd.Parameters["@Description"].Value = val.Description;
					cmd.Parameters["@BackgroundColor"].Value = val.BackgroundColor;
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
