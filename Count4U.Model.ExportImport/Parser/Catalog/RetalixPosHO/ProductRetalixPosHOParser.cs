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
	public class ProductRetalixPosHOParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductRetalixPosHOParser(IServiceLocator serviceLocator,
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

//Field1: Item Code (col 1-13)					 //0
//Field2: Name (col 15-29)					   //1
//Field4: SectionID (col 38-39)				//2
//Field6: PriceSell (col 45-51)				   //3
//Field7: PriceBuy (col 53-59)				   //4
//“-“ sign appears in position 61		 //5
 //Field8: Quantity Expected (col 61-67)		 //6
//Field10: Item Type (col 72-73)

			List<Point> startEndSubstring = new List<Point>();

			// Item Code (col 1-13)					 //0
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogItemCodeStart,
				End = catalogParserPoints.CatalogItemCodeEnd,
				Length = catalogParserPoints.CatalogItemCodeEnd - catalogParserPoints.CatalogItemCodeStart + 1
			});
			// Name (col 15-29)					   //1
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogItemNameStart,
				End = catalogParserPoints.CatalogItemNameEnd,
				Length = catalogParserPoints.CatalogItemNameEnd - catalogParserPoints.CatalogItemNameStart	+ 1
			});	
			//SectionID (col 38-39)				//2
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.SectionCodeStart,
				End = catalogParserPoints.SectionCodeEnd,
				Length = catalogParserPoints.SectionCodeEnd - catalogParserPoints.SectionCodeStart + 1
			});
			//PriceSell (col 45-51)				   //3
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogPriceSaleStart,
				End = catalogParserPoints.CatalogPriceSaleEnd,
				Length = catalogParserPoints.CatalogPriceSaleEnd - catalogParserPoints.CatalogPriceSaleStart + 1
			});
			//PriceBuy (col 53-59)				   //4
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogPriceBuyStart,
				End = catalogParserPoints.CatalogPriceBuyEnd,
				Length = catalogParserPoints.CatalogPriceBuyEnd - catalogParserPoints.CatalogPriceBuyStart + 1
			});

			//“-“ sign appears in position 61		 //5
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.QuantityTypeStart,
				End = catalogParserPoints.QuantityTypeEnd,
				Length = catalogParserPoints.QuantityTypeEnd - catalogParserPoints.QuantityTypeStart + 1
			});
			//Quantity Expected (col 61-67)		 //6
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.QuantityERPStart,
				End = catalogParserPoints.QuantityERPEnd,
				Length = catalogParserPoints.QuantityERPEnd - catalogParserPoints.QuantityERPStart + 1
			});
			//Field10: Item Type (col 72-73)		//7
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.UnitTypeCodeStart,
				End = catalogParserPoints.UnitTypeCodeEnd,
				Length = catalogParserPoints.UnitTypeCodeEnd - catalogParserPoints.UnitTypeCodeStart + 1
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

				//Item Code (col 1-13)				   //0
				//Name (col 15-29)					   //1
				//SectionID (col 38-39)				   //2
				//PriceSell (col 45-51)				   //3
				//PriceBuy (col 53-59)				   //4
				// "-"										   //5
				//Quantity Expected (col 61-67)	//6
				//Item Type (col 72-73)		//7

				String[] record = { "", "", "", "0.0", "0.0", "", "0.0" , "00"};
		
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
				//Item Code (col 1-13)				   //0
				//Name (col 15-29)					   //1
				//SectionID (col 38-39)				   //2
				//PriceSell (col 45-51)				   //3
				//PriceBuy (col 53-59)				   //4
				// "-"										   //5
				//Quantity Expected (col 61-67)	//6
				//Unit Type (col 72-73)		//7
				newProductSimpleString.Makat = makat;
				string name = record[1].ReverseDosHebrew(invertLetter, rt2lf); 
 				newProductSimpleString.Name = name;
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				newProductSimpleString.SectionCode = record[2].Trim().LeadingZero3(); 
				newProductSimpleString.BalanceQuantityERP = "0";
				if (withQuantityERP == true)
				{
					string balanceQuantityERP = record[3].FormatComa2();
					string sign = record[5];
					if (sign == "-" && string.IsNullOrWhiteSpace(balanceQuantityERP) == false)
					{	 
						balanceQuantityERP = "-" + balanceQuantityERP;
					}
					newProductSimpleString.BalanceQuantityERP = balanceQuantityERP; 
				}

					string priceSale = record[3];
					newProductSimpleString.PriceSale = priceSale.FormatComa2();
					string priceBuy = record[4];
					newProductSimpleString.PriceBuy = priceBuy.FormatComa2();
					newProductSimpleString.PriceString = String.IsNullOrEmpty(priceBuy) ? "0" : priceBuy;

					string unitType = record[7];
					newProductSimpleString.UnitTypeCode = unitType;

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
