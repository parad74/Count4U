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
	public class ProductCatalogForYarpaParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogForYarpaParser(IServiceLocator serviceLocator,
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
			//CatalogItemCodeStart = 2,
			//    CatalogItemCodeEnd = 7,
			//    CatalogItemNameStart = 14,
			//    CatalogItemNameEnd = 38,
			//    CatalogPriceSaleStart = 40,
			//    CatalogPriceSaleEnd = 49,
			//    QuantityInPackStart =  60,
			//    QuantityInPackEnd = 64,
			//    CatalogPriceBuyStart = 71,
			//    CatalogPriceBuyEnd = 79,
			//    HamarotBarcodeStart = 85,
			//    HamarotBarcodeEnd = 97

		
  			List<Point> startEndSubstring = new List<Point>();

			startEndSubstring.Add(new Point			  //1
			{
				Start = catalogParserPoints.CatalogItemCodeStart,
				End = catalogParserPoints.CatalogItemCodeEnd,
				Length = catalogParserPoints.CatalogItemCodeEnd - catalogParserPoints.CatalogItemCodeStart + 1
			});	 
			startEndSubstring.Add(new Point			   //2
			{
				Start = catalogParserPoints.CatalogItemNameStart,
				End = catalogParserPoints.CatalogItemNameEnd,
				Length = catalogParserPoints.CatalogItemNameEnd - catalogParserPoints.CatalogItemNameStart	+ 1
			});
			startEndSubstring.Add(new Point				 //3
			{
				Start = catalogParserPoints.CatalogPriceSaleStart,
				End = catalogParserPoints.CatalogPriceSaleEnd,
				Length = catalogParserPoints.CatalogPriceSaleEnd - catalogParserPoints.CatalogPriceSaleStart + 1
			});
			//    QuantityInPackStart =  60,
			//    QuantityInPackEnd = 64,
			startEndSubstring.Add(new Point		   //4
			{
				Start = catalogParserPoints.QuantityInPackStart,
				End = catalogParserPoints.QuantityInPackEnd,
				Length = catalogParserPoints.QuantityInPackEnd - catalogParserPoints.QuantityInPackStart + 1
			});
			//    CatalogPriceBuyStart = 71,
			//    CatalogPriceBuyEnd = 79,
			startEndSubstring.Add(new Point				//5
			{
				Start = catalogParserPoints.CatalogPriceBuyStart,
				End = catalogParserPoints.CatalogPriceBuyEnd,
				Length = catalogParserPoints.CatalogPriceBuyEnd - catalogParserPoints.CatalogPriceBuyStart + 1
			});
 			//    HamarotBarcodeStart = 85,
			//    HamarotBarcodeEnd = 97
			//startEndSubstring.Add(new Point
			//{
			//    Start = catalogParserPoints.HamarotBarcodeStart,
			//    End = catalogParserPoints.HamarotBarcodeEnd,
			//    Length = catalogParserPoints.HamarotBarcodeEnd - catalogParserPoints.HamarotBarcodeStart + 1
			//});
	
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
				//ItemCode,	ItemName,  PriceSale,  QuantityInPack, PriceBuy
				//0				, 1					, 2				,3						,4
				String[] record = { "", "", "0.0", "0", "0.0" };
				
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
				//ItemCode,	ItemName,  PriceSale,  QuantityInPack, PriceBuy
				//0				, 1					, 2				,3						,4
				//Field1: Item Code (col 2-7)				    0
				//Field2: Name (col 14-38)						1
				//Field3: Sell Price (col 40-49)				2
				//Field4: Quantity In Pack (col 60-64)	3
				//Field5: Buy Price (col 71-79)				4
				//Field6: Barcode (col 85-97) 				5

				newProductSimpleString.Makat = makat;
				string name = record[1].ReverseDosHebrew(invertLetter, rt2lf); 
 				newProductSimpleString.Name = name;
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				//newProductSimpleString.UnitTypeCode = record[2].Trim();
				newProductSimpleString.BalanceQuantityERP = "0";
				//if (withQuantityERP == true)
				//{
				//    newProductSimpleString.BalanceQuantityERP = record[3].FormatComa2();
				//}
				//newProductSimpleString.SectionCode = record[4].Trim();
				string priceSale = record[2].Trim(' ');
				newProductSimpleString.PriceSale = priceSale;

				string countInParentPack = record[3];
				newProductSimpleString.CountInParentPack = countInParentPack;
			
				string priceBuy = record[4].Trim(' ');
				newProductSimpleString.PriceBuy = priceBuy;
				newProductSimpleString.PriceString = String.IsNullOrEmpty(priceBuy) ? "0" : priceBuy;

				//newProductSimpleString.SectionCode = "";
				//newProductSimpleString.SupplierCode = "";
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
