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
	public class ProductRetalixNextParser : BaseProductCatalogParser, IProductParser
	{

		public ProductRetalixNextParser(IServiceLocator serviceLocator,
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
	
			string separator = " ";
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

			//  Catalog files: TOINFXXX.TXT
			//Files type : Fixed ANSI 

			//TOINFXXX.TXT
//        Field1: Item Code (col 1-13)
//Field2: Name (col 14-38)
//Field3: UnitType (col 39-40) 
//Field4: Quantity Expected {ERP} (col 41-48)


//Field5: SectionID (col 49-50)
//Field6: PriceSell (col 51-57)

//Field7: PriceBuy (col 58-64)
			List<Point> startEndSubstring = new List<Point>();

			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogItemCodeStart,
				End = catalogParserPoints.CatalogItemCodeEnd,
				Length = catalogParserPoints.CatalogItemCodeEnd - catalogParserPoints.CatalogItemCodeStart + 1
			});	 
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogItemNameStart,
				End = catalogParserPoints.CatalogItemNameEnd,
				Length = catalogParserPoints.CatalogItemNameEnd - catalogParserPoints.CatalogItemNameStart	+ 1
			});	
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.UnitTypeCodeStart,
				End = catalogParserPoints.UnitTypeCodeEnd,
				Length = catalogParserPoints.UnitTypeCodeEnd - catalogParserPoints.UnitTypeCodeStart + 1
			});
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.QuantityERPStart,
				End = catalogParserPoints.QuantityERPEnd,
				Length = catalogParserPoints.QuantityERPEnd - catalogParserPoints.QuantityERPStart + 1
			});
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.SectionCodeStart,
				End = catalogParserPoints.SectionCodeEnd,
				Length = catalogParserPoints.SectionCodeEnd - catalogParserPoints.SectionCodeStart + 1
			});
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogPriceSaleStart,
				End = catalogParserPoints.CatalogPriceSaleEnd,
				Length = catalogParserPoints.CatalogPriceSaleEnd - catalogParserPoints.CatalogPriceSaleStart + 1
			});
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogPriceBuyStart,
				End = catalogParserPoints.CatalogPriceBuyEnd,
				Length = catalogParserPoints.CatalogPriceBuyEnd - catalogParserPoints.CatalogPriceBuyStart + 1
			});
			int count = startEndSubstring.Count;

			
			foreach (String rec in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				if (string.IsNullOrWhiteSpace(rec) == true) { continue; }
				int lenRow = rec.Length;
	
				
				if (lenRow < catalogParserPoints.CatalogMinLengthIncomingRow)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedLengthString, rec));
					continue;
				}



				String[] record = { "", "", "", "0.0", "", "0.0", "0.0" };
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
				//string recReverse = rec.ReverseDosHebrew(invertLetter, rt2lf); 

				ProductSimpleString newProductSimpleString = new ProductSimpleString();
				Product newProductSimple = new Product();
				//BaseProduct newProduct = null;
				//BaseProduct newBaseProduct = (BaseProduct)Activator.CreateInstance(typeof(T));
				//if (typeof(T) == typeof(Product))
				//{
				//    newProduct = (Product)newBaseProduct;
				//}
				//else if (typeof(T) == typeof(ProductSimple))
				//{
				//    newProduct = (ProductSimple)newBaseProduct;
				//}

			
				string makat = record[0].Trim();
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
					continue;
				}

				newProductSimpleString.MakatOriginal = makat;

				if (makatApplyMask == true)
				{
					makat = maskPackage.MakatMaskTemplate.FormatString(
						makat, maskPackage.MakatMaskRecord.Value);
				}

				if (productMakatDBDictionary.ContainsKey(makat) == true)
				{
					base.Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatExistInDB, record.JoinRecord(separator)));
					continue;
				}
				//===========Product=======================================
				//Makat appears more than once on Catalog.dat file: 
				//Ignore the second Makat, 
				//write into the log file the line which contains the second appearance of the exists Makat
				// 0			1			2				3					4					5					6		
				//makat	name		unitType   quantityERP    sectionCode 	 priceSale 	  priceBuy
				newProductSimpleString.Makat = makat;
				string name = record[1].ReverseDosHebrew(invertLetter, rt2lf); 
 				newProductSimpleString.Name = name;
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				newProductSimpleString.UnitTypeCode = record[2].Trim();
				newProductSimpleString.BalanceQuantityERP = "0";
				if (withQuantityERP == true)
				{
					newProductSimpleString.BalanceQuantityERP = record[3].FormatComa2();
				}
					newProductSimpleString.SectionCode = record[4].Trim().LeadingZero3();
					string priceSale = record[5];
					newProductSimpleString.PriceSale = priceSale.FormatComa2();
					string priceBuy = record[6];
					newProductSimpleString.PriceBuy = priceBuy.FormatComa2();
					newProductSimpleString.PriceString = String.IsNullOrEmpty(priceBuy) ? "0" : priceBuy;
				

				newProductSimpleString.SupplierCode = "";
				int retBit = newProductSimple.ValidateError(newProductSimpleString,base.Dtfi); 
			
				//retBit = newProduct.ValidateError(newProductSimpleString);
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

				retBit = newProductSimple.ValidateWarning(newProductSimpleString, base.Dtfi);
  				if (retBit != 0)
				{
					base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
				}

				productMakatDBDictionary[makat] = null;  //newProductSimple.Makat,
				newProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
				newProductSimple.InputTypeCode = InputTypeCodeEnum.B.ToString();

				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

				yield return newProductSimple;
				//if (newProduct is T)
				//{
				//    yield return (T)newProduct;
				//}
			} //foreach
		}

		

	
	}

	
}
