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
	public class ProductCatalogAS400AmericanEagleParser1 : BaseProductCatalogParser, IProductSimpleParser
	{
		public ProductCatalogAS400AmericanEagleParser1(IServiceLocator serviceLocator,
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

			//		Field1: Item Code (col 1-15)
			//Field2: Barcode1 (col 17-31)
			//Field3: Barcode2 (col 33-47)
			//Field4: Name (col 49-78)
			//Field5: SectionID (col 100-103)

			List<Point> startEndSubstring = new List<Point>();

			startEndSubstring.Add(new Point(catalogParserPoints.CatalogItemCodeStart, catalogParserPoints.CatalogItemCodeEnd));	 //Point(0, 15) 
			startEndSubstring.Add(new Point(catalogParserPoints.HamarotItemCodeStart, catalogParserPoints.HamarotItemCodeEnd));
			startEndSubstring.Add(new Point(catalogParserPoints.HamarotBarcodeStart, catalogParserPoints.HamarotBarcodeEnd));
			startEndSubstring.Add(new Point(catalogParserPoints.CatalogItemNameStart, catalogParserPoints.CatalogItemNameEnd));
			startEndSubstring.Add(new Point(catalogParserPoints.SectionCodeStart, catalogParserPoints.SectionCodeEnd));	 

			int count = 5;

			
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


				String[] record = { "", "", "", "", "" };
				//	Field1 goes into Makat 0
				//Field2 goes into Barcode	1
				//Field3 goes into Barcode	2
				//Field4 goes into Name		3
				//Field5 goes into SectionID	4


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

		
				for (int i = 1; i <= 2; i++)
				{
					string barcode = record[i].Trim();
					//string barcode = record[0].Trim();
					if (string.IsNullOrWhiteSpace(barcode) == true)
					{
						continue;
					}

					ProductSimpleString newBarcodeProductSimpleString = new ProductSimpleString();
					Product newBarcodeProductSimple = new Product();

					newBarcodeProductSimpleString.MakatOriginal = barcode;
					if (barcodeApplyMask == true)
					{
						barcode = maskPackage.BarcodeMaskTemplate.FormatString(
						barcode, maskPackage.BarcodeMaskRecord.Value);
					}

					if (makat == barcode)
					{
						continue;
					}

					if (productMakatDBDictionary.ContainsKey(barcode) == true)
					{
						continue;
					}

					newBarcodeProductSimpleString.Makat = barcode;
					newBarcodeProductSimpleString.TypeCode = TypeMakatEnum.B.ToString();
					newBarcodeProductSimpleString.ParentMakat = makat;
					newBarcodeProductSimpleString.Name = "";
					newBarcodeProductSimpleString.PriceString = "0";
					newBarcodeProductSimpleString.SupplierCode = "";

					int retBit = newBarcodeProductSimple.ValidateError(newBarcodeProductSimpleString, this.Dtfi);
					if (retBit != 0)  //Error
					{
						this._errorBitList.Add(new BitAndRecord
						{
							Bit = retBit,
							Record = record.JoinRecord(separator),
							ErrorType = MessageTypeEnum.Error
						});
						continue;
					}

					retBit = newBarcodeProductSimple.ValidateWarning(newBarcodeProductSimpleString, this.Dtfi);
					if (retBit != 0)
					{
						this._errorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
					}

					//================productMakatDBDictionary=======================================
					productMakatDBDictionary[barcode] = null;
					newBarcodeProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
					newBarcodeProductSimple.InputTypeCode = InputTypeCodeEnum.B.ToString();

					if (cancellationToken.IsCancellationRequested == true) break;
					k++;
					if (k % 100 == 0) countAction(k);

					yield return newBarcodeProductSimple;
				} //for i
			} //foreach

		}
		
	}
}
