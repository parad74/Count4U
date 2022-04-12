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
	public class ImportCatalogSimpleADORepository : BaseImportADORepository, IImportCatalogADORepository
	{
		private readonly ICatalogConfigRepository _catalogConfigRepository;
		private readonly IMakatRepository _makatRepository;
		private Dictionary<string, ProductMakat> _makatDictionary;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private IProductSimpleParser _productParser;
		private ISupplierRepository _supplierRepository;
		private IProductRepository _productRepository;
	

		public ImportCatalogSimpleADORepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			ICatalogConfigRepository catalogConfigRepository,
			IMakatRepository makatRepository,
			IProductRepository productRepository,
			ISupplierRepository supplierRepository)
			: base(connection, dbSettings, log, serviceLocator)
        {
			if (catalogConfigRepository == null) throw new ArgumentNullException("catalogConfigRepository");
			if (makatRepository == null) throw new ArgumentNullException("makatRepository");
			if (supplierRepository == null) throw new ArgumentNullException("supplierRepository");
			
			this._makatDictionary = new Dictionary<string, ProductMakat>();
			this._makatRepository = makatRepository;
			this._catalogConfigRepository = catalogConfigRepository;
			this._supplierRepository = supplierRepository;
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

		private Dictionary<string, Supplier> GetSupplierDictionary(string pathDB)
		{
			//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill {0}"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "SupplierDictionary"));
				Dictionary<string, Supplier> supplierMakatDictionary =
					this._supplierRepository.GetSupplierDictionary(pathDB, true);
				//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill  {0}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "SupplierDictionary"));
				return supplierMakatDictionary;
		}

		public void InsertProducts(string fromPathFile, string pathDB,
			ProductSimpleParserEnum productParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._productParser = this._serviceLocator.GetInstance<IProductSimpleParser>(productParserEnum.ToString());

			if (this._productParser == null)
			{
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, productParserEnum.ToString()));
				return;
			}

			if (File.Exists(fromPathFile) == false)
			{
				this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
				return;
			}

			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			Dictionary<string, ProductMakat> productMakatDictionary =
				this.GetProductMakatDictionary(pathDB, true);

			//Localization.Resources.Log_TraceRepository1040%"[{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "ImportCatalogRepository", "ImportCatalogSimpleADORepository"));
			//Localization.Resources.Log_TraceRepository1040%"[{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "ImportCatalogParser", productParserEnum.ToString()));

			string sql1 = "INSERT INTO [Product] ([Makat],[ParentMakat],[Name],[MakatOriginal],[Family],[FamilyCode]"
				+ ", [PriceString], [PriceSale], [PriceBuy], [SupplierCode], [TypeCode], [FromCatalogType],[InputTypeCode]"
				+ ", [SectionCode],[UnitTypeCode],[BalanceQuantityERP], [IsUpdateERP],[CountInParentPack], [BalanceQuantityPartialERP]) "
				+ "VALUES (@Makat,@ParentMakat, @Name, @MakatOriginal, @Family, @FamilyCode"
				+ ", @PriceString, @PriceSale, @PriceBuy, @SupplierCode, @TypeCode, @FromCatalogType, @InputTypeCode"
				+ ", @SectionCode, @UnitTypeCode, @BalanceQuantityERP, @IsUpdateERP, @CountInParentPack, @BalanceQuantityPartialERP ) ";
			SqlCeTransaction tran = null;

			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();


				int k = 0;
				//countAction(k);

				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

				cmd.Parameters.Add(new SqlCeParameter(
					"@ParentMakat", SqlDbType.NVarChar, 300));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Makat", SqlDbType.NVarChar, 300));
				cmd.Parameters.Add(new SqlCeParameter(
					"@MakatOriginal", SqlDbType.NVarChar, 300));
				cmd.Parameters.Add(new SqlCeParameter(
					"@Family", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
				"@FamilyCode", SqlDbType.NVarChar, 100));
  				cmd.Parameters.Add(new SqlCeParameter(
					"@Name", SqlDbType.NVarChar, 100));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceString", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceSale", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@PriceBuy", SqlDbType.Float));
				//cmd.Parameters.Add(new SqlCeParameter(
				//    "@ParserBag", SqlDbType.NVarChar, 500));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SupplierCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@TypeCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@FromCatalogType", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@InputTypeCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@SectionCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@UnitTypeCode", SqlDbType.NVarChar, 50));
				cmd.Parameters.Add(new SqlCeParameter(
					"@BalanceQuantityERP", SqlDbType.Float));
				cmd.Parameters.Add(new SqlCeParameter(
					"@IsUpdateERP", SqlDbType.Bit));
				cmd.Parameters.Add(new SqlCeParameter(
					"@CountInParentPack", SqlDbType.Int));
				cmd.Parameters.Add(new SqlCeParameter(
					"@BalanceQuantityPartialERP", SqlDbType.Int));
				
	
				
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Product", pathDB));

				if (importType.Contains(ImportDomainEnum.ImportCatalog) == true)
				{
				//	foreach (ProductSimple product in this._productParser.GetProducts(fromPathFile,
					foreach (Product product in this._productParser.GetProducts(fromPathFile,
						encoding, separators, countExcludeFirstString, productMakatDictionary, importType, parms))
					{

						if (cancellationToken.IsCancellationRequested == true)
						{
							break;
						}
						k++;
						//if (k % 100 == 0)
						//{
						//    countAction(k);
						//    //tran.Commit();
						//    //tran = sqlCeConnection.BeginTransaction();
						//}

						cmd.Parameters["@ParentMakat"].Value = product.ParentMakat;
						cmd.Parameters["@Makat"].Value = product.Makat;
						cmd.Parameters["@MakatOriginal"].Value = product.MakatOriginal;
						cmd.Parameters["@Family"].Value = product.Family;
						cmd.Parameters["@FamilyCode"].Value = product.FamilyCode;
						cmd.Parameters["@Name"].Value = product.Name;
						cmd.Parameters["@PriceString"].Value = product.PriceString;
						cmd.Parameters["@PriceSale"].Value = product.PriceSale;
						cmd.Parameters["@PriceBuy"].Value = product.PriceBuy;
						cmd.Parameters["@SupplierCode"].Value = product.SupplierCode;
						cmd.Parameters["@TypeCode"].Value = product.TypeCode;
						cmd.Parameters["@FromCatalogType"].Value = product.FromCatalogType;
						cmd.Parameters["@InputTypeCode"].Value = product.InputTypeCode;
						cmd.Parameters["@SectionCode"].Value = product.SectionCode;
						cmd.Parameters["@UnitTypeCode"].Value = product.UnitTypeCode;
						cmd.Parameters["@BalanceQuantityERP"].Value = product.BalanceQuantityERP;
						cmd.Parameters["@CountInParentPack"].Value = product.CountInParentPack == 0 ? 1 : product.CountInParentPack;
						cmd.Parameters["@BalanceQuantityPartialERP"].Value = product.BalanceQuantityPartialERP;
						cmd.Parameters["@IsUpdateERP"].Value = product.IsUpdateERP;
						
						//cmd.Parameters["@ParentMakat"].Value = "";
						//cmd.Parameters["@Makat"].Value = "";
						//cmd.Parameters["@MakatOriginal"].Value = "";
						//cmd.Parameters["@Name"].Value = "";
						//cmd.Parameters["@PriceString"].Value = "";
						//cmd.Parameters["@PriceSale"].Value = 0;
						//cmd.Parameters["@PriceBuy"].Value = 0;
						//cmd.Parameters["@SupplierCode"].Value = "";
						//cmd.Parameters["@TypeCode"].Value = "";

						cmd.ExecuteNonQuery();

					}
					//textBox5.Text = k.ToString();
					countAction(k);
				}
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "Product", pathDB));

		
				//try
				//{
				//    CatalogConfig catalogConfig = new CatalogConfig
				//    {
				//        Description = fromPathFile,
				//        CreateDate = DateTime.Now,
				//        ModifyDate = DateTime.Now,
				//        Tag = k.ToString()
				//    };
				//    this._catalogConfigRepository.Insert(catalogConfig, pathDB);
				//}
				//catch { }

				if (importType.Contains(ImportDomainEnum.ImportSupplier) == true)
				{
					this._supplierRepository.Insert(this._productParser.SupplierDictionary, pathDB);
					//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, this._productParser.SupplierDictionary.Count.ToString(), "Supplier", pathDB));
				}

				//Localization.Resources.Log_TraceRepositoryResult1035%"Start process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1035, "ImportCatalogSimpleADORepository"));
				tran.Commit();
				//Localization.Resources.Log_TraceRepositoryResult1036%"End process: Commit Data To DB [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1036, "ImportCatalogSimpleADORepository"));
			}
			catch (Exception error)
			{
				_logger.ErrorException("InserProducts", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);

				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}

			this._productRepository.SetLastUpdatedCatalog(pathDB);

			this.Log.Add(MessageTypeEnum.TraceRepository, "");
			this.FillLogFromErrorBitList(this._productParser.ErrorBitList);
		}

		public void ClearProducts(string pathDB)
		{
			string sql1 = "DELETE FROM  [Product]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "Product", pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Product"));
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Product"));
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearProducts", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
			this._productRepository.SetLastUpdatedCatalog(pathDB);
		}

		public void ClearSupplier(string pathDB)
		{
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
				//Localization.Resources.Log_TraceRepository1001%"Start process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "Supplier"));
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "Supplier"));
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("ClearSupplier", error);
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

		#region IImportCatalogADORepository Members


		public void ClearProductsMakatOnly(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
