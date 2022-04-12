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
	public class ProductCatalogRetalixNextParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogRetalixNextParser(IServiceLocator serviceLocator,
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

			base.SupplierDictionary.Clear();
			base.ErrorBitList.Clear();

			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

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

	
			/* Field1: Item Code (col 1-13)
			Field2: Name (col 14-29)
			Field3: Unit (col 30) 
			Field4: Quantity Expected (col 31-40)
			 Field5: PriceBuy (col 41-49)
			 Field6: PriceSell (col 50-58)
			 Field7: SectionID (col 59-60)*/

			/*Field1 goes into Makat
			Field2 goes into Name
			Field3 do not import 
			Field4 goes into BalanceQuantityERP 
			Field5 goes into PriceBuy (Type- 7.2 without the dot)
			Field6 goes into PriceBuy (Type- 7.2 without the dot)
			Field7 goes into SectionID*/



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
				Length = catalogParserPoints.CatalogItemNameEnd - catalogParserPoints.CatalogItemNameStart + 1
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
				Start = catalogParserPoints.CatalogPriceBuyStart,
				End = catalogParserPoints.CatalogPriceBuyEnd,
				Length = catalogParserPoints.CatalogPriceBuyEnd - catalogParserPoints.CatalogPriceBuyStart + 1
			});
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogPriceSaleStart,
				End = catalogParserPoints.CatalogPriceSaleEnd,
				Length = catalogParserPoints.CatalogPriceSaleEnd - catalogParserPoints.CatalogPriceSaleStart + 1
			});
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.SectionCodeStart,
				End = catalogParserPoints.SectionCodeEnd,
				Length = catalogParserPoints.SectionCodeEnd - catalogParserPoints.SectionCodeStart + 1
			});
			int count = startEndSubstring.Count;
			long k = 0;

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


				String[] record = { "", "", "", "0.0", "0.0", "0.0", "" };
				// 0			1			2				3					4					5					6		
				//makat	name		unitType   quantityERP   priceBuy  	 priceSale 	  	  sectionCode
				//            9     ‡‘” љ‰‰ђЋ‡Њ0-     2913        0      39912

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
				// 0			1			2				3					4					5					6		
				//makat	name		unitType   quantityERP   priceBuy  	 priceSale 	  	  sectionCode
				newProductSimpleString.Makat = makat;
				string name = record[1].ReverseDosHebrew(invertLetter, rt2lf);
				newProductSimpleString.Name = name;
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				//newProductSimpleString.UnitTypeCode = record[2].Trim();
				newProductSimpleString.BalanceQuantityERP = "0";

				if (withQuantityERP == true)
				{
					newProductSimpleString.BalanceQuantityERP = record[3].FormatComa2();
				}
				string priceBuy = record[4];
				newProductSimpleString.PriceBuy = priceBuy.FormatComa2();
				string priceSale = record[5];
				newProductSimpleString.PriceSale = priceSale.FormatComa2();
				newProductSimpleString.PriceString = String.IsNullOrEmpty(priceBuy) ? "0" : priceBuy;
				newProductSimpleString.SectionCode = record[6].Trim().LeadingZero3();



				newProductSimpleString.SupplierCode = "";

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

				productMakatDBDictionary[makat] = null;  //newProductSimple.Makat,
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
