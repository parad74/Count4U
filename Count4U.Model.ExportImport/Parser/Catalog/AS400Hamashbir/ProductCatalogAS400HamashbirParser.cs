﻿using System;
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
	public class ProductCatalogAS400HamashbirParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogAS400HamashbirParser(IServiceLocator serviceLocator,
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

			//SPARITEWSN
			//Files type : Fixed ANSI 
			//CatalogMinLengthIncomingRow = 10,
			//	CatalogItemCodeStart = 3,
			//	CatalogItemCodeEnd = 10, 
			//	CatalogItemNameStart = 11,
			//	CatalogItemNameEnd = 40,
			//	SectionCodeStart=41, 
			//	SectionCodeEnd=44,
			//	SectionNameStart = 45,
			//	SectionNameEnd = 56,
			//	SupplierCodeStart = 73, 
			//	SupplierCodeEnd = 77,
			//	CatalogPriceSaleStart = 82,
			//	CatalogPriceSaleEnd = 90 
		
//	Field1: CompanyCode (col 1-2)
//Field2: Item Code (col 3-10)
//Field3: Name (col 11-40)

//Field4: FamilyID (col 41-44)
//Field5: FamilyName (col 45-56)
 //Field6: SectionID (col 57-60)
//Field7: SectionName (col 61-72)

//Field8: SupplierID (col 73-77)
//Field9: PriceSell (col 82-90)


			List<Point> startEndSubstring = new List<Point>();
			//0			 //Field2: Item Code (col 3-10)
			startEndSubstring.Add(new Point					
			{
				Start = catalogParserPoints.CatalogItemCodeStart,
				End = catalogParserPoints.CatalogItemCodeEnd,
				Length = catalogParserPoints.CatalogItemCodeEnd - catalogParserPoints.CatalogItemCodeStart + 1
			});
			//1	   //Field3: Name (col 11-40)
			startEndSubstring.Add(new Point		
			{
				Start = catalogParserPoints.CatalogItemNameStart,
				End = catalogParserPoints.CatalogItemNameEnd,
				Length = catalogParserPoints.CatalogItemNameEnd - catalogParserPoints.CatalogItemNameStart	+ 1
			});
			//2		 //Field6: SectionID (col 57-60)
			startEndSubstring.Add(new Point
			{		  
				Start = catalogParserPoints.SectionCodeStart,
				End = catalogParserPoints.SectionCodeEnd,
				Length = catalogParserPoints.SectionCodeEnd - catalogParserPoints.SectionCodeStart + 1
			});
			//3			//Field8: SupplierID (col 73-77)
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.SupplierCodeStart,
				End = catalogParserPoints.SupplierCodeEnd,
				Length = catalogParserPoints.SupplierCodeEnd - catalogParserPoints.SupplierCodeStart + 1
			});
			//4		 //Field9: PriceSell (col 82-90)
			startEndSubstring.Add(new Point		
			{
				Start = catalogParserPoints.CatalogPriceSaleStart,
				End = catalogParserPoints.CatalogPriceSaleEnd,
				Length = catalogParserPoints.CatalogPriceSaleEnd - catalogParserPoints.CatalogPriceSaleStart + 1
			});
			//5	//Field4: FamilyID (col 41-44)
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.FamilyCodeStart,
				End = catalogParserPoints.FamilyCodeEnd,
				Length = catalogParserPoints.FamilyCodeEnd - catalogParserPoints.FamilyCodeStart + 1
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

				//	CatalogItemCodeStart = 3,				 0
				//	CatalogItemCodeEnd = 10, 

				//	CatalogItemNameStart = 11,				1
				//	CatalogItemNameEnd = 40,

				//	SectionCodeStart=57, 					   2
				//	SectionCodeEnd=60,

				//	SupplierCodeStart = 73, 				  3
				//	SupplierCodeEnd = 77,

				//	CatalogPriceSaleStart = 82,			  4
				//	CatalogPriceSaleEnd = 90 

				//5	//Field4: FamilyID (col 41-44)			5
				//FamilyCodeStart = 41
				//FamilyCodeEnd	 = 44

				String[] record = { "", "", "", "", "0.0", ""};
			
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
				//	CatalogItemCodeStart = 3,				 0
				//	CatalogItemCodeEnd = 10, 

				//	CatalogItemNameStart = 11,				1
				//	CatalogItemNameEnd = 40,

				//	SectionCodeStart=57, 					   2
				//	SectionCodeEnd=60,

				//	SupplierCodeStart = 73, 				  3
				//	SupplierCodeEnd = 77,

				//	CatalogPriceSaleStart = 82,			  4
				//	CatalogPriceSaleEnd = 90 

				//		5
				//FamilyCodeStart = 41
				//FamilyCodeEnd	 = 44

				newProductSimpleString.Makat = makat;//.TrimStart('0');
				string name = record[1].ReverseDosHebrew(invertLetter, rt2lf); 
 				newProductSimpleString.Name = name;
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				newProductSimpleString.BalanceQuantityERP = "0";
		
				newProductSimpleString.SectionCode = record[2].Trim().LeadingZero4(); 				  //test!
				string priceSale = record[4];
				newProductSimpleString.PriceSale = priceSale.FormatComa2();
				newProductSimpleString.SupplierCode = record[3].Trim();
				newProductSimpleString.FamilyCode = record[5].Trim().LeadingZero4(); 	 
		
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
