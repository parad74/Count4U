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
	public class ImportFamilyADORepository : BaseImportADORepository, IImportFamilyRepository
	{
		private IFamilyParser _familyParser;
		private readonly IFamilyRepository _familyRepository;
		private Dictionary<string, Family> _familyDictionary;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportFamilyADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IFamilyRepository familyRepository)
			: base(connection, dbSettings, log, serviceLocator)
        {
			//if (locationParser == null) throw new ArgumentNullException("locationParser");
			//if (serviceLocator == null) throw new ArgumentNullException("serviceLocator");
			if (familyRepository == null) throw new ArgumentNullException("FamilyRepository");

			this._familyRepository = familyRepository;
			this._familyDictionary = new Dictionary<string, Family>();
	    }

		private Dictionary<string, Family> GetFamilyDictionary(string pathDB)
		{
			//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "FamilyDictionary"));
			this._familyRepository.ClearFamilyDictionary();
			//Localization.Resources.Log_TraceRepository1045%"Clear [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1045, "FamilyDictionary"));
			Dictionary<string, Family> familyFromDBDictionary = this._familyRepository.GetFamilyDictionary(pathDB, true);
			//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill  [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "FamilyDictionary"));
			return familyFromDBDictionary;
		}

		public void InsertFamilys(string fromPathFile, string pathDB, FamilyParserEnum familyParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._familyParser = this._serviceLocator.GetInstance<IFamilyParser>(familyParserEnum.ToString());

			if (this._familyParser == null)
			{
				//Localization.Resources.Log_Error1010%"In  {0} {1} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1009, "ParserList", familyParserEnum));
				return;
			}

			if (File.Exists(fromPathFile) == false)
			{
				this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
				return;
			}

			Dictionary<string, Family> familyFromDBDictionary = this.GetFamilyDictionary(pathDB);
			//Localization.Resources.Log_TraceRepository1040%"[{0}]  is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "ImportFamily Repository", "ImportFamilyADORepository"));
			//this.Log.Add("InventIturParser is [ " + inventProductSimpleParserEnum.ToString() + "]");

			string sql1 = "INSERT INTO [Family](" +
		   "[Name]" + 
		   ",[Type]" +
		    ",[Size]" +
			",[Extra1]" +
			",[Extra2]" +
			",[Description]" +
		    ",[FamilyCode]" + 
			")" + 
      " VALUES(" +
			"@Name" +
		   ",@Type" +
		    ",@Size" +
			",@Extra1" +
			",@Extra2" +
			",@Description" +
		    ",@FamilyCode" + 
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
				"@Type", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
				"@Size", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
				"@Extra1", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
				"@Extra2", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Description", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FamilyCode", SqlDbType.NVarChar, 50));
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Family", pathDB));
				int k = 0;
				foreach (KeyValuePair<string, Family> keyValuePair in
					this._familyParser.GetFamilys(fromPathFile, encoding, separators, 
					countExcludeFirstString,
					familyFromDBDictionary,
					parms))
				{
					k++;
					string key = keyValuePair.Key;
					Family val = keyValuePair.Value;
					cmd.Parameters["@FamilyCode"].Value = val.FamilyCode;
					cmd.Parameters["@Name"].Value = val.Name;
					cmd.Parameters["@Type"].Value = val.Type;
					cmd.Parameters["@Size"].Value = val.Size;
					cmd.Parameters["@Extra1"].Value = val.Extra1;
					cmd.Parameters["@Extra2"].Value = val.Extra2;
					cmd.Parameters["@Description"].Value = val.Description;
					cmd.ExecuteNonQuery();
				}
				//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportFamilyADORepository"));
				tran.Commit();
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "Family", pathDB));
				//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportFamilyADORepository"));
			}

			catch (Exception error)
			{
				_logger.ErrorException("InsertFamilys", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);

				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}

			this.Log.Add(MessageTypeEnum.TraceRepository, "");
			this.FillLogFromErrorBitList(this._familyParser.ErrorBitList);
			//LogPrint();
		}



		public void ClearFamilys(string pathDB)
		{
			//this.Log.Clear();
			string sql1 = "DELETE FROM  [Family]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Family", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start Process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Family"));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Family"));
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearFamilys", error);
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
							FamilyValidate.ConvertDataErrorCode2ErrorMessage(b) + " [ " + record + " ] ");
					}
				}

				if (errorType == MessageTypeEnum.WarningParser)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.WarningParser, //bitAndRecord.ErrorType.ToString() + " : " +
							 FamilyValidate.ConvertDataErrorCode2WarningMessage(b) + " [ " + record + " ] ");
					}
				}
			}
		}
		
	}
}
