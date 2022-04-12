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
using System.IO;


namespace Count4U.Model.Count4U
{
	public class ImportPropertyStrADORepository : BaseImportADORepository, IImportPropertyStrRepository
	{
		private IPropertyStrParser _propertyStrParser;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly IPropertyStrRepository _propertyStrRepository;

		public ImportPropertyStrADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IPropertyStrRepository propertyStrRepository)
			: base(connection,dbSettings,  log, serviceLocator)
        {
			if (serviceLocator == null) throw new ArgumentNullException("serviceLocator");
			if (propertyStrRepository == null) throw new ArgumentNullException("propertyStrRepository");

			this._propertyStrRepository = propertyStrRepository;
	    }

		private Dictionary<string, PropertyStr> GetPropertyStrDictionary(DomainObjectTypeEnum  domainObjectType, string pathDB)
		{
			//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill  [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "PropertyStrDictionary"));
			Dictionary<string, PropertyStr> propertyStrFromDBDictionary =
				this._propertyStrRepository.GetPropertyStrDictionary(domainObjectType.ToString(), pathDB, true);
			//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill  [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "PropertyStrDictionary"));
			return propertyStrFromDBDictionary;
		}

		public void InsertPropertyStrs(string fromPathFile, string pathDB, 
			PropertyStrParserEnum propertyStrParserEnum,
			DomainObjectTypeEnum  domainObjectType,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._propertyStrParser = this._serviceLocator.GetInstance<IPropertyStrParser>(propertyStrParserEnum.ToString());
		   if (this._propertyStrParser == null)
			{
				//Localization.Resources.Log_Error1007%"In  IturParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1007, propertyStrParserEnum));
				return;
			}

		   //if (File.Exists(fromPathFile) == false)
		   //{
		   //	this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
		   //	return;
		   //}

			//this.Log.Clear();
			//this._locationDictionary.Clear();
		   CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None)	//throw new ArgumentNullException("CancellationToken.None");
			{
				var c = new CancellationTokenSource();
				cancellationToken = c.Token;
			}

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null)// throw new ArgumentNullException("ActionUpdateProgress is null");
			{
				countAction = CountLong;
			}

			//Localization.Resources.Log_TraceRepository1040%"[{0}]  is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "ImportPropertyStrRepository", "ImportPropertyStrADORepository"));
			//this.Log.Add("InventPropertyStrParser is [ " + inventProductSimpleParserEnum.ToString() + "]");

			Dictionary<string, PropertyStr> propertyStrFromDBDictionary = this.GetPropertyStrDictionary(domainObjectType, pathDB);

			// сохранили в словаре из БД
			Dictionary<string, PropertyStr> propertyStrToDBDictionary = this._propertyStrParser.GetPropertyStrs(fromPathFile,
					domainObjectType, encoding, separators,
					countExcludeFirstString, propertyStrFromDBDictionary, parms);
			// удалили из БД
			if (importType.Contains(ImportDomainEnum.ClearByDomainObjectType) == true)
			{
				this._propertyStrRepository.DeleteAllByDomainObjectType(domainObjectType.ToString(), pathDB);
			}
			//заполнили БД 
			this.FromDictionaryToDB(pathDB, propertyStrToDBDictionary, cancellationToken, countAction);
			this.FillLogFromErrorBitList(this._propertyStrParser.ErrorBitList);
			//LogPrint();
		}

		public void FromDictionaryToDB(string pathDB, Dictionary<string, PropertyStr> propertyStrToDBDictionary,
			CancellationToken cancellationToken, Action<long> countAction)
		{
			if (countAction == null)// throw new ArgumentNullException("ActionUpdateProgress is null");
			{
				countAction = CountLong;
			}
	 //	   INSERT INTO [PropertyStr]
	 //	  ([ID]
	 //	  ,[TypeCode]
	 //	  ,[PropertyStrCode]
	 //	  ,[Name]
	 //	  ,[DomainObject]
	 //	  ,[Code])
	 //VALUES
	 //	  (<ID, bigint,>
	 //	  ,<TypeCode, nvarchar(50),>
	 //	  ,<PropertyStrCode, nvarchar(50),>
	 //	  ,<Name, nvarchar(50),>
	 //	  ,<DomainObject, nvarchar(50),>
	 //	  ,<Code, nvarchar(50),>);

			string sql1 = "INSERT INTO [PropertyStr] (" +
          " [TypeCode] " +
           ",[PropertyStrCode] " +
           ",[Name]" +
           ",[DomainObject]" +
           ",[Code])" +
			"VALUES(" +
				 "@TypeCode" +
				",@PropertyStrCode" +
				",@Name" +
				",@DomainObject" +
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
					"@TypeCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PropertyStrCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Name", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@DomainObject", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Code", SqlDbType.NVarChar, 50));
				
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "PropertyStr", pathDB));
				//LogPrint();
				int k = 0;
				//countAction(k);
				foreach (KeyValuePair<string, PropertyStr> keyValuePair in propertyStrToDBDictionary)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}
					k++;
					if (k % 10 == 0)
					{
						countAction(k);
						//tran.Commit();
						//tran = sqlCeConnection.BeginTransaction();
					}
					string key = keyValuePair.Key;
					PropertyStr val = keyValuePair.Value;
					cmd.Parameters["@TypeCode"].Value = val.TypeCode != null ? val.TypeCode : "";
					cmd.Parameters["@PropertyStrCode"].Value = val.PropertyStrCode != null ? val.PropertyStrCode : "";
					cmd.Parameters["@Name"].Value = val.Name != null ? val.Name : "";
					cmd.Parameters["@DomainObject"].Value = val.DomainObject != null ? val.DomainObject : "Unknown";
					cmd.Parameters["@Code"].Value = val.Code != null ? val.Code : "";
					cmd.ExecuteNonQuery();

				}
				//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportPropertyStrADORepository"));
				tran.Commit();
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "PropertyStr", pathDB));
				//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportPropertyStrADORepository"));
			}
			catch (Exception error)
			{
				_logger.ErrorException("FromDictionaryToDB", error);
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
			//return cancellationToken;
		}

		public void Clear(string pathDB, string sql1)
		{
		//	string sql1 = "DELETE FROM  [PropertyStr]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "PropertyStr", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "PropertyStr"));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "PropertyStr"));
				this.Log.Add(MessageTypeEnum.TraceRepository, "");
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearPropertyStrs", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
			//LogPrint();
		}

		public void ClearPropertyStrs(DomainObjectTypeEnum domainObject, string pathDB)
		{
			//DELETE FROM Customers WHERE ([Company Name] = 'Wide World Importers')
			string sql = @"DELETE FROM PropertyStr WHERE ([DomainObject] =" + @"'" + domainObject.ToString() + @"'" + ")";
			this.Clear(pathDB, sql);
		}

		public void ClearPropertyStrs(string pathDB)
		{
			this.Clear(pathDB, @"DELETE FROM  [PropertyStr]");
		}
	
		public void FillLogFromErrorBitList(List<BitAndRecord> errorBitList)
		{
			if (errorBitList == null) return;
			if (errorBitList.Count == 0) return;
			//Log_TraceParser1001% "Parser Error And Message : "
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
						this.Log.Add(MessageTypeEnum.Error, // bitAndRecord.ErrorType.ToString() + " : "+
							IturValidate.ConvertDataErrorCode2ErrorMessage(b) + " [ " + record + " ] ");
					}
				}

				if (errorType == MessageTypeEnum.WarningParser)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.WarningParser, //bitAndRecord.ErrorType.ToString() + " : "+
							IturValidate.ConvertDataErrorCode2WarningMessage(b) +" [ " + record + " ] ");
					}
				}
			}
		}

		private void CountLong(long count)
		{
		}

	

	

	}
}
