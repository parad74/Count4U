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
	public class ProductCatalogAS400AprilUpdateERPQuentetyParser :BaseProductCatalogParser, IProductSimpleParser
	{
		private readonly IProductRepository _productRepository;
		private readonly IImportCatalogADORepository _importCatalogSimpleRepository;

		public ProductCatalogAS400AprilUpdateERPQuentetyParser(IProductRepository productRepository,
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
			//base.ProductParentMakatDictionary.Clear();

			string separator = " ";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			//Files type : Fixed ANSI 
			//Field1: Item Code (col 1-13)
			//Field2: Name (col 14-38)
			//Field3: UnitType (col 39-40) 
			//Field4: Quantity Expected (col 41-46)
			//Field5: Quantity Expected Sign(col 47)
			//Field6: SectionID (col 48-49)
			//Field7: PriceSell (col 50-56)
			//Field8: PriceBuy (col 57-63)
			//Field9: SupplierID (col 64-71)

			//CatalogMinLengthIncomingRow = 2,
			//CatalogItemCodeStart = 1,
			//CatalogItemCodeEnd = 13, 
			//CatalogItemNameStart = 14,
			//CatalogItemNameEnd = 38,
			//UnitTypeCodeStart = 39,
			//UnitTypeCodeEnd = 40, 
			//QuantityERPStart =41, 
			//QuantityERPEnd=46, 
			//QuantityTypeStart=47, 
			//QuantityTypeEnd=47, 
			//SectionCodeStart=48, 
			//SectionCodeEnd=49,
			//CatalogPriceSaleStart = 50,
			//CatalogPriceSaleEnd = 56, 
			//CatalogPriceBuyStart=57, 
			//CatalogPriceBuyEnd=63,
			//SupplierCodeStart = 64, 
			//SupplierCodeEnd = 71

			MaskPackage maskPackage = parms.GetMaskPackageFromParms();
			bool makatApplyMask = false;
			bool barcodeApplyMask = false;
			GetApplyMasks(importType, maskPackage, out makatApplyMask, out barcodeApplyMask);

			List<Point> startEndSubstring = new List<Point>();

			startEndSubstring.Add(new Point		//Field1: Item Code (col 1-13)
			{
				Start = catalogParserPoints.CatalogItemCodeStart,
				End = catalogParserPoints.CatalogItemCodeEnd,
				Length = catalogParserPoints.CatalogItemCodeEnd - catalogParserPoints.CatalogItemCodeStart + 1
			});
			startEndSubstring.Add(new Point		//Field2: Name (col 14-38)
			{
				Start = catalogParserPoints.CatalogItemNameStart,
				End = catalogParserPoints.CatalogItemNameEnd,
				Length = catalogParserPoints.CatalogItemNameEnd - catalogParserPoints.CatalogItemNameStart + 1
			});
			startEndSubstring.Add(new Point		//Field3: UnitType (col 39-40) 
			{
				Start = catalogParserPoints.UnitTypeCodeStart,
				End = catalogParserPoints.UnitTypeCodeEnd,
				Length = catalogParserPoints.UnitTypeCodeEnd - catalogParserPoints.UnitTypeCodeStart + 1
			});

			startEndSubstring.Add(new Point			//Field4: Quantity Expected (col 41-46)
			{
				Start = catalogParserPoints.QuantityERPStart,
				End = catalogParserPoints.QuantityERPEnd,
				Length = catalogParserPoints.QuantityERPEnd - catalogParserPoints.QuantityERPStart + 1
			});
			startEndSubstring.Add(new Point		//Field5: Quantity Expected Sign(col 47)
			{
				Start = catalogParserPoints.QuantityTypeStart,
				End = catalogParserPoints.QuantityTypeEnd,
				Length = catalogParserPoints.QuantityTypeEnd - catalogParserPoints.QuantityTypeStart + 1
			});
			//startEndSubstring.Add(new Point		//Field6: SectionID (col 48-49)
			//{
			//	Start = catalogParserPoints.SectionCodeStart,
			//	End = catalogParserPoints.SectionCodeEnd,
			//	Length = catalogParserPoints.SectionCodeEnd - catalogParserPoints.SectionCodeStart + 1
			//});
			//startEndSubstring.Add(new Point		//Field7: PriceSell (col 50-56)
			//{
			//	Start = catalogParserPoints.CatalogPriceSaleStart,
			//	End = catalogParserPoints.CatalogPriceSaleEnd,
			//	Length = catalogParserPoints.CatalogPriceSaleEnd - catalogParserPoints.CatalogPriceSaleStart + 1
			//});
			//startEndSubstring.Add(new Point		//Field8: PriceBuy (col 57-63)
			//{
			//	Start = catalogParserPoints.CatalogPriceBuyStart,
			//	End = catalogParserPoints.CatalogPriceBuyEnd,
			//	Length = catalogParserPoints.CatalogPriceBuyEnd - catalogParserPoints.CatalogPriceBuyStart + 1
			//});
			//startEndSubstring.Add(new Point		//Field9: SupplierID (col 64-71)
			//{
			//	Start = catalogParserPoints.SupplierCodeStart,
			//	End = catalogParserPoints.SupplierCodeEnd,
			//	Length = catalogParserPoints.SupplierCodeEnd - catalogParserPoints.SupplierCodeStart + 1
			//});
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

				String[] record = { "", "", "", "0", "" };


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
				//Field1: Item Code (col 1-13)						//0
				//Field2: Name (col 14-38)							//1
				//Field3: UnitType (col 39-40)					//2
				//Field4: Quantity Expected (col 41-46)	//3
				//Field5: Quantity Expected Sign(col 47)	//4
				//Field6: SectionID (col 48-49)					//5
				//Field7: PriceSell (col 50-56)					//6
				//Field8: PriceBuy (col 57-63)						//7
				//Field9: SupplierID (col 64-71)					//8
				newProductSimpleString.Makat = makat;//.TrimStart('0');

				newProductSimpleString.BalanceQuantityERP = "0";

				string sign = record[4].Trim();

				string quantityERP = record[3].Trim();

				if (quantityERP == "000000" || quantityERP == "00000" || quantityERP == "0000" || quantityERP == "000" || quantityERP == "00" || quantityERP == "0")
				{
					newProductSimpleString.BalanceQuantityERP = "0";
				}
				else
				{
					newProductSimpleString.BalanceQuantityERP = quantityERP.TrimStart('0');
					if (sign == "-") newProductSimpleString.BalanceQuantityERP = "-" + newProductSimpleString.BalanceQuantityERP;
				}



				//string priceSale = record[6];
				//newProductSimpleString.PriceSale = priceSale.FormatComa2();
				//string priceBuy = record[7];
				//newProductSimpleString.PriceBuy = priceBuy.FormatComa2();

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
					//productSimple.PriceBuy = ps.PriceBuy;
					//productSimple.PriceSale = ps.PriceSale;
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
