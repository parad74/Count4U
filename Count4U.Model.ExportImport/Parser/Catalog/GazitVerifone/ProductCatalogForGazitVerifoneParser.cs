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
	//ВНИМАНИЕ используется в 2 адаптерах
	public class ProductCatalogForGazitVerifoneParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogForGazitVerifoneParser(IServiceLocator serviceLocator,
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
			CatalogParserPoints gazitPoints = parms.GetCatalogParserPointsFromParm();

			bool invertLetter = false;
			bool rt2lf = false;
			if (encoding == Encoding.GetEncoding(862) || encoding == Encoding.GetEncoding(1255))
			{
				invertLetter = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLetters);
				rt2lf = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWords);
			}


			//Field1: Item Code (col 1-16)
			//Field2: Item Name (col 17-36)
			//Field3: Price Buy (col 37-46) – Optional

			//Field1 goes into Makat
			//Field2 goes into Name
			//Field3 goes into PriceBuy

			//////{0,16,36,46};
			////List<Point> startEndSubstring = new List<Point>();
			////startEndSubstring.Add(new Point(0, 15, 16));	 //15 - 0 + 1
			////startEndSubstring.Add(new Point(16, 35, 20));	//35 - 16 + 1
			////startEndSubstring.Add(new Point(36, 45, 10));	//45 - 36 + 1

			//{0,16,32,46};
			//startEndSubstring.Add(new Point(0, 15, 16));	 //15 - 0 + 1
			//startEndSubstring.Add(new Point(16, 31, 16));	//31 - 16 + 1
			//startEndSubstring.Add(new Point(32, 45, 14));	//45 - 32 + 1

			//{0,16,32,46};
			List<Point> startEndSubstring = new List<Point>();

			startEndSubstring.Add(new Point
			{
				Start = gazitPoints.CatalogItemCodeStart,
				End = gazitPoints.CatalogItemCodeEnd,
				Length = gazitPoints.CatalogItemCodeEnd - gazitPoints.CatalogItemCodeStart + 1
			});	 //Point(0, 15, 16) 15 - 0 + 1
			startEndSubstring.Add(new Point
			{
				Start = gazitPoints.CatalogItemNameStart,
				End = gazitPoints.CatalogItemNameEnd,
				Length = gazitPoints.CatalogItemNameEnd - gazitPoints.CatalogItemNameStart	+ 1
			});	 	//Point(16, 31, 16) 31 - 16 + 1
			startEndSubstring.Add(new Point
			{
				Start = gazitPoints.CatalogPriceBuyStart,
				End = gazitPoints.CatalogPriceBuyEnd,
				Length = gazitPoints.CatalogPriceBuyEnd - gazitPoints.CatalogPriceBuyStart	+ 1
			});	 	//Point(32, 45, 14) 45 - 32 + 1
			int count = 3;

			
			foreach (String rec in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				if (string.IsNullOrWhiteSpace(rec) == true) { continue; }
				int lenRow = rec.Length;
	
				//if (len < 36)
				if (lenRow < gazitPoints.CatalogMinLengthIncomingRow)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedLengthString, rec));
					continue;
				}

				//string recReverse = rec.ReverseDosHebrew(invertLetter, rt2lf); 

				String[] record = { "", "", "0.0" };
				//makat						  name			 price		
				//V42479-NS             F50 PREMIUM BP7  //36
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

				newProductSimpleString.Makat = makat;
				string name = record[1].ReverseDosHebrew(invertLetter, rt2lf);
				newProductSimpleString.Name = name;
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				string price = record[2].Trim();
				newProductSimpleString.PriceString = String.IsNullOrEmpty(price) ? "0" : price;
				newProductSimpleString.PriceSale = record[2].Trim();
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
					//new ProductMakat(
					//{
					//    Makat = newProductSimple.Makat,
					//    Name = newProductSimple.Name,
					//    MakatOriginal = newProductSimple.MakatOriginal,
					//    ParentMakat = "",
					//    TypeCode = TypeMakatEnum.M.ToString()
					//});
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
