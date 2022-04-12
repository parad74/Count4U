using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Count4U.Model;
using System.Threading;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{

	// странный процесс через промежуточный баркод
	public class ProductCatalogAS400HoParser2_1 : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogAS400HoParser2_1(IServiceLocator serviceLocator,
			ILog log)
			: base(serviceLocator, log)
		{
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

			base.SupplierDictionary.Clear();
			base.ErrorBitList.Clear();

			string separator = ",";
			string separatorParms = parms.GetStringValueFromParm(ImportProviderParmEnum.Delimiter);
			if (string.IsNullOrWhiteSpace(separatorParms) == false) separator = separatorParms; 

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			MaskPackage maskPackage = parms.GetMaskPackageFromParms();
			bool makatApplyMask = false;
			bool barcodeApplyMask = false;
			GetApplyMasks(importType, maskPackage, out makatApplyMask, out barcodeApplyMask);
			CatalogParserPoints catalogParserPoints = parms.GetCatalogParserPointsFromParm();
			bool withQuantityERP = parms.GetBoolValueFromParm(ImportProviderParmEnum.WithQuantityERP);
			bool invertLetter = false;
			bool rt2lf = false;
			if (encoding == Encoding.GetEncoding(862) || encoding == Encoding.GetEncoding(1255))
			{
				invertLetter = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLetters);
				rt2lf = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWords);
			}

			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			IProductRepository productRepository =  base.ServiceLocator.GetInstance<IProductRepository>();
			Products products = productRepository.GetProductsBarcodeOnly(dbPath);
			Dictionary<string, Product> productBarcodeDictionary = products.Distinct().ToDictionary(x => x.Makat);

			foreach (String[] record in fileParser.GetRecords(fromPathFile,
					encoding,
					separators,
					countExcludeFirstString))
			{
		

				if (record == null) continue;

				if (record.Length < 2)
				{
						continue;
				}

				string Barcode1 = record[0].Trim();
				if (string.IsNullOrWhiteSpace(Barcode1) == true)
				{
					continue;
				}
				//	Field1: Item Code				  0	 (Makat or) Barcode1 - этот проход 	   Barcode1	 -> ParentMakat == makat
				//Field2: Barcode ecxtra		   1	barcodeExtra = barcode

				// Берем 	Field1: Barcode1	 и ищем 	его среди баркодов, 
				// если находим Product с таким кодом, берем его ParentMakat - что является Makat
				// который и будет родительским Makat для  		//Field2: Barcode ecxtra	
				ProductSimpleString newProductSimpleString = new ProductSimpleString();
				Product newProductSimple = new Product();
				string barcodeExtra = record[1].Trim();		 //barcodeExtra = barcode
				if (string.IsNullOrWhiteSpace(barcodeExtra) == true)
				{
					continue;
				}

				// Берем 	Field1: Barcode1	 и ищем 	его среди баркодов, (чтобы найти Makat)
				if (productBarcodeDictionary.ContainsKey(Barcode1) == false)
				{
					continue;
				}
				// 	makat = Barcode1.ParentMakat 
				//		makat	->	 Barcode1	 ->	   barcodeEcxtra
				// записываем в БД 	 makat		->	 barcodeEcxtra
				Product product = productBarcodeDictionary[Barcode1];
				// если находим Product с таким кодом, берем его ParentMakat - что является Makat
				string makat = product.ParentMakat;			   // Barcode1	 -> ParentMakat == makat

				// который и будет родительским Makat для  		//Field2: Barcode ecxtra	

				string barcode = barcodeExtra;							 //barcodeExtra = barcode

				//================ дальше все как всегда

				newProductSimpleString.MakatOriginal = barcode;
				if (barcodeApplyMask == true)
				{
					barcode = maskPackage.BarcodeMaskTemplate.FormatString(
					barcode, maskPackage.BarcodeMaskRecord.Value);
				}

				if (makatApplyMask == true)
				{
					makat = maskPackage.MakatMaskTemplate.FormatString(
						makat, maskPackage.MakatMaskRecord.Value);
				}

				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					continue;
				}

				if (makat.ToUpper() == barcode.ToUpper())
				{
					continue;
				}

				if (productMakatDBDictionary.ContainsKey(barcode) == true)
				{
					continue;
				}

				//===========Product=======================================
				//	Field1: Item Code				  0				   or Barcode1
				//Field2: Barcode							 1

				newProductSimpleString.Makat = barcode;
				newProductSimpleString.Name = "";
				newProductSimpleString.TypeCode = TypeMakatEnum.B.ToString();
				newProductSimpleString.ParentMakat = makat;
				//newProductSimpleString.PriceString = "0.0";
				//newProductSimpleString.PriceSale = "0.0";
				//newProductSimpleString.PriceBuy = "0.0";
				//string countInParentPack = record[1];
				//newProductSimpleString.CountInParentPack = countInParentPack;
				//newProductSimpleString.BalanceQuantityERP = "0.0";
				//newProductSimpleString.SupplierCode = "";
				//newProductSimpleString.SectionCode = "";
				//newProductSimpleString.UnitTypeCode = "";

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

				retBit = newProductSimple.ValidateWarning(newProductSimpleString, this.Dtfi);

				if (retBit != 0)
				{
					base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
				}

				productMakatDBDictionary[barcode] = null;			   //newProductSimple.Makat
				newProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
				newProductSimple.InputTypeCode = InputTypeCodeEnum.B.ToString();

				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

				yield return newProductSimple;


			} //foreach
		}

		

	
	}

	
}
