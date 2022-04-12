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
using System.Threading;
using ErikEJ.SqlCe;
using System.Data.SqlClient;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4Mobile;
using System.IO;
using Count4U.Model.Common;
using System.Data.Entity;

namespace Count4U.Model.Count4U
{
	public class CurrentInventoryAdvancedReaderBlukRepository : BaseCopyBulkRepository, IImportCurrentInventoryAdvancedBlukRepository
	{
		private ICurrentInventoryAdvancedReader _currentInventoryAdvancedReader;

		public CurrentInventoryAdvancedReaderBlukRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log
			)
			: base(connection, dbSettings, log, serviceLocator)
		{
			//Database.SetInitializer<AnalyticDBContext>(new AnalyticDBContextInitializer());
		}

		public	void InsertCurrentInventoryAdvanced(string pathDB,
			CurrentInventoryAdvancedReaderEnum currentInventoryAdvancedReaderEnum, 
			bool refill = true, 
			bool refillCatalogDictionary = false,
			SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null, 
		//	bool addResult = true, 
			List<ImportDomainEnum> importType = null, 
			List<string[]> columnMappings = null)
		{

			Action<long> countAction = null;
			CancellationToken cancellationToken = CancellationToken.None;
			Dictionary<ImportProviderParmEnum, object> parms = new Dictionary<ImportProviderParmEnum, object>();
			if (parmsIn != null)
			{
				parms = parmsIn.ConvertToImportProviderParmEnum();
				cancellationToken = parms.GetCancellationTokenFromParm();
			}
			if (cancellationToken == CancellationToken.None)	//throw new ArgumentNullException("CancellationToken.None");
			{
				var c = new CancellationTokenSource();
				cancellationToken = c.Token;
			}
			countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null)// throw new ArgumentNullException("ActionUpdateProgress is null");
			{
				countAction = CountLong;
			}
	
	
			this._currentInventoryAdvancedReader =
						this._serviceLocator.GetInstance<ICurrentInventoryAdvancedReader>(currentInventoryAdvancedReaderEnum.ToString());
			if (this._currentInventoryAdvancedReader == null)
			{
				//TODO Localization
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, this._currentInventoryAdvancedReader.ToString()));
				return;
			}

			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, " IturAnalyzesBulk Repository", "[IturAnalyzesBulkRepository]"));
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "IturAnalyzesReader", _currentInventoryAdvancedReader.ToString()));

			int i = 0;

			string sessionCodeIn = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			//========
			#region CurrentInventoryAdvanced
			string tableName = @"CurrentInventoryAdvanced";
			string analyticDBFile = base.DbSettings.AnalyticDBFile;
			string connectionString = base.ConnectionADO.GetADOConnectionStringBySubFolder(pathDB, analyticDBFile);

			IEnumerable<CurrentInventoryAdvanced> currentInventoryAdvanced = 
				this._currentInventoryAdvancedReader.GetCurrentInventoryAdvanceds(pathDB,
					refill, refillCatalogDictionary, selectParms, parmsIn, importType);


			IEnumerable<CurrentInventoryAdvanced> test1 = currentInventoryAdvanced.ToList();
			long count1 = base.CountRow(tableName, pathDB, analyticDBFile);

			DoBulkCopyMapping<CurrentInventoryAdvanced>(tableName, test1, true, pathDB, columnMappings, analyticDBFile);

			long count = base.CountRow(tableName, pathDB, analyticDBFile);
			countAction(count);
			//Localization.Resources.Log_TraceRepositoryResult1062%"Destination Table [{0}] in DB [{1}] {2} after Copy, have [{3}] Row"
			this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1062,
				tableName, pathDB, Environment.NewLine, count));

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

				#endregion IturAnalyzes
		}

		private void CountLong(long count)
		{
		}




		#region IImportCurrentInventoryAdvancedBlukRepository Members


		public void ClearCurrentInventoryAdvanced(string pathDB)
		{
			base.DeleteTable(@"CurrentInventoryAdvanced", pathDB, "AnalyticDB.sdf");
		}


		public void DropCurrentInventoryAdvanced(string pathDB)
		{
			string analyticDBFile = DbSettings.AnalyticDBFile;
			string analyticPathDB = base.ConnectionADO.BuildPathFileADO(pathDB, analyticDBFile);
			if (File.Exists(analyticPathDB) == false) return;

			//this.Log.Clear();
			//string sql1 = @"DROP TABLE [PreviousInventory]";
			string sql1 = @"DROP TABLE [CurrentInventoryAdvanced]";

			string sql2 = @"CREATE TABLE [CurrentInventoryAdvanced] (
  [Id] bigint IDENTITY (1,1)  NOT NULL
, [Uid] nvarchar(250)  NOT NULL
, [SerialNumberLocal] nvarchar(250)  NOT NULL
, [ItemCode] nvarchar(300)  NOT NULL
, [DomainObject] nvarchar(50)  NOT NULL
, [Table] nvarchar(50)  NOT NULL
, [Adapter] nvarchar(50)  NOT NULL
, [SerialNumberSupplier] nvarchar(50)  NOT NULL
, [Quantity] nvarchar(50)  NOT NULL
, [QuantityDouble] float NOT NULL
, [PropertyStr1] nvarchar(100)  NOT NULL
, [PropertyStr1Code] nvarchar(100)  NOT NULL
, [PropertyStr1Name] nvarchar(100)  NOT NULL
, [PropertyStr2] nvarchar(100)  NOT NULL
, [PropertyStr2Code] nvarchar(100)  NOT NULL
, [PropertyStr2Name] nvarchar(100)  NOT NULL
, [PropertyStr3] nvarchar(100)  NOT NULL
, [PropertyStr3Code] nvarchar(100)  NOT NULL
, [PropertyStr3Name] nvarchar(100)  NOT NULL
, [PropertyStr4] nvarchar(100)  NOT NULL
, [PropertyStr4Code] nvarchar(100)  NOT NULL
, [PropertyStr4Name] nvarchar(100)  NOT NULL
, [PropertyStr5] nvarchar(100)  NOT NULL
, [PropertyStr5Code] nvarchar(100)  NOT NULL
, [PropertyStr5Name] nvarchar(100)  NOT NULL
, [PropertyStr6] nvarchar(100)  NOT NULL
, [PropertyStr6Code] nvarchar(100)  NOT NULL
, [PropertyStr6Name] nvarchar(100)  NOT NULL
, [PropertyStr7] nvarchar(100)  NOT NULL
, [PropertyStr7Code] nvarchar(100)  NOT NULL
, [PropertyStr7Name] nvarchar(100)  NOT NULL
, [PropertyStr8] nvarchar(100)  NOT NULL
, [PropertyStr8Code] nvarchar(100)  NOT NULL
, [PropertyStr8Name] nvarchar(100)  NOT NULL
, [PropertyStr9] nvarchar(100)  NOT NULL
, [PropertyStr9Code] nvarchar(100)  NOT NULL
, [PropertyStr9Name] nvarchar(100)  NOT NULL
, [PropertyStr10] nvarchar(100)  NOT NULL
, [PropertyStr10Code] nvarchar(100)  NOT NULL
, [PropertyStr10Name] nvarchar(100)  NOT NULL
, [PropertyStr11] nvarchar(100)  NOT NULL
, [PropertyStr11Code] nvarchar(100)  NOT NULL
, [PropertyStr11Name] nvarchar(100)  NOT NULL
, [PropertyStr12] nvarchar(100)  NOT NULL
, [PropertyStr12Code] nvarchar(100)  NOT NULL
, [PropertyStr12Name] nvarchar(100)  NOT NULL
, [PropertyStr13] nvarchar(100)  NOT NULL
, [PropertyStr13Code] nvarchar(100)  NOT NULL
, [PropertyStr13Name] nvarchar(100)  NOT NULL
, [PropertyStr14] nvarchar(100)  NOT NULL
, [PropertyStr14Code] nvarchar(100)  NOT NULL
, [PropertyStr14Name] nvarchar(100)  NOT NULL
, [PropertyStr15] nvarchar(100)  NOT NULL
, [PropertyStr15Code] nvarchar(100)  NOT NULL
, [PropertyStr15Name] nvarchar(100)  NOT NULL
, [PropertyStr16] nvarchar(100)  NOT NULL
, [PropertyStr16Code] nvarchar(100)  NOT NULL
, [PropertyStr16Name] nvarchar(100)  NOT NULL
, [PropertyStr17] nvarchar(100)  NOT NULL
, [PropertyStr17Code] nvarchar(100)  NOT NULL
, [PropertyStr17Name] nvarchar(100)  NOT NULL
, [PropertyStr18] nvarchar(100)  NOT NULL
, [PropertyStr18Code] nvarchar(100)  NOT NULL
, [PropertyStr18Name] nvarchar(100)  NOT NULL
, [PropertyStr19] nvarchar(100)  NOT NULL
, [PropertyStr19Code] nvarchar(100)  NOT NULL
, [PropertyStr19Name] nvarchar(100)  NOT NULL
, [PropertyStr20] nvarchar(100)  NOT NULL
, [PropertyStr20Code] nvarchar(100)  NOT NULL
, [PropertyStr20Name] nvarchar(100)  NOT NULL
, [PropExtenstion1] nvarchar(100)  NOT NULL
, [PropExtenstion2] nvarchar(100)  NOT NULL
, [PropExtenstion3] nvarchar(100)  NOT NULL
, [PropExtenstion4] nvarchar(100)  NOT NULL
, [PropExtenstion5] nvarchar(100)  NOT NULL
, [PropExtenstion6] nvarchar(100)  NOT NULL
, [PropExtenstion7] nvarchar(100)  NOT NULL
, [PropExtenstion8] nvarchar(100)  NOT NULL
, [PropExtenstion9] nvarchar(100)  NOT NULL
, [PropExtenstion10] nvarchar(100)  NOT NULL
, [PropExtenstion11] nvarchar(100)  NOT NULL
, [PropExtenstion12] nvarchar(100)  NOT NULL
, [PropExtenstion13] nvarchar(100)  NOT NULL
, [PropExtenstion14] nvarchar(100)  NOT NULL
, [PropExtenstion15] nvarchar(100)  NOT NULL
, [PropExtenstion16] nvarchar(100)  NOT NULL
, [PropExtenstion17] nvarchar(100)  NOT NULL
, [PropExtenstion18] nvarchar(100)  NOT NULL
, [PropExtenstion19] nvarchar(100)  NOT NULL
, [PropExtenstion20] nvarchar(100)  NOT NULL
, [PropExtenstion21] nvarchar(100)  NOT NULL
, [PropExtenstion22] nvarchar(100)  NOT NULL
, [LocationCode] nvarchar(250)  NOT NULL
, [LocationDescription] nvarchar(500)  NOT NULL
, [LocationLevel1Code] nvarchar(50)  NOT NULL
, [LocationLevel1Name] nvarchar(250)  NOT NULL
, [LocationLevel2Code] nvarchar(50)  NOT NULL
, [LocationLevel2Name] nvarchar(250)  NOT NULL
, [LocationLevel3Code] nvarchar(50)  NOT NULL
, [LocationLevel3Name] nvarchar(250)  NOT NULL
, [LocationLevel4Code] nvarchar(50)  NOT NULL
, [LocationLevel4Name] nvarchar(250)  NOT NULL
, [LocationInvStatus] nvarchar(50)  NOT NULL
, [LocationNodeType] nvarchar(50)  NOT NULL
, [LocationLevelNum] nvarchar(50)  NOT NULL
, [LocationTotal] nvarchar(50)  NOT NULL
, [DateModified] nvarchar(50)  NOT NULL
, [DateCreated] nvarchar(50)  NOT NULL
, [ItemStatus] nvarchar(4000)  NULL
, [ItemType] nvarchar(4000)  NULL
, [UnitTypeCode] nvarchar(4000)  NULL
, [CatalogItemCode] nvarchar(300)  NOT NULL
, [CatalogItemName] nvarchar(100)  NOT NULL
, [CatalogItemType] nvarchar(50)  NOT NULL
, [CatalogFamilyCode] nvarchar(50)  NOT NULL
, [CatalogFamilyName] nvarchar(50)  NOT NULL
, [CatalogSectionCode] nvarchar(50)  NOT NULL
, [CatalogSectionName] nvarchar(100)  NOT NULL
, [CatalogSubSectionCode] nvarchar(50)  NOT NULL
, [CatalogSubSectionName] nvarchar(100)  NOT NULL
, [CatalogPriceBuy] nvarchar(50)  NOT NULL
, [CatalogPriceSell] nvarchar(50)  NOT NULL
, [CatalogSupplierCode] nvarchar(50)  NOT NULL
, [CatalogSupplierName] nvarchar(50)  NOT NULL
, [CatalogUnitTypeCode] nvarchar(50)  NOT NULL
, [CatalogDescription] nvarchar(500)  NOT NULL
, [TemporaryOldUid] nvarchar(250)  NOT NULL
, [TemporaryNewUid] nvarchar(250)  NOT NULL
, [TemporaryOldSerialNumber] nvarchar(250)  NOT NULL
, [TemporaryOldItemCode] nvarchar(300)  NOT NULL
, [TemporaryOldLocationCode] nvarchar(250)  NOT NULL
, [TemporaryOldKey] nvarchar(300)  NOT NULL
, [TemporaryNewSerialNumber] nvarchar(250)  NOT NULL
, [TemporaryNewItemCode] nvarchar(300)  NOT NULL
, [TemporaryNewLocationCode] nvarchar(250)  NOT NULL
, [TemporaryNewKey] nvarchar(300)  NOT NULL
, [TemporaryDateModified] nvarchar(50)  NOT NULL
, [TemporaryOperation] nvarchar(50)  NOT NULL
, [TemporaryDevice] nvarchar(50)  NOT NULL
, [TemporaryDbFileName] nvarchar(300)  NOT NULL
, [IturCode] nvarchar(50)  NOT NULL
);	
";

			string sql3 = @"ALTER TABLE [CurrentInventoryAdvanced] ADD CONSTRAINT [PK_dbo.CurrentInventoryAdvanced] PRIMARY KEY ([Id]);";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB, analyticDBFile);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				var cmd2 = new SqlCeCommand(sql2, sqlCeConnection, tran);
				var cmd3 = new SqlCeCommand(sql3, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "CurrentInventoryAdvanced", connectionString));
				//Localization.Resources.Log_TraceRepository1001%"Start Process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "CurrentInventoryAdvanced"));
				//LogPrint();
				try
				{
					cmd.ExecuteNonQuery();
				}
				catch { }
				cmd2.ExecuteNonQuery();
				cmd3.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "CurrentInventoryAdvanced"));
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("DropCurrentInventoryAdvanced", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
		}

		#endregion
	}
}




