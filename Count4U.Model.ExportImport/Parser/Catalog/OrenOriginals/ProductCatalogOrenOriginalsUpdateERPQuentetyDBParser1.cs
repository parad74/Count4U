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
	// не используем
	// ?? вопрос использования. Надо все удалять и писать вновь - поэтому используем ProductCatalogMPLParser2
	public class ProductCatalogOrenOriginalsUpdateERPQuentetyDBParser1 :BaseProductCatalogParser, IProductSimpleParser
	{
		private readonly IProductRepository _productRepository;
		private readonly IImportCatalogADORepository _importCatalogSimpleRepository;

		public ProductCatalogOrenOriginalsUpdateERPQuentetyDBParser1(IProductRepository productRepository,
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

			CatalogParserPoints catalogParserPoints = parms.GetCatalogParserPointsFromParm();
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
	
			//==================================
			Dictionary<string, ProductSimple> productQuantityERPDictionary = this._productRepository.GetProductQuantityERPDictionary(dbPath); //суммируем QuantityERP снизу (barcode) по ParentMakat - у
			Products productMakatOnlyFromDB = this._productRepository.GetProductsMakatOnly(dbPath) ; //Все продукты толька с М , сохраняем в памяти
			this._productRepository.DeleteAllMakat(dbPath); //Удаляем из БД все продукты толька с М 
			//===========================================================
			foreach (Product product in productMakatOnlyFromDB)
			{
				ProductSimple ps = new ProductSimple();		  //get from updateDictionary ProductSimple
				bool exists = productQuantityERPDictionary.TryGetValue(product.Makat, out ps);	//update Dictionary

				if (exists == true)	   //если в файле апдейта есть Макат текущего  ProductSimple, то мы его обновляем
				{
					product.BalanceQuantityERP = ps.BalanceQuantityERP;  // меняем на сумму BalanceQuantityERP всех barcode
				}
				else	  //если нет - обнуляем количество, цену не трогаем
				{
					product.BalanceQuantityERP = 0.0;
				}
				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

				yield return product;
			} //foreach

		}

	
	}
}
