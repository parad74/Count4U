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
	public class ProductCatalogAS400MegaParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogAS400MegaParser(IServiceLocator serviceLocator,
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

			//Files type : Fixed ANSI 
//Field1: Barcode (col 1-13)
//Field2: Item Code (col 14-26)
//Field3: Name (col 28-47)
//Field4: SectionID (col 56-58)
//Field5: PriceSell (col 59-66)

//Field6: SupplierCode (col 76-85) **
//Field7: SupplierName (col 86-105) **

				//CatalogMinLengthIncomingRow = 26,
				//HamarotBarcodeStart = 1,
				//HamarotBarcodeEnd = 13,
				//CatalogItemCodeStart = 14,
				//CatalogItemCodeEnd = 26, 
				//CatalogItemNameStart = 28,
				//CatalogItemNameEnd = 47,

				//SectionCodeStart=56, 
				//SectionCodeEnd=58,
				//CatalogPriceSaleStart = 59,
				//CatalogPriceSaleEnd = 66, 
		
				//SupplierCodeStart = 76, 
				//SupplierCodeEnd = 85,	
				//SupplierNameStart = 86, 
				//SupplierNameEnd = 105,
		

			List<Point> startEndSubstring = new List<Point>();

			startEndSubstring.Add(new Point			 //Field1: Barcode (col 1-13)			0
			{
				Start = catalogParserPoints.HamarotBarcodeStart,
				End = catalogParserPoints.HamarotBarcodeEnd,
				Length = catalogParserPoints.HamarotBarcodeEnd - catalogParserPoints.HamarotBarcodeStart + 1
			});
			startEndSubstring.Add(new Point			//Field2: Item Code (col 14-26)		  1
			{
				Start = catalogParserPoints.CatalogItemCodeStart,
				End = catalogParserPoints.CatalogItemCodeEnd,
				Length = catalogParserPoints.CatalogItemCodeEnd - catalogParserPoints.CatalogItemCodeStart + 1
			});
			startEndSubstring.Add(new Point			//Field3: Name (col 28-47)				2
			{
				Start = catalogParserPoints.CatalogItemNameStart,
				End = catalogParserPoints.CatalogItemNameEnd,
				Length = catalogParserPoints.CatalogItemNameEnd - catalogParserPoints.CatalogItemNameStart + 1
			});

			startEndSubstring.Add(new Point		//Field4: SectionID (col 56-58)			3
			{
				Start = catalogParserPoints.SectionCodeStart,
				End = catalogParserPoints.SectionCodeEnd,
				Length = catalogParserPoints.SectionCodeEnd - catalogParserPoints.SectionCodeStart + 1
			});
			startEndSubstring.Add(new Point		   //Field5: PriceSell (col 59-66)			  4
			{
				Start = catalogParserPoints.CatalogPriceSaleStart,
				End = catalogParserPoints.CatalogPriceSaleEnd,
				Length = catalogParserPoints.CatalogPriceSaleEnd - catalogParserPoints.CatalogPriceSaleStart + 1
			});
			startEndSubstring.Add(new Point		//Field6: SupplierCode (col 76-85) **		 5
			{
				Start = catalogParserPoints.SupplierCodeStart,
				End = catalogParserPoints.SupplierCodeEnd,
				Length = catalogParserPoints.SupplierCodeEnd - catalogParserPoints.SupplierCodeStart + 1
			});
			//startEndSubstring.Add(new Point		//Field7: SupplierName (col 86-105) **	  6
			//{
			//	Start = catalogParserPoints.SupplierNameStart,
			//	End = catalogParserPoints.SupplierNameEnd,
			//	Length = catalogParserPoints.SupplierNameEnd - catalogParserPoints.SupplierNameStart + 1
			//});
		
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

			
				String[] record = { "", "", "", "", "0.0", "", ""};
			
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
				string makat = record[1].Trim();
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

				//Field1: Barcode (col 1-13)							0
				//Field2: Item Code (col 14-26)					1
				//Field3: Name (col 28-47)							2
				//Field4: SectionID (col 56-58)					3
				//Field5: PriceSell (col 59-66)						4

				//Field6: SupplierCode (col 76-85) **			5
				//Field7: SupplierName (col 86-105) **		6

				newProductSimpleString.Makat = makat;//.TrimStart('0');
				string isEmpty = makat.TrimStart('0');
				if (string.IsNullOrWhiteSpace(isEmpty) == true) 
					continue;
				string name = record[2].ReverseDosHebrew(invertLetter, rt2lf); 
 				newProductSimpleString.Name = name;
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				//newProductSimpleString.UnitTypeCode = record[2].Trim();
				newProductSimpleString.BalanceQuantityERP = "0";
				//string sign = record[4].Trim();

				//if (withQuantityERP == true)
				//{
				//	string quantityERP = record[3].Trim();

				//	if (quantityERP == "000000" || quantityERP == "00000" || quantityERP == "0000" || quantityERP == "000" || quantityERP == "00" || quantityERP == "0")
				//	{
				//		newProductSimpleString.BalanceQuantityERP = "0";
				//	}
				//	else
				//	{
				//		newProductSimpleString.BalanceQuantityERP = quantityERP.TrimStart('0');
				//		if (sign == "-") newProductSimpleString.BalanceQuantityERP = "-" + newProductSimpleString.BalanceQuantityERP;
				//	}
				//}


				//Field4: SectionID (col 56-58)					3
				//Field5: PriceSell (col 59-66)						4

				//Field6: SupplierCode (col 76-85) **			5
				//Field7: SupplierName (col 86-105) **		6
				newProductSimpleString.SectionCode = record[3].Trim().LeadingZero3(); 
				string priceSale = record[4];
				newProductSimpleString.PriceSale = priceSale.FormatComa2();
				//string priceBuy = record[7];
				//newProductSimpleString.PriceBuy = priceBuy.FormatComa2();
				//newProductSimpleString.PriceString = String.IsNullOrEmpty(priceBuy) ? "0" : priceBuy;
				newProductSimpleString.SupplierCode = DomainUnknownCode.UnknownSupplier;

				isEmpty = record[5].TrimStart('0');
				if (string.IsNullOrWhiteSpace(isEmpty) == false) 
				{
					newProductSimpleString.SupplierCode = record[5].Trim();
				}
		
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
