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
	public class ProductCatalogForGeneralXLSXUpdateERPQuentetyDBParser2 :BaseProductCatalogParser, IProductSimpleParser
	{
		private readonly IProductRepository _productRepository;
		private readonly IImportCatalogADORepository _importCatalogSimpleRepository;

		public ProductCatalogForGeneralXLSXUpdateERPQuentetyDBParser2(IProductRepository productRepository,
				ILog log, 
			IServiceLocator serviceLocator) 	: base(serviceLocator, log)
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

			int sheetNumberXlsx = parms.GetIntValueFromParm(ImportProviderParmEnum.SheetNumberXlsx);					// start from 1
			if (sheetNumberXlsx == 0) sheetNumberXlsx = 1;

			string sheetNameXlsx = parms.GetStringValueFromParm(ImportProviderParmEnum.SheetNameXlsx);		

			long k = 0;
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			Dictionary<string, Product> quantityERPDictionary = new Dictionary<string, Product>();

			CatalogParserPoints catalogParserPoints = parms.GetCatalogParserPointsFromParm();
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);

			bool updatePriceBuy = parms.GetBoolValueFromParm(ImportProviderParmEnum.UpdatePriceBuy);
			bool updatePriceSell = parms.GetBoolValueFromParm(ImportProviderParmEnum.UpdatePriceSell);
			

			base.SupplierDictionary.Clear();
			base.ErrorBitList.Clear();

			string separator = ",";
			string separatorParms = parms.GetStringValueFromParm(ImportProviderParmEnum.Delimiter);
			if (string.IsNullOrWhiteSpace(separatorParms) == false) separator = separatorParms; 

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			//H            3498 03122012
//mlout.txt
   // 67397,     11, 
   // makat,  Quantity Expected,  

			MaskPackage maskPackage = parms.GetMaskPackageFromParms();
			bool makatApplyMask = false;
			bool barcodeApplyMask = false;
			GetApplyMasks(importType, maskPackage, out makatApplyMask, out barcodeApplyMask);

			//fill dictionry
			foreach (String[] record in fileParser.GetRecords(fromPathFile,
		encoding,
		separators,
		countExcludeFirstString))
			{
				// 67397,     11
				// makat,  Quantity Expected
				//		0,		1,							
				if (record == null) continue;

				if (record.Length < 2)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}
						ProductSimpleString newProductSimpleString = new ProductSimpleString();
					Product newProductSimple = new Product();
					string makat = record[0].Trim();
					if (string.IsNullOrWhiteSpace(makat) == true)
					{
						Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
						continue;
					}

					//newProductSimpleString.MakatOriginal = makat;

					if (makatApplyMask == true)
					{
						makat = maskPackage.MakatMaskTemplate.FormatString(
							makat, maskPackage.MakatMaskRecord.Value);
					}


					if (productMakatDBDictionary.ContainsKey(makat) == false)
					{
						base.Log.Add(MessageTypeEnum.WarningParser, String.Format(ParserFileErrorMessage.MakatNotExistInDB, record.JoinRecord(separator)));
						continue;
					}
					//===========Product=======================================
					newProductSimpleString.Makat = makat;
					newProductSimpleString.BalanceQuantityERP = record[1].Trim(' ');

				

					int retBit = newProductSimple.ValidateError(newProductSimpleString, this.Dtfi);
					if (retBit != 0)  //Error
					{
						base.ErrorBitList.Add(new BitAndRecord
						{
							Bit = retBit,
							Record = record.JoinRecord(separator),
							ErrorType = MessageTypeEnum.Error
						});
						continue;
					}

					////// Optional
					//Field3  PriceBuy						2		 Optional
					//Field4  PriceSale						3	 Optional
					newProductSimpleString.PriceBuy = "0";
					if (record.Length > 2)	//2,3...
					{
						string priceBuy = record[2].Trim(' ');
						newProductSimpleString.PriceBuy = priceBuy;
					}

					newProductSimpleString.PriceSale = "0";
					if (record.Length > 3) //3,4..
					{
						string priceSale = record[3].Trim(' ');
						newProductSimpleString.PriceSale = priceSale;
					}

					retBit = newProductSimple.ValidateUpdateQuantityERPAndPrice(newProductSimpleString, this.Dtfi);
					if (retBit != 0)
					{
						base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
					}

					//double quantity = 0.0;
					//bool ret = Double.TryParse(record[1], out quantity);
					quantityERPDictionary[makat] = newProductSimple;
				
			} //foreach

			
			IEnumerable<Product> productFromDBSimples = this._productRepository.GetProducts(dbPath) ;
			//this._productRepository.DeleteAll(dbPath);
			IImportCatalogADORepository provider = ServiceLocator.GetInstance<IImportCatalogADORepository>();
			provider.ClearProducts(dbPath);

			//IImportCatalogSimpleRepository
			foreach (Product productSimple in productFromDBSimples)
			{
				Product ps = new Product();		  //get from updateDictionary ProductSimple
				Product psPartial = new Product();		  //get from updateDictionary ProductSimple
				bool exists = quantityERPDictionary.TryGetValue(productSimple.Makat, out ps);	//update Dictionary

				if (exists == true)	   //если в файле апдейта есть Макат текущего  ProductSimple, то мы его обновляем
				{
					productSimple.BalanceQuantityERP = ps.BalanceQuantityERP;
					if (updatePriceBuy == true)
					{
						productSimple.PriceBuy = ps.PriceBuy;
					}
					if (updatePriceSell == true)
					{
						productSimple.PriceSale = ps.PriceSale;
					}
					//productSimple.IsUpdateERP = true;
				}
				else	  //если нет - обнуляем количество, цену не трогаем
				{
					productSimple.BalanceQuantityERP = 0.0;
					if (updatePriceBuy == true)
					{
						productSimple.PriceBuy = 0.0;
					}
					if (updatePriceSell == true)
					{
						productSimple.PriceSale = 0.0;
					}
					//productSimple.IsUpdateERP = false;
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
