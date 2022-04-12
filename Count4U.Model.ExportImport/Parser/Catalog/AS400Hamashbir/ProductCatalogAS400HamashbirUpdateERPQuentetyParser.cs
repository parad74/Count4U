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
	//	import ERPQuentety by barcode
	public class ProductCatalogAS400HamashbirUpdateERPQuentetyParser :BaseProductCatalogParser, IProductSimpleParser
	{
		private readonly IProductRepository _productRepository;
		private readonly IImportCatalogADORepository _importCatalogSimpleRepository;

		public ProductCatalogAS400HamashbirUpdateERPQuentetyParser(IProductRepository productRepository,
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

			Dictionary<string, double> quantityERPDictionary = new Dictionary<string, double>();

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


			MaskPackage maskPackage = parms.GetMaskPackageFromParms();
			bool makatApplyMask = false;
			bool barcodeApplyMask = false;
			GetApplyMasks(importType, maskPackage, out makatApplyMask, out barcodeApplyMask);

			List<Point> startEndSubstring = new List<Point>();

	//CatalogMinLengthIncomingRow = 25,  
	//			CatalogItemCodeStart = 5, 
	//			CatalogItemCodeEnd = 12,
	//			HamarotBarcodeStart = 13,
	//			HamarotBarcodeEnd = 25,
	//			QuantityTypeStart = 26,
	//			QuantityTypeEnd = 26, 
	//			QuantityERPStart = 27,
	//			QuantityERPEnd = 31

			startEndSubstring.Add(new Point		//Field1: Item Code (col 1-13)
			{
				Start = catalogParserPoints.HamarotBarcodeStart,
				End = catalogParserPoints.HamarotBarcodeEnd,
				Length = catalogParserPoints.HamarotBarcodeEnd - catalogParserPoints.HamarotBarcodeStart + 1
			});
			startEndSubstring.Add(new Point		//Field5: Quantity Expected Sign(col 26)
			{
				Start = catalogParserPoints.QuantityTypeStart,
				End = catalogParserPoints.QuantityTypeEnd,
				Length = catalogParserPoints.QuantityTypeEnd - catalogParserPoints.QuantityTypeStart + 1
			});
			startEndSubstring.Add(new Point			//Field4: Quantity Expected (col 27-31)
			{
				Start = catalogParserPoints.QuantityERPStart,
				End = catalogParserPoints.QuantityERPEnd,
				Length = catalogParserPoints.QuantityERPEnd - catalogParserPoints.QuantityERPStart + 1
			});

			startEndSubstring.Add(new Point			//Field2: Item Code (col 5-12)
			{
				Start = catalogParserPoints.CatalogItemCodeStart,
				End = catalogParserPoints.CatalogItemCodeEnd,
				Length = catalogParserPoints.CatalogItemCodeEnd - catalogParserPoints.CatalogItemCodeStart + 1
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

				String[] record = { "", "", "0.0" , ""};


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
				string barcode = record[0].Trim();
				if (string.IsNullOrWhiteSpace(barcode) == true)
				{
					continue;
				}


				//newProductSimpleString.MakatOriginal = makat;

				if (barcodeApplyMask == true)
				{
					barcode = maskPackage.BarcodeMaskTemplate.FormatString(
					barcode, maskPackage.BarcodeMaskRecord.Value);
				}


				if (productMakatDBDictionary.ContainsKey(barcode) == false)
				{
					continue;
				}
				//===========Product=======================================
				//HamarotBarcodeStart = 13,			   0
				//HamarotBarcodeEnd = 25,
				 //QuantityTypeStart = 26,				  1
				//	QuantityTypeEnd = 26, 
				//QuantityERPStart = 26,				  2
				//QuantityERPEnd = 31
				//newProductSimpleString.Makat = makat;//.TrimStart('0');

				newProductSimpleString.BalanceQuantityERP = "0";

				string sign = record[1].Trim();

				string quantityERP = record[2].Trim();
				

				if (quantityERP == "000000" || quantityERP == "00000" || quantityERP == "0000" || quantityERP == "000" || quantityERP == "00" || quantityERP == "0")
				{
					quantityERP = "0";
				}
				else
				{
					quantityERP = quantityERP.TrimStart('0');
					if (sign == "-") quantityERP = "-" + quantityERP;
				}

				double quantity = 0.0;
				bool ret = Double.TryParse(quantityERP, out quantity);
				if (ret == true)
				{
					quantityERPDictionary[barcode] = quantity;
				}

			} //foreach


			IEnumerable<Product> productFromDBSimples = this._productRepository.GetProducts(dbPath);
			//this._productRepository.DeleteAll(dbPath);
			IImportCatalogADORepository provider = ServiceLocator.GetInstance<IImportCatalogADORepository>();
			provider.ClearProducts(dbPath);
			 string barcodeType = TypeMakatEnum.B.ToString();

			//IImportCatalogSimpleRepository
			 foreach (Product productSimple in productFromDBSimples)
			 {
				 if (productSimple.TypeCode == barcodeType)
				 {
					 double balanceQuantityERP = 0.0;		  //get from updateDictionary ProductSimple
					 //double quantityERP = 0.0;
					 bool exists = quantityERPDictionary.TryGetValue(productSimple.Makat, out balanceQuantityERP);	//update Dictionary
					 if (exists == true)	   //если в файле апдейта есть Макат текущего  ProductSimple, то мы его обновляем
					 {
						 productSimple.BalanceQuantityERP = balanceQuantityERP;
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
