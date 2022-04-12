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
	public class ImportSupplierADORepository : BaseImportADORepository, IImportSupplierRepository
	{
		private ISupplierParser _supplierParser;
		private readonly ISupplierRepository _supplierRepository;
		private Dictionary<string, Supplier> _supplierDictionary;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportSupplierADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			ISupplierRepository supplierRepository)
			: base(connection,dbSettings, log, serviceLocator)
        {
			//if (locationParser == null) throw new ArgumentNullException("locationParser");
			//if (serviceLocator == null) throw new ArgumentNullException("serviceLocator");
			if (supplierRepository == null) throw new ArgumentNullException("supplierRepository");

			this._supplierRepository = supplierRepository;
			this._supplierDictionary = new Dictionary<string, Supplier>();
	    }

		private Dictionary<string, Supplier> GetSupplierDictionary(string pathDB)
		{
			//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "SupplierDictionary"));
			this._supplierRepository.ClearSupplierDictionary();
			//Localization.Resources.Log_TraceRepository1045%"Clear [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1045, "SupplierDictionary"));
			Dictionary<string, Supplier> supplierFromDBDictionary = this._supplierRepository.GetSupplierDictionary(pathDB, true);
			//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill  [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "SupplierDictionary"));
			return supplierFromDBDictionary;
		}

		public void InsertSuppliers(string fromPathFile, string pathDB, SupplierParserEnum supplierParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._supplierParser = this._serviceLocator.GetInstance<ISupplierParser>(supplierParserEnum.ToString());

			if (this._supplierParser == null)
			{
				//Localization.Resources.Log_Error1010%"In  {0} {1} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1009, "ParserList", supplierParserEnum));
				return;
			}

			if (File.Exists(fromPathFile) == false)
			{
				this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
				return;
			}

			Dictionary<string, Supplier> supplierFromDBDictionary = this.GetSupplierDictionary(pathDB);
			//Localization.Resources.Log_TraceRepository1040%"[{0}]  is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "ImportSupplier Repository", "ImportSupplierADORepository"));
			//this.Log.Add("InventIturParser is [ " + inventProductSimpleParserEnum.ToString() + "]");

			string sql1 = "INSERT INTO [Supplier](" +
		   "[Name]" + 
		   ",[Description]" +
		   ",[SupplierCode]" +
		   ",[SupplierLevel]" + 
			")" + 
      " VALUES(" +
			"@Name" +
		   ",@Description" +
		   ",@SupplierCode" +
		   ",@SupplierLevel" +
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
					"@SupplierCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SupplierLevel", SqlDbType.Int));
				
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Supplier", pathDB));
				int k = 0;
				foreach (KeyValuePair<string, Supplier> keyValuePair in
					this._supplierParser.GetSuppliers(fromPathFile, encoding, separators, 
					countExcludeFirstString,
					supplierFromDBDictionary,
					parms))
				{
					k++;
					string key = keyValuePair.Key;
					Supplier val = keyValuePair.Value;
					cmd.Parameters["@SupplierCode"].Value = val.SupplierCode;
					cmd.Parameters["@Name"].Value = val.Name;
					cmd.Parameters["@Description"].Value = val.Description;
					cmd.Parameters["@SupplierLevel"].Value = val.SupplierLevel;
					cmd.ExecuteNonQuery();
				}
				//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportSupplierADORepository"));
				tran.Commit();
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "Supplier", pathDB));
				//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportSupplierADORepository"));
			}

			catch (Exception error)
			{
				_logger.ErrorException("InsertSuppliers", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);

				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}

			this.Log.Add(MessageTypeEnum.TraceRepository, "");
			this.FillLogFromErrorBitList(this._supplierParser.ErrorBitList);

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			//LogPrint();
		}



		public void ClearSuppliers(string pathDB)
		{
			//this.Log.Clear();
			string sql1 = "DELETE FROM  [Supplier]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Supplier", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start Process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Supplier"));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Supplier"));
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearSuppliers", error);
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
							SupplierValidate.ConvertDataErrorCode2ErrorMessage(b) + " [ " + record + " ] ");
					}
				}

				if (errorType == MessageTypeEnum.WarningParser)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.WarningParser, //bitAndRecord.ErrorType.ToString() + " : " +
							 SupplierValidate.ConvertDataErrorCode2WarningMessage(b) + " [ " + record + " ] ");
					}
				}
			}
		}
		
	}
}
