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
	public class ProductFromDBUpdateERPQuentetyParser : BaseProductCatalogParser, IProductSimpleParser
	{
		private readonly IProductRepository _productRepository;
		private readonly IImportCatalogADORepository _importCatalogSimpleRepository;

		public ProductFromDBUpdateERPQuentetyParser(IProductRepository productRepository,
			ILog log, 
			IServiceLocator serviceLocator) 
			: base(serviceLocator, log)
		{
			if (productRepository == null) throw new ArgumentNullException("productRepository");
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
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("ExcelFileParser is null");

			long k = 0;
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			//источник, заполить из БД from
			 IIturAnalyzesRepository iturAnalyzesRepository =
				this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			Dictionary<string, ProductSimple> quantityERPDictionary = 
				iturAnalyzesRepository.GetProductSimpleDictionary(fromPathFile, true);   

			//CatalogParserPoints catalogParserPoints = parms.GetCatalogParserPointsFromParm();
			string toDBPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);	  //Текущая БД

			base.SupplierDictionary.Clear();
			base.ErrorBitList.Clear();
			
			IEnumerable<Product> productToDBSimples = this._productRepository.GetProducts(toDBPath) ;
			IImportCatalogADORepository provider = ServiceLocator.GetInstance<IImportCatalogADORepository>();
			provider.ClearProducts(toDBPath);

			//IImportCatalogSimpleRepository
			foreach (Product productToDBSimple in productToDBSimples)
			{
				ProductSimple ps = new ProductSimple();		  //get from updateDictionary ProductSimple
				//double quantityERP = 0.0;
				bool exists = quantityERPDictionary.TryGetValue(productToDBSimple.Makat, out ps);	//update Dictionary
				if (exists == true)	   //если в файле апдейта есть Макат текущего  ProductSimple, то мы его обновляем
				{
					productToDBSimple.BalanceQuantityERP = ps.BalanceQuantityERP;
					productToDBSimple.BalanceQuantityPartialERP = ps.BalanceQuantityPartialERP;
					//productToDBSimple.IsUpdateERP = true;
					productToDBSimple.PriceBuy = ps.PriceBuy;
					productToDBSimple.PriceSale = ps.PriceSale;
				}
				else	  //если нет - обнуляем количество, цену не трогаем
				{
					productToDBSimple.BalanceQuantityERP = 0.0;
					//productToDBSimple.IsUpdateERP = false;
				}
				yield return productToDBSimple;
			} //foreach

		}
	}
}
