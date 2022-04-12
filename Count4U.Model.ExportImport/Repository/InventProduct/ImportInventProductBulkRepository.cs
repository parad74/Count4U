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
using System.IO;

namespace Count4U.Model.Count4U
{
	public class ImportInventProductBulkRepository : BaseCopyBulkRepository, IImportInventProductBlukRepository
	{
		//private readonly ICatalogConfigRepository _catalogConfigRepository;
		//private ISupplierRepository _supplierRepository;
		private readonly IMakatRepository _makatRepository;
		private readonly IIturRepository _iturRepository;
		public readonly List<IInventProductSimpleParser> _inventProductParserList;
		private IInventProductSimpleParser _inventProductParser;
		private IInventProductSimpleParser _inventProductAdvancedParser;

		public ImportInventProductBulkRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IMakatRepository makatRepository,
			IIturRepository iturRepository)
			: base(connection,dbSettings, log, serviceLocator)
		{
			if (makatRepository == null) throw new ArgumentNullException("makatRepository");
			if (iturRepository == null) throw new ArgumentNullException("iturRepository");

			this._makatRepository = makatRepository;
			this._iturRepository = iturRepository;
		}

		private Dictionary<string, Itur> GetIturDictionary(string pathDB, bool refill = false)
		{
			if (refill == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "IturDictionary"));
				Dictionary<string, Itur> iturFromDBDictionary =
					this._iturRepository.GetIturDictionary(pathDB, true);
				//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "IturDictionary"));
				return iturFromDBDictionary;
			}
			else
			{
				return this._iturRepository.GetIturDictionary(pathDB, false);
			}
		}

		public void InsertInventProducts(string fromPathFile, string pathDB,
			InventProductSimpleParserEnum inventProductSimpleParserEnum,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null, 
			List<string[]> columnMappings = null)
		{

			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1059%"Cancel Insert [{0}] in DB [{1}] from file [{2}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1059, "[InventProduct]", pathDB, fromPathFile));
				return;
			}
			this._inventProductParser =
						this._serviceLocator.GetInstance<IInventProductSimpleParser>(inventProductSimpleParserEnum.ToString());
			if (this._inventProductParser == null)
			{
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, inventProductSimpleParserEnum.ToString()));
				return;
			}
			//if (File.Exists(fromPathFile) == false)
			//{
			//	this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
			//	return;
			//}

			//this._inventProductAdvancedParser   =  
			//    this._serviceLocator.GetInstance<IInventProductSimpleParser>(inventProductAdvancedParser.ToString());

			Dictionary<string, Itur> iturFromDBDictionary = new Dictionary<string, Itur>();
			if (importType.Contains(ImportDomainEnum.ExistItur) == true)
			{
				iturFromDBDictionary = this.GetIturDictionary(pathDB, true);
			}
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, " ImportPDA Repository", "[ImportInventProductBulkRepository]"));
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "InventProductParser", inventProductSimpleParserEnum.ToString()));

			int i = 0;

			Dictionary<string, ProductMakat> productMakatDictionary = new Dictionary<string, ProductMakat>();
			if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
			{
				bool refillMalat = false;
				if (importType.Contains(ImportDomainEnum.RefillMakatDictionary) == true) refillMalat = true;

				productMakatDictionary = this._makatRepository.ProductMakatDictionaryRefill(pathDB, refillMalat);
			}

			string sessionCodeIn = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			//========
			#region InventProduct
			string tableName = @"InventProduct";
			if (cancellationToken.IsCancellationRequested == true) return;
			countAction(0);
	
			if (importType.Contains(ImportDomainEnum.ImportInventProduct) == true)
			{

				IEnumerable<InventProduct> dataInventProduct = this._inventProductParser.GetInventProducts(
			   fromPathFile, encoding, separators,
			   countExcludeFirstString, sessionCodeIn,
				productMakatDictionary, //productBarcodeDictionary, 
				iturFromDBDictionary,
				importType,
				parms) ;

				DoBulkCopyMapping<InventProduct>(tableName, dataInventProduct, true, pathDB, columnMappings);

			}
			long count = base.CountRow(tableName, pathDB);
			countAction(count);
			//Localization.Resources.Log_TraceRepositoryResult1062%"Destination Table [{0}] in DB [{1}] {2} after Copy, have [{3}] Row"
			this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1062,
				tableName, pathDB, Environment.NewLine, count));

			this._inventProductParser = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			#endregion InventProduct
		}





		//private static IEnumerable<SqlBulkCopyColumnMapping> GetColumnMappings()
		// {
		//    yield return new SqlBulkCopyColumnMapping("Id", "AlbumId");
		//    yield return new SqlBulkCopyColumnMapping("GenreId", "GenreId");
		//    yield return new SqlBulkCopyColumnMapping("ArtistId", "ArtistId");
		//    yield return new SqlBulkCopyColumnMapping("AlbumTitle", "Title");
		//    yield return new SqlBulkCopyColumnMapping("Price", "Price");
		//    yield return new SqlBulkCopyColumnMapping("AlbumArtImage", "AlbumArtUrl");
		//}

		public void ClearInventProducts(string pathDB)
		{
			DeleteTable(@"InventProduct", pathDB);
		}

		public void ClearDocumentHeaders(string pathDB)
		{
			DeleteTable(@"DocumentHeader", pathDB);
		}

		public void ClearSession(string pathDB)
		{
			DeleteTable(@"Session", pathDB);
		}

		public void ClearItur(string pathDB)
		{
			DeleteTable(@"Itur", pathDB);
		}

	}
}


