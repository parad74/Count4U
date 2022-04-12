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
using Count4U.Model.Interface.Count4Mobile;
using System.IO;


namespace Count4U.Model.Count4U
{
	public class ImportBuildingConfigADORepository : BaseImportADORepository, IImportBuildingConfigADORepository
	{
		public readonly List<IBuildingConfigParser> _buildingConfigParserList;
		private IBuildingConfigParser _buildingConfigParser;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly IBuildingConfigRepository _buildingConfigRepository;

		public ImportBuildingConfigADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IBuildingConfigRepository buildingConfigRepository)
			: base(connection, dbSettings, log, serviceLocator)
        {
			if (serviceLocator == null) throw new ArgumentNullException("serviceLocator");
			if (buildingConfigRepository == null) throw new ArgumentNullException("buildingConfigRepository");

			this._buildingConfigRepository = buildingConfigRepository;
	    }

		private Dictionary<string, string> GetBuildingConfigDictionary(string pathDB)
		{
			Dictionary<string, string> dictionaryBuildingConfigCode = new Dictionary<string, string>();
	
			BuildingConfigs buildingConfigs =	this._buildingConfigRepository.GetBuildingConfigs(pathDB);
			foreach (BuildingConfig buildingConfig in buildingConfigs)
			{
				try
				{
					dictionaryBuildingConfigCode[buildingConfig.Name] = buildingConfig.Ord.ToString();
				}
				catch { }
			}
			return dictionaryBuildingConfigCode;
		}

		public void InsertBuildingConfig(string fromPathFile, string pathDB,
			BuildingConfigParserEnum buildingConfigParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._buildingConfigParser = this._serviceLocator.GetInstance<IBuildingConfigParser>(buildingConfigParserEnum.ToString());
		   if (this._buildingConfigParser == null)
			{
				//Localization.Resources.Log_Error1007%"In  IturParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1007, buildingConfigParserEnum));
				return;
			}

		   if (File.Exists(fromPathFile) == false)
		   {
			   this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
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
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "ImportBuildingConfigRepository", "ImportBuildingConfigADORepository"));
			//this.Log.Add("InventIturParser is [ " + inventProductSimpleParserEnum.ToString() + "]");

			Dictionary<string, string> buildingConfigFromDBDictionary = this.GetBuildingConfigDictionary( pathDB);

			Dictionary<string, BuildingConfig> buildingConfigToDBDictionary = this._buildingConfigParser.GetBuildingConfigs(
						fromPathFile, encoding, separators,
						countExcludeFirstString,
						buildingConfigFromDBDictionary, DomainObjectTypeEnum.BuildingConfig, parms);

