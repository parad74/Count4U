using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Count4U.Model;
using Microsoft.Practices.ServiceLocation;
using System.Threading;

namespace Count4U.Model.Count4U
{
	//2 шаг суммируем  ERPQuentety by barcode in ERPQuentety for Makat
	public class ProductCatalogAS400HamashbirUpdateERPQuentetyParser2 :BaseProductCatalogParser, IProductSimpleParser
	{
		private readonly IProductRepository _productRepository;
		private readonly IImportCatalogADORepository _importCatalogSimpleRepository;

		public ProductCatalogAS400HamashbirUpdateERPQuentetyParser2(IProductRepository productRepository,
			ILog log, 
			IServiceLocator serviceLocator) 
			: base(serviceLocator, log)
		{
			this._productRepository = productRepository;
			this.Dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this.Dtfi.ShortTimePattern = @"hh:mm:ss";
	
		}

	
		/// <summary>
		/// Получение списка Product
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Product> GetProducts(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, ProductMakat> productMakatDBDictionary,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			long k = 0;
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			CatalogParserPoints catalogParserPoints = parms.GetCatalogParserPointsFromParm();
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);

			base.SupplierDictionary.Clear();
			base.ErrorBitList.Clear();
			//base.ProductParentMakatDictionary.Clear();

			Dictionary<string, ProductSimple> quantityERPDictionary = this._productRepository.GetSumProductQuantityERPByMakatDictionary(dbPath);

			//IEnumerable<Product> productFromDBSimples = this._productRepository.GetProducts(dbPath);

			Products productMakatOnlyFromDB = this._productRepository.GetProductsMakatOnly(dbPath);

			//this._productRepository.DeleteAll(dbPath);
			IImportCatalogADORepository provider = ServiceLocator.GetInstance<IImportCatalogADORepository>();
			//provider.ClearProducts(dbPath);
			provider.ClearProductsMakatOnly(dbPath);
			 string makatType = TypeMakatEnum.M.ToString();

			//IImportCatalogSimpleRepository
			 foreach (Product productSimple in productMakatOnlyFromDB)
			 {
				 if (productSimple.TypeCode == makatType)
				 {
					// double balanceQuantityERP = 0.0;		  //get from updateDictionary ProductSimple
					 ProductSimple ps = new ProductSimple();
					 //double quantityERP = 0.0;
					 bool exists = quantityERPDictionary.TryGetValue(productSimple.Makat, out ps);	//update Dictionary
					 if (exists == true)	   //если в файле апдейта есть Макат текущего  ProductSimple, то мы его обновляем
					 {
						 productSimple.BalanceQuantityERP = ps.BalanceQuantityERP;
					 }
					 else	  //если нет - обнуляем количество, цену не трогаем
					 {
						 productSimple.BalanceQuantityERP = 0.0;
						 //productSimple.IsUpdateERP = false;
					 }
				 }
				 if (cancellationToken.IsCancellationRequested == true) break;
				 k++;
				 if (k % 100 == 0) countAction(k);
				 yield return productSimple;
			 } //foreach

			//return this._productRepository.GetProductSimples(dbPath);
			//List<App_Data.Product> products = this._productRepository.GetApp_DataProducts(fromPathFile);
			//this._productRepository.Copy(products, toPathDB);
		}

	
	}
}
