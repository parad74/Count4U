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
	public class ProductCatalogForPriorityRenuarParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogForPriorityRenuarParser(IServiceLocator serviceLocator,
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

			bool withFamily = parms.GetBoolValueFromParm(ImportProviderParmEnum.WithFamily);

			MaskPackage maskPackage = parms.GetMaskPackageFromParms();
			bool makatApplyMask = false;
			bool barcodeApplyMask = false;
			GetApplyMasks(importType, maskPackage, out makatApplyMask, out barcodeApplyMask);
			CatalogParserPoints catalogParserPoints = parms.GetCatalogParserPointsFromParm();
			bool invertLetter = false;
			bool rt2lf = false;
			if (encoding == Encoding.GetEncoding(862) || encoding == Encoding.GetEncoding(1255))
			{
				invertLetter = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLetters);
				rt2lf = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWords);
			}

			//  Catalog files: Items.dat
			//Files type : Fixed ANSI 

			//Items.dat
			//Field1: Item Code (col 1-13)
			//Field2: Barcode (col 17-29) - Optional
			//Field3: Name (col 35-65) 	- Optional
			//Field4: Price (col 115-124) - Optional

			//Field1 goes into Makat
			//Field2 goes into Barcode
			//Field3 goes into Name
			//Field3 goes into Price Sell (format : 7.2)

			//old/////Field1: Item Code (col 1-16)
			////////Field2: Item Name (col 17-36)
			////////Field3: Price Buy (col 37-46) – Optional

			////////Field1 goes into Makat
			////////Field2 goes into Name
			////////Field3 goes into PriceBuy

			//////{0,16,36,46};
			////List<Point> startEndSubstring = new List<Point>();
			////startEndSubstring.Add(new Point(0, 15, 16));	 //15 - 0 + 1
			////startEndSubstring.Add(new Point(16, 35, 20));	//35 - 16 + 1
			////startEndSubstring.Add(new Point(36, 45, 10));	//45 - 36 + 1

			//{0,16,32,46};
			//startEndSubstring.Add(new Point(0, 15, 16));	 //15 - 0 + 1
			//startEndSubstring.Add(new Point(16, 31, 16));	//31 - 16 + 1
			//startEndSubstring.Add(new Point(32, 45, 14));	//45 - 32 + 1

			List<Point> startEndSubstring = new List<Point>();

			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogItemCodeStart,
				End = catalogParserPoints.CatalogItemCodeEnd,
				Length = catalogParserPoints.CatalogItemCodeEnd - catalogParserPoints.CatalogItemCodeStart + 1
			});	 //Point(0, 12, 13) 12 - 0 + 1		//	 (col 1-13)
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogItemNameStart,
				End = catalogParserPoints.CatalogItemNameEnd,
				Length = catalogParserPoints.CatalogItemNameEnd - catalogParserPoints.CatalogItemNameStart	+ 1
			});	//Point(34, 64, 31) 64- 34 + 1	   //(col 35-65) 
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogPriceBuyStart,
				End = catalogParserPoints.CatalogPriceBuyEnd,
				Length = catalogParserPoints.CatalogPriceBuyEnd - catalogParserPoints.CatalogPriceBuyStart	+ 1
			});	 //Point(114, 123, 10) 123 - 114 + 1  (col 115-124) 
			int count = 3;

			
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

				

				String[] record = { "", "", "0.0" };
				//makat		barcode				  name		<...>	 price		
				//1101000         1101000                                МВЕТО ИЧ'В rounded jacket                   'ЗЙ 0000000.00 0000279.00 NIS 0000000.00 101      1056.00       0000000000.00     МВЕТО ИЧ'В                                                     МЙТО+ИСЕЕ+ИЧ'В       TREND                Main              0         

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
				string family = record[0];

				string makat = record[0].Trim();
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
					continue;
				}
 //old
//1-2 : Woman		//1
// 3-4 Men			//2
// 6-7 TFS			//3
//0,5,8,9 - None//0

				//new
//	all sections -> ""
//001 -> W
//002 -> M
//003 + 004+ 0 -> T --- this file is not required.
//003 + 0 -> TW
//004 -> TM
				string startNum = makat.Substring(0, 1);
				string sectionCode = "0";
				if (startNum == "1" || startNum == "2")
				{
					sectionCode = "001";
				}
				else if (startNum == "3" || startNum == "4")
				{
					sectionCode = "002";
				}
				else if (startNum == "6" || startNum == "7")
				{
					sectionCode = "003";
				}
				else if (startNum == "5" || startNum == "8")
				{
					sectionCode = "004";
				}
				else if (startNum == "9"
					|| startNum == "0")
				{
					sectionCode = "0";
				}

				newProductSimpleString.MakatOriginal = makat;

				if (makat.Length < 10)
				{
					if (makatApplyMask == true)
					{
						makat = maskPackage.MakatMaskTemplate.FormatString(
							makat, maskPackage.MakatMaskRecord.Value);
					}
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

				newProductSimpleString.Makat = makat;
				string name = record[1].ReverseDosHebrew(invertLetter, rt2lf); 
 				newProductSimpleString.Name = name;
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				string price = record[2].Trim();
				newProductSimpleString.PriceString = String.IsNullOrEmpty(price) ? "0" : price;
				newProductSimpleString.PriceSale = record[2].Trim();
				newProductSimpleString.SupplierCode = "";
				newProductSimpleString.SectionCode = sectionCode;
				
		
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
				
				if (withFamily == true)
				{
					if (family.Length > 7)
					{
						family = family.Substring(0, 7);
					}
					newProductSimple.Family = family;
					newProductSimple.FamilyCode = family;
				}

				productMakatDBDictionary[makat] = null;  
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