			this.FromDictionaryToDB(pathDB, buildingConfigToDBDictionary, cancellationToken, countAction);
			this.FillLogFromErrorBitList(this._buildingConfigParser.ErrorBitList);
			//LogPrint();
		}

		public void FromDictionaryToDB(string pathDB, Dictionary<string, BuildingConfig> iturToDBDictionary,
			CancellationToken cancellationToken, Action<long> countAction)
		{
			if (countAction == null)// throw new ArgumentNullException("ActionUpdateProgress is null");
			{
				countAction = CountLong;
			}
			//string sql1 = "INSERT INTO [Itur](" +
			//	"[Name]" + 
			//	",[Description]" + 
			//	//",[InitialQuantityMakatExpected]" + 
			//		",[IturCode]" +
			//		",[ERPIturCode]" +
			//	//",[Approve]" + 
			//	//",[Disabled]" + 
			//	//",[StatusIturCode]" + 
			//	//",[CreateDate]" + 
			//	//",[ModifyDate]" + 
			//	//",[Publishe]" + 
			//		",[StatusIturBit]" +
			//		",[Number]" +
			//		",[NumberPrefix]" +
			//		",[NumberSufix]" +
			//		",[LocationCode]" +
			//",[Width]" +
			//",[Height]" +
			//",[IncludeInFacing]" +
			//",[ShelfCount]" +
			//",[ShelfInItur]" +
			//",[PlaceCount]" +
			//",[PlaceInItur]" +
			//",[Supplier1PlaceCount]" +
			//",[Supplier2PlaceCount]" +
			//",[Supplier3PlaceCount]" +
			//",[Supplier4PlaceCount]" +
			//",[Supplier5PlaceCount]" +
			//",[SupplierOtherPlaceCount]" +
			//",[UnitPlaceWidth]" +
			//",[Area]" +
			//",[AreaCount]" +
			//		 ")" +

			//   " VALUES(" +
			//	 "@Name" + 
			//	",@Description" + 
			//	//",@InitialQuantityMakatExpected" + 
			//		",@IturCode" +
			//		",@ERPIturCode" +
			//	//",@Approve" + 
			//	//",@Disabled" + 
			//	//",@StatusIturCode" + 
			//	//",@CreateDate" + 
			//	//",@ModifyDate" + 
			//	//",@Publishe" + 
			//		",@StatusIturBit" +
			//		",@Number" +
			//		",@NumberPrefix" +
			//		",@NumberSufix" +
			//		",@LocationCode" +
			//",@Width" +
			//",@Height" +
			//",@IncludeInFacing" +
			//",@ShelfCount" +
			//",@ShelfInItur" +
			//",@PlaceCount" +
			//",@PlaceInItur" +
			//",@Supplier1PlaceCount" +
			//",@Supplier2PlaceCount" +
			//",@Supplier3PlaceCount" +
			//",@Supplier4PlaceCount" +
			//",@Supplier5PlaceCount" +
			//",@SupplierOtherPlaceCount" +
			//",@UnitPlaceWidth" +
			//",@Area" +
			//",@AreaCount" +
			//		 ")";


			SqlCeTransaction tran = null;

			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
			//	var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

			//	cmd.Parameters.Add(new SqlCeParameter(
			//		"@Name", SqlDbType.NVarChar, 50));
			//	cmd.Parameters.Add(new SqlCeParameter(
			//		"@Description", SqlDbType.NVarChar, 500));
			//	//cmd.Parameters.Add(new SqlCeParameter(
			//	//    "@InitialQuantityMakatExpected", SqlDbType.Float, 50));			//?
			//	cmd.Parameters.Add(new SqlCeParameter(
			//		"@IturCode", SqlDbType.NVarChar, 50));
			//	cmd.Parameters.Add(new SqlCeParameter(
			//		"@ERPIturCode", SqlDbType.NVarChar, 50));
			//	//cmd.Parameters.Add(new SqlCeParameter(		   //?
			//	//    "@Approve", SqlDbType.Bit));
			//	//cmd.Parameters.Add(new SqlCeParameter(		   //?
			//	//    "@Disabled", SqlDbType.Bit));
			//	//cmd.Parameters.Add(new SqlCeParameter(
			//	//    "@StatusIturCode", SqlDbType.NVarChar, 50));
			//	//cmd.Parameters.Add(new SqlCeParameter(
			//	//    "@CreateDate", SqlDbType.NVarChar, 50));
			//	//cmd.Parameters.Add(new SqlCeParameter(
			//	//    "@ModifyDate", SqlDbType.NVarChar, 50));	 //?
			//	//cmd.Parameters.Add(new SqlCeParameter(
			//	//    "@Publishe", SqlDbType.Bit));		 //?
			//	cmd.Parameters.Add(new SqlCeParameter(
			//		"@StatusIturBit", SqlDbType.Int, 50));
			//	cmd.Parameters.Add(new SqlCeParameter(
			//		"@Number", SqlDbType.Int));
			//	cmd.Parameters.Add(new SqlCeParameter(
			//		"@NumberPrefix", SqlDbType.NVarChar, 50));
			//	cmd.Parameters.Add(new SqlCeParameter(
			//	"@NumberSufix", SqlDbType.NVarChar, 50));
			//	cmd.Parameters.Add(new SqlCeParameter(
			//		"@LocationCode", SqlDbType.NVarChar, 50));
			//		cmd.Parameters.Add(new SqlCeParameter("@Width", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@Height", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@IncludeInFacing", SqlDbType.Bit));
			//		cmd.Parameters.Add(new SqlCeParameter("@ShelfCount", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@ShelfInItur", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@PlaceCount", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@PlaceInItur", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@Supplier1PlaceCount", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@Supplier2PlaceCount", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@Supplier3PlaceCount", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@Supplier4PlaceCount", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@Supplier5PlaceCount", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@SupplierOtherPlaceCount", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@UnitPlaceWidth", SqlDbType.Int));
			//		cmd.Parameters.Add(new SqlCeParameter("@Area", SqlDbType.Float, 50));	
			//		cmd.Parameters.Add(new SqlCeParameter("@AreaCount", SqlDbType.Float, 50));	
			//	//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
			//	this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Itur", pathDB));
			//	//LogPrint();
				int k = 0;
			//	//countAction(k);
			//	foreach (KeyValuePair<string, Itur> keyValuePair in iturToDBDictionary)
			//	//this._iturParser.GetIturs(fromPathFile, encoding, separators, 
			//	//countExcludeFirstString, iturFromDBDictionary))
			//	{
			//		if (cancellationToken.IsCancellationRequested == true)
			//		{
			//			break;
			//		}
			//		k++;
			//		if (k % 10 == 0)
			//		{
			//			countAction(k);
			//			//tran.Commit();
			//			//tran = sqlCeConnection.BeginTransaction();
			//		}
			//		string key = keyValuePair.Key;
			//		Itur val = keyValuePair.Value;

			//		cmd.Parameters["@IturCode"].Value = val.IturCode;
			//		cmd.Parameters["@ERPIturCode"].Value = val.ERPIturCode != null ? val.ERPIturCode : "";
			//		cmd.Parameters["@Name"].Value = val.Name;
			//		cmd.Parameters["@Description"].Value = val.Description != null ? val.Description : "";
			//		//cmd.Parameters["@InitialQuantityMakatExpected"].Value = itur.InitialQuantityMakatExpected;//?
			//		//cmd.Parameters["@Approve"].Value = itur.Approve;		  //?
			//		//cmd.Parameters["@Disabled"].Value = itur.Disabled;		  //?
			//		//cmd.Parameters["@StatusIturCode"].Value = val.StatusIturCode != null ? val.StatusIturCode : DomainUnknownCode.UnknownStatus;
			//		//cmd.Parameters["@CreateDate"].Value = itur.CreateDate;
			//		//cmd.Parameters["@ModifyDate"].Value = DateTime.Now;
			//		//cmd.Parameters["@Publishe"].Value = itur.Publishe;	   //?
			//		cmd.Parameters["@StatusIturBit"].Value = val.StatusIturBit; //(int)IturStatusGroupEnum.Empty;
			//		cmd.Parameters["@Number"].Value = val.Number;
			//		cmd.Parameters["@NumberPrefix"].Value = val.NumberPrefix;
			//		cmd.Parameters["@NumberSufix"].Value = val.NumberSufix;
			//		cmd.Parameters["@LocationCode"].Value = val.LocationCode != null ? val.LocationCode : DomainUnknownCode.UnknownLocation;
			//		cmd.Parameters["@Width"].Value = val.Width;
			//		cmd.Parameters["@Height"].Value = val.Height;
			//		cmd.Parameters["@IncludeInFacing"].Value = val.IncludeInFacing;
			//		cmd.Parameters["@ShelfCount"].Value = val.ShelfCount;
			//		cmd.Parameters["@ShelfInItur"].Value = val.ShelfInItur;
			//		cmd.Parameters["@PlaceCount"].Value = val.PlaceCount;
			//		cmd.Parameters["@PlaceInItur"].Value = val.PlaceInItur;
			//		cmd.Parameters["@Supplier1PlaceCount"].Value = val.Supplier1PlaceCount;
			//		cmd.Parameters["@Supplier2PlaceCount"].Value = val.Supplier2PlaceCount;
			//		cmd.Parameters["@Supplier3PlaceCount"].Value = val.Supplier3PlaceCount;
			//		cmd.Parameters["@Supplier4PlaceCount"].Value = val.Supplier4PlaceCount;
			//		cmd.Parameters["@Supplier5PlaceCount"].Value = val.Supplier5PlaceCount;
			//		cmd.Parameters["@SupplierOtherPlaceCount"].Value = val.SupplierOtherPlaceCount;
			//		cmd.Parameters["@UnitPlaceWidth"].Value = val.UnitPlaceWidth;
			//		cmd.Parameters["@Area"].Value = val.Area;
			//		cmd.Parameters["@AreaCount"].Value = val.AreaCount;
			//		//this._locationDictionary.Add(product.Barcode,
			//		//    new ProductSimple { ProductID = 0, ProductName = product.Name });

			//		cmd.ExecuteNonQuery();

			//	}
				//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportIturADORepository"));
				tran.Commit();
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "Itur", pathDB));
				//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportIturADORepository"));
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

		public void ClearBuildingConfig(string pathDB)
		{
			//string sql1 = "DELETE FROM  [BuildingConfig]";
			//SqlCeTransaction tran = null;
			//string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			//SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			//try
			//{
			//	sqlCeConnection.Open();
			//	tran = sqlCeConnection.BeginTransaction();
			//	var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
			//	//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
			//	this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Itur", pathDB));
			//	//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
			//	this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Itur"));
			//	//LogPrint();
			//	cmd.ExecuteNonQuery();
			//	//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
			//	this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Itur"));
			//	this.Log.Add(MessageTypeEnum.TraceRepository, "");
			//	tran.Commit();
			//}
			//catch (Exception error)
			//{
			//	_logger.ErrorException("ClearBuildingConfigs", error);
			//	this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
			//	tran.Rollback();
			//}
			//finally
			//{
			//	sqlCeConnection.Close();
			//}
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
