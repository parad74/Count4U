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
using Count4U.Model.Count4U.Validate;
using System.IO;

namespace Count4U.Model.Count4U
{
	public class ImportCatalogBulkRepository : BaseCopyBulkRepository, IImportCatalogBlukRepository
	{
		//private readonly ICatalogConfigRepository _catalogConfigRepository;
		//private ISupplierRepository _supplierRepository;
		private readonly IMakatRepository _makatRepository;
		private readonly IIturRepository _iturRepository;
		public readonly List<IInventProductSimpleParser> _inventProductParserList;
		private IProductSimpleParser _productParser;
		private IInventProductSimpleParser _inventProductAdvancedParser;
		private Dictionary<string, ProductMakat> _makatDictionary;
		private IProductRepository _productRepository;

		public ImportCatalogBulkRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IProductRepository productRepository,
			IMakatRepository makatRepository)
			: base(connection, dbSettings, log, serviceLocator)
		{
			if (makatRepository == null) throw new ArgumentNullException("makatRepository");

			this._makatRepository = makatRepository;
			this._makatDictionary = new Dictionary<string, ProductMakat>();
			this._productRepository = productRepository;
		}

		private Dictionary<string, ProductMakat> GetProductMakatDictionary(string pathDB,
		bool refill = false)
		{
			if (refill == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill {0}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "ProductMakatDictionary"));
				Dictionary<string, ProductMakat> productMakatDictionary =
					this._makatRepository.GetProductBarcodeDictionary(pathDB, true);
				//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill {0}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "ProductMakatDictionary"));
				return productMakatDictionary;
			}
			else
			{
				return this._makatRepository.GetProductBarcodeDictionary(pathDB, false);
			}
		}



		public void InsertProduct(string fromPathFile, string pathDB,
			ProductSimpleParserEnum productSimpleParserEnum,
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
			ProductCatalogMerkavaXslx2SdfParser parcer = new ProductCatalogMerkavaXslx2SdfParser(_serviceLocator, Log);
			this._productParser =
						this._serviceLocator.GetInstance<IProductSimpleParser>(productSimpleParserEnum.ToString());
			if (this._productParser == null)
			{
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, this._productParser.ToString()));
				return;
			}

			if (File.Exists(fromPathFile) == false)
			{
				this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
				return;
			}
			//this._inventProductAdvancedParser   =  
			//    this._serviceLocator.GetInstance<IInventProductSimpleParser>(inventProductAdvancedParser.ToString());
			Dictionary<string, ProductMakat> productMakatDictionary = new Dictionary<string, ProductMakat>();

			if (importType.Contains(ImportDomainEnum.DontRefillProductMakatDictionary) == false)
			{
				productMakatDictionary = this.GetProductMakatDictionary(pathDB, true);
			}


			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, " ImportPDA Repository", "[ImportCatalogBulkRepository]"));
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "InventProductParser", productSimpleParserEnum.ToString()));

			int i = 0;

			string sessionCodeIn = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			//========
			#region Product
			string tableName = @"Product";
			if (cancellationToken.IsCancellationRequested == true) return;
			countAction(0);
			//Localization.Resources.Log_TraceRepositoryResult1061%"Source Table [{0}] in DB [{1}] {2} have [{3}] Row"
			//this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1061,
			//    tableName, fromPathDB, Environment.NewLine, count));

			if (importType.Contains(ImportDomainEnum.ImportCatalog) == true)
			{

				IEnumerable<Product> dataProduct = this._productParser.GetProducts(fromPathFile,
						encoding, separators, countExcludeFirstString, productMakatDictionary, importType, parms).ToList();

				//CopyBulkIEnumerable<T>(SqlCeBulkCopy bulkCopy, IEnumerable<T> data)
				DoBulkCopyMapping<Product>(tableName, dataProduct, true, pathDB, columnMappings);

				//string queryString = "SELECT  Product.*  FROM  Product";
				//this.CopyTable(tableName, fromPathDB, toPathDB);
			}
			long count = base.CountRow(tableName, pathDB);
			countAction(count);
			//Localization.Resources.Log_TraceRepositoryResult1062%"Destination Table [{0}] in DB [{1}] {2} after Copy, have [{3}] Row"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1062,
				tableName, pathDB, Environment.NewLine, count));
			
			this._productRepository.SetLastUpdatedCatalog(pathDB);

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			this.Log.Add(MessageTypeEnum.TraceRepository, "");
			this.FillLogFromErrorBitList(this._productParser.ErrorBitList);

			//private static IEnumerable<SqlBulkCopyColumnMapping> GetColumnMappings()
			// {
			//    yield return new SqlBulkCopyColumnMapping("Id", "AlbumId");
			//    yield return new SqlBulkCopyColumnMapping("GenreId", "GenreId");
			//    yield return new SqlBulkCopyColumnMapping("ArtistId", "ArtistId");
			//    yield return new SqlBulkCopyColumnMapping("AlbumTitle", "Title");
			//    yield return new SqlBulkCopyColumnMapping("Price", "Price");
			//    yield return new SqlBulkCopyColumnMapping("AlbumArtImage", "AlbumArtUrl");
			//}
			#endregion Product
		}

		public void ClearProduct(string pathDB)
		{
			DeleteTable(@"Product", pathDB);
			this._productRepository.SetLastUpdatedCatalog(pathDB);
		}


		public void FillLogFromErrorBitList(List<BitAndRecord> errorBitList)
		{
			if (errorBitList == null) return;
			if (errorBitList.Count == 0) return;
			//Log_TraceParser1001%"Parser Error And Message : "
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
						this.Log.Add(MessageTypeEnum.Error, //bitAndRecord.ErrorType.ToString() + " : " +
							 ProductValidate.ConvertDataErrorCode2ErrorMessage(b) + " [ " + record + " ] ");
					}
				}

				if (errorType == MessageTypeEnum.WarningParser)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.WarningParser, //bitAndRecord.ErrorType.ToString() + " : "  +
							 ProductValidate.ConvertDataErrorCode2WarningMessage(b) + " [ " + record + " ] ");
					}
				}
			}
		}

	}
}




