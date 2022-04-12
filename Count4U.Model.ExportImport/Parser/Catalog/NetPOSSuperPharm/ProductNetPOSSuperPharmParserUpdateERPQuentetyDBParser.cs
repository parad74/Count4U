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
	public class ProductNetPOSSuperPharmParserUpdateERPQuentetyDBParser :BaseProductCatalogParser, IProductSimpleParser
	{
		private readonly IProductRepository _productRepository;
		private readonly IImportCatalogADORepository _importCatalogSimpleRepository;

		public ProductNetPOSSuperPharmParserUpdateERPQuentetyDBParser(IProductRepository productRepository,
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

			Dictionary<string, Product> quantityERPDictionary = new Dictionary<string, Product>();

			CatalogParserPoints catalogParserPoints = parms.GetCatalogParserPointsFromParm();
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);

			base.SupplierDictionary.Clear();
			base.ErrorBitList.Clear();

			string separator = " ";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			MaskPackage maskPackage = parms.GetMaskPackageFromParms();
			bool makatApplyMask = false;
			bool barcodeApplyMask = false;
			GetApplyMasks(importType, maskPackage, out makatApplyMask, out barcodeApplyMask);

			List<Point> startEndSubstring = new List<Point>();

			startEndSubstring.Add(new Point				 //0
			{
				Start = catalogParserPoints.CatalogItemCodeStart,
				End = catalogParserPoints.CatalogItemCodeEnd,
				Length = catalogParserPoints.CatalogItemCodeEnd - catalogParserPoints.CatalogItemCodeStart + 1
			});	 
			startEndSubstring.Add(new Point				  //1
			{
				Start = catalogParserPoints.QuantityERPStart,
				End = catalogParserPoints.QuantityERPEnd,
				Length = catalogParserPoints.QuantityERPEnd - catalogParserPoints.QuantityERPStart + 1
			});
			startEndSubstring.Add(new Point					  //2
				{
				Start = catalogParserPoints.CatalogPriceSaleStart,
				End = catalogParserPoints.CatalogPriceSaleEnd,
				Length = catalogParserPoints.CatalogPriceSaleEnd - catalogParserPoints.CatalogPriceSaleStart + 1
			});
			startEndSubstring.Add(new Point						//3
			{
				Start = catalogParserPoints.CatalogPriceBuyStart,
				End = catalogParserPoints.CatalogPriceBuyEnd,
				Length = catalogParserPoints.CatalogPriceBuyEnd - catalogParserPoints.CatalogPriceBuyStart + 1
			});
			int count = startEndSubstring.Count;

			//fill dictionry
			foreach (String rec in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				if (string.IsNullOrWhiteSpace(rec) == true) { continue; }
				int lenRow = rec.Length;

				//if (len < 36)
				if (lenRow < catalogParserPoints.CatalogMinLengthIncomingRow)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedLengthString, rec));
					continue;
				}

				String[] record = { "", "0.0", "0.0", "0.0" };	  //makat quantityERP  priceSale  priceBuy
					  
				// 0			1			2				3					4					5					6		
				//makat	name		unitType   quantityERP    sectionCode 	 priceSale 	  priceBuy
				//0000000001001           MG  HYPOTENS 1 u000000000800010400001350

				for (int i = 0; i < count; i++)
				{
					if (startEndSubstring[i].End < lenRow)
					{
						record[i] = rec.Substring(startEndSubstring[i].Start, startEndSubstring[i].Length);
					}
					else //startEndSubstring[i].End >= lenRow
					{
						if (startEndSubstring[i].Start < lenRow)
						{
							record[i] = rec.Substring(startEndSubstring[i].Start, lenRow - startEndSubstring[i].Start);
						}
					}
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
				//Makat appears more than once on Catalog.dat file: 
				//Ignore the second Makat, 
				//write into the log file the line which contains the second appearance of the exists Makat
				// 0			1			2				3					4					5					6		
				//makat	name		unitType   quantityERP    sectionCode 	 priceSale 	  priceBuy
				newProductSimpleString.Makat = makat;
				//string name = record[1].ReverseDosHebrew(invertLetter, rt2lf);
				//newProductSimpleString.Name = name;
				//newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				//newProductSimpleString.ParentMakat = "";
				//newProductSimpleString.UnitTypeCode = record[2].Trim();
				newProductSimpleString.BalanceQuantityERP = record[1].FormatComa2();
				//newProductSimpleString.SectionCode = record[4].Trim();
				string priceSale = record[2];
				newProductSimpleString.PriceSale = priceSale.FormatComa2();
				string priceBuy = record[3];
				newProductSimpleString.PriceBuy = priceBuy.FormatComa2();
				//newProductSimpleString.PriceString = String.IsNullOrEmpty(priceBuy) ? "0" : priceBuy;

				//newProductSimpleString.SupplierCode = "";

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

				retBit = newProductSimple.ValidateUpdate(newProductSimpleString, this.Dtfi);
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
				//double quantityERP = 0.0;
				bool exists = quantityERPDictionary.TryGetValue(productSimple.Makat, out ps);	//update Dictionary
				if (exists == true)	   //если в файле апдейта есть Макат текущего  ProductSimple, то мы его обновляем
				{
					productSimple.BalanceQuantityERP = ps.BalanceQuantityERP;
					//productSimple.IsUpdateERP = true;
					productSimple.PriceBuy = ps.PriceBuy;
					productSimple.PriceSale = ps.PriceSale;
				}
				else	  //если нет - обнуляем количество, цену не трогаем
				{
					productSimple.BalanceQuantityERP = 0.0;
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
