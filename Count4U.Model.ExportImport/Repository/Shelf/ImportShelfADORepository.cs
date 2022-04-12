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
using System.Threading;
using NLog;


namespace Count4U.Model.Count4U
{
	//не сделано
	public class ImportShelfADORepository : BaseImportADORepository, //IImportSelfRepository
	{
		//public readonly List<IIturParser> _iturParserList;
		private IShelfParser _shelfParser;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly IShelfRepository _shelfRepository;

		public ImportShelfADORepository(
			IConnectionADO connection,
			IServiceLocator serviceLocator,
			ILog log,
			IShelfRepository shelfRepository)
			: base(connection, log, serviceLocator)
        {
			if (serviceLocator == null) throw new ArgumentNullException("serviceLocator");
			if (shelfRepository == null) throw new ArgumentNullException("shelfRepository");

			this._shelfRepository = shelfRepository;
	    }

		private Dictionary<string, Itur> GetIturDictionary(string pathDB)
		{
			//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill  [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "IturDictionary"));
			//this._iturRepository.ClearIturDictionary();
			//this.Log.Add("Clear IturDictionary");
			Dictionary<string, Itur> iturFromDBDictionary =
				this._iturRepository.GetIturDictionary(pathDB, true);
			//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill  [{0}]"
			this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "IturDictionary"));
			return iturFromDBDictionary;
		}

		public void InsertIturs(string fromPathFile, string pathDB, IturParserEnum iturParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._iturParser = this._serviceLocator.GetInstance<IIturParser>(iturParserEnum.ToString());
		   if (this._iturParser == null)
			{
				//Localization.Resources.Log_Error1007%"In  IturParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1007, iturParserEnum));
				return;
			}

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
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "ImportIturRepository", "ImportIturADORepository"));
			//this.Log.Add("InventIturParser is [ " + inventProductSimpleParserEnum.ToString() + "]");

			Dictionary<string, Itur> iturFromDBDictionary = this.GetIturDictionary(pathDB);

			Dictionary<string, Itur> iturToDBDictionary = this._iturParser.GetIturs(fromPathFile, encoding, separators,
					countExcludeFirstString, iturFromDBDictionary, parms);

			this.FromDictionaryToDB(pathDB, iturToDBDictionary, cancellationToken, countAction);
			this.FillLogFromErrorBitList(this._iturParser.ErrorBitList);
			//LogPrint();
		}

		public void FromDictionaryToDB(string pathDB, Dictionary<string, Itur> iturToDBDictionary,
			CancellationToken cancellationToken, Action<long> countAction)
		{
			if (countAction == null)// throw new ArgumentNullException("ActionUpdateProgress is null");
			{
				countAction = CountLong;
			}
			string sql1 = "INSERT INTO [Itur](" +
				"[Name]" + 
				//",[Description]" + 
				//",[InitialQuantityMakatExpected]" + 
					",[IturCode]" +
					",[ERPIturCode]" +
				//",[Approve]" + 
				//",[Disabled]" + 
				//",[StatusIturCode]" + 
				//",[CreateDate]" + 
				//",[ModifyDate]" + 
				//",[Publishe]" + 
					",[StatusIturBit]" +
					",[Number]" +
					",[NumberPrefix]" +
					",[NumberSufix]" +
					",[LocationCode]" +
					 ")" +
			   " VALUES(" +
				 "@Name" + 
				//",@Description" + 
				//",@InitialQuantityMakatExpected" + 
					",@IturCode" +
					",@ERPIturCode" +
				//",@Approve" + 
				//",@Disabled" + 
				//",@StatusIturCode" + 
				//",@CreateDate" + 
				//",@ModifyDate" + 
				//",@Publishe" + 
					",@StatusIturBit" +
					",@Number" +
					",@NumberPrefix" +
					",@NumberSufix" +
					",@LocationCode" +
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
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Description", SqlDbType.NVarChar, 100));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@InitialQuantityMakatExpected", SqlDbType.Float, 50));			//?
				cmd.Parameters.Add(new SqlCeParameter(
					"@IturCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@ERPIturCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(		   //?
				//    "@Approve", SqlDbType.Bit));
				//cmd.Parameters.Add(new SqlCeParameter(		   //?
				//    "@Disabled", SqlDbType.Bit));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@StatusIturCode", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@CreateDate", SqlDbType.NVarChar, 50));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@ModifyDate", SqlDbType.NVarChar, 50));	 //?
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@Publishe", SqlDbType.Bit));		 //?
				cmd.Parameters.Add(new SqlCeParameter(
					"@StatusIturBit", SqlDbType.Int, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Number", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@NumberPrefix", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
				"@NumberSufix", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@LocationCode", SqlDbType.NVarChar, 50));

				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Itur", pathDB));
				//LogPrint();
				int k = 0;
				//countAction(k);
				foreach (KeyValuePair<string, Itur> keyValuePair in iturToDBDictionary)
				//this._iturParser.GetIturs(fromPathFile, encoding, separators, 
				//countExcludeFirstString, iturFromDBDictionary))
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
					Itur val = keyValuePair.Value;

					cmd.Parameters["@IturCode"].Value = val.IturCode;
					cmd.Parameters["@ERPIturCode"].Value = val.ERPIturCode != null ? val.ERPIturCode : "";
					cmd.Parameters["@Name"].Value = val.Name;
					//cmd.Parameters["@Description"].Value = itur.Description;
					//cmd.Parameters["@InitialQuantityMakatExpected"].Value = itur.InitialQuantityMakatExpected;//?
					//cmd.Parameters["@Approve"].Value = itur.Approve;		  //?
					//cmd.Parameters["@Disabled"].Value = itur.Disabled;		  //?
					//cmd.Parameters["@StatusIturCode"].Value = val.StatusIturCode != null ? val.StatusIturCode : DomainUnknownCode.UnknownStatus;
					//cmd.Parameters["@CreateDate"].Value = itur.CreateDate;
					//cmd.Parameters["@ModifyDate"].Value = DateTime.Now;
					//cmd.Parameters["@Publishe"].Value = itur.Publishe;	   //?
					cmd.Parameters["@StatusIturBit"].Value = val.StatusIturBit; //(int)IturStatusGroupEnum.Empty;
					cmd.Parameters["@Number"].Value = val.Number;
					cmd.Parameters["@NumberPrefix"].Value = val.NumberPrefix;
					cmd.Parameters["@NumberSufix"].Value = val.NumberSufix;
					cmd.Parameters["@LocationCode"].Value = val.LocationCode != null ? val.LocationCode : DomainUnknownCode.UnknownLocation;
					//this._locationDictionary.Add(product.Barcode,
					//    new ProductSimple { ProductID = 0, ProductName = product.Name });

					cmd.ExecuteNonQuery();

				}
				//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportShelfADORepository"));
				tran.Commit();
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "Itur", pathDB));
				//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportShelfADORepository"));
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

			
			//return cancellationToken;
		}

		public void ClearIturs(string pathDB)
		{
			string sql1 = "DELETE FROM  [Shelf]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Shelf", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Shelf"));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Shelf"));
				this.Log.Add(MessageTypeEnum.TraceRepository, "");
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearIturs", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
			//LogPrint();
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
