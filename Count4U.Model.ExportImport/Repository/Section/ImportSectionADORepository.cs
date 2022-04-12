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
	public class ImportSectionADORepository : BaseImportADORepository, IImportSectionRepository
	{
		private ISectionParser _sectionParser;
		private readonly ISectionRepository _sectionRepository;
		private Dictionary<string, Section> _sectionDictionary;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportSectionADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			ISectionRepository sectionRepository)
			: base(connection, dbSettings, log, serviceLocator)
        {
			//if (locationParser == null) throw new ArgumentNullException("locationParser");
			//if (serviceLocator == null) throw new ArgumentNullException("serviceLocator");
			if (sectionRepository == null) throw new ArgumentNullException("sectionRepository");

			this._sectionRepository = sectionRepository;
			this._sectionDictionary = new Dictionary<string, Section>();
	    }

		private Dictionary<string, Section> GetSectionDictionary(string pathDB)
		{
			//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "SectionDictionary"));
			this._sectionRepository.ClearSectionDictionary();
			//Localization.Resources.Log_TraceRepository1045%"Clear [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1045, "SectionDictionary"));
			Dictionary<string, Section> sectionFromDBDictionary =  this._sectionRepository.GetSectionDictionary(pathDB, true);
			//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill  [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "SectionDictionary"));
			return sectionFromDBDictionary;
		}

		public void InsertSections(string fromPathFile, string pathDB, SectionParserEnum sectionParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._sectionParser = this._serviceLocator.GetInstance<ISectionParser>(sectionParserEnum.ToString());

			if (this._sectionParser == null)
			{
				//Localization.Resources.Log_Error1010%"In  {0} {1} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1009, "ParserList", sectionParserEnum));
				return;
			}

			if (File.Exists(fromPathFile) == false)
			{
				this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
				return;
			}

			Dictionary<string, Section> sectionFromDBDictionary = this.GetSectionDictionary(pathDB);
			//Localization.Resources.Log_TraceRepository1040%"[{0}]  is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "ImportSection Repository", "ImportSectionADORepository"));
			//this.Log.Add("InventIturParser is [ " + inventProductSimpleParserEnum.ToString() + "]");
		
			string sql1 = "INSERT INTO [Section](" +
		   "[Name]" + 
		   ",[Description]" +
		   ",[SectionCode]" + 
		   ",[ParentSectionCode]" + 
		   ",[Tag]" + 
		   ",[TypeCode]" + 
			")" + 
      " VALUES(" +
			"@Name" +
		   ",@Description" +
		   ",@SectionCode" +
		   ",@ParentSectionCode" +
		   ",@Tag" +
		   ",@TypeCode" + 
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
					"@SectionCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
				"@ParentSectionCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
				"@Tag", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
				"@TypeCode", SqlDbType.NVarChar, 50));
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Section", pathDB));
				int k = 0;
				foreach (KeyValuePair<string, Section> keyValuePair in
					this._sectionParser.GetSections(fromPathFile, encoding, separators, 
					countExcludeFirstString,
					sectionFromDBDictionary, parms))
				{
					k++;
					string key = keyValuePair.Key;
					Section val = keyValuePair.Value;
					cmd.Parameters["@SectionCode"].Value = val.SectionCode;
					cmd.Parameters["@Name"].Value = val.Name;
					cmd.Parameters["@Description"].Value = val.Description;
					cmd.Parameters["@ParentSectionCode"].Value = val.ParentSectionCode;
					cmd.Parameters["@Tag"].Value = val.Tag;
					cmd.Parameters["@TypeCode"].Value = string.IsNullOrWhiteSpace(val.TypeCode) == false ? val.TypeCode : TypeSectionEnum.S.ToString();
					cmd.ExecuteNonQuery();
				}
				//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportSectionADORepository"));
				tran.Commit();
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "Section", pathDB));
				//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportSectionADORepository"));
			}

			catch (Exception error)
			{
				_logger.ErrorException("InsertSections", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);

				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			this.Log.Add(MessageTypeEnum.TraceRepository, "");
			this.FillLogFromErrorBitList(this._sectionParser.ErrorBitList);
			//LogPrint();
		}



		public void ClearSections(string pathDB)
		{
			//this.Log.Clear();
			string sql1 = "DELETE FROM  [Section]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Section", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start Process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Section"));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Section"));
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearSections", error);
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
							SectionValidate.ConvertDataErrorCode2ErrorMessage(b) + " [ " + record + " ] ");
					}
				}

				if (errorType == MessageTypeEnum.WarningParser)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.WarningParser, //bitAndRecord.ErrorType.ToString() + " : " +
							 SectionValidate.ConvertDataErrorCode2WarningMessage(b) + " [ " + record + " ] ");
					}
				}
			}
		}
		
	}
}
