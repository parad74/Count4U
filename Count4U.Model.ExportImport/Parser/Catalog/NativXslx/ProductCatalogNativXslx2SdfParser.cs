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
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class ProductCatalogNativXslx2SdfParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogNativXslx2SdfParser(IServiceLocator serviceLocator,
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

			
			int sheetNumberXlsx = parms.GetIntValueFromParm(ImportProviderParmEnum.SheetNumberXlsx);					// start from 1
			if (sheetNumberXlsx == 0) sheetNumberXlsx = 1;

			string sheetNameXlsx = parms.GetStringValueFromParm(ImportProviderParmEnum.SheetNameXlsx);				
			
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


			decimal codeCh = 0x200E;//33286;//8206;//(decimal)ch;	  //"\u200E"
			char oldCh = (char)codeCh;

		
  		foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding,
				separators,
				countExcludeFirstString,
				sheetNameXlsx,
				sheetNumberXlsx))
			{
//Field1 goes into Makat		    0
//Field2 goes into Name			1
//Field3 goes into ItemType	    2
 
				if (record == null) continue;

				if (record.Length < 3)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}
		
				string makat = record[0].Trim();
				makat = makat.CutLength(299);
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
					continue;
				}

				ProductSimpleString newProductSimpleString = new ProductSimpleString();
				Product newProductSimple = new Product();
		
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
				//Field1 goes into Makat		    0
				//Field2 goes into Name			1
				//Field3 goes into ItemType	    2
			////// Optional
				//Field4  FamilyCode		 3
				//Field5  Family // can use as FamilyName	 4
				 //Field6  SectionCode		 5
				//Field7  SectionName		 6
				//Field8  SubSectionCode		 7
				//Field9  SubSectionName		 8
				//Field10  PriceBuy						9
				//Field11  PriceSale						10
				//Field12  SupplierCode	 11
				//Field13  SupplierName	 12


			
				newProductSimpleString.Makat = makat;
				string name = record[1].CutLength(99);
				name = name.ReverseDosHebrew(invertLetter, rt2lf);
					
				name = name.Replace(oldCh, '%');
				name = name.Replace("%", "");
		
				newProductSimpleString.Name = name;
				newProductSimpleString.UnitTypeCode = record[2].Trim().CutLength(49);
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				//newProductSimpleString.UnitTypeCode = record[2].Trim();
				newProductSimpleString.BalanceQuantityERP = "0";

				////// Optional
				//Field10  PriceBuy						9		 Optional
				//Field11  PriceSale						10		 Optional
				newProductSimpleString.PriceBuy = "0";
				if (record.Length > 9)	//10,11...
				{
					string priceBuy = record[9].Trim(' ');
					newProductSimpleString.PriceBuy = priceBuy;
				}

				newProductSimpleString.PriceSale = "0";
				if (record.Length > 10) //11,12..
				{
					string priceSale = record[10].Trim(' ');
					newProductSimpleString.PriceSale = priceSale;
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

				////// Optional
			
				//Field4  FamilyCode		 3
				//Field5  Family // can use as FamilyName	 4
				newProductSimple.FamilyCode = DomainUnknownCode.UnknownFamily;
				if (record.Length > 3)	//4,5...
				{
					string familyCode = record[3].Trim(' ');
					newProductSimple.FamilyCode = familyCode;
				}
				newProductSimple.Family = "";
				if (record.Length > 4)	//5,6...
				{
					string family = record[4].Trim(' ');
					newProductSimple.Family = family;
				}

				//Field6  SectionCode		 5
				//Field7  SectionName		 6
				newProductSimple.SectionCode = DomainUnknownCode.UnknownSection; 
				if (record.Length > 5) //6,7..
				{
					string sectionCode = record[5].Trim(' ');
					newProductSimple.SectionCode = sectionCode;
				}

				newProductSimple.SectionName = "";
				if (record.Length > 6) //7,8..
				{
					string sectionName = record[6].Trim(' ');
					newProductSimple.SectionName = sectionName;
				}

				//Field8  SubSectionCode		 7
				//Field9  SubSectionName		 8
				newProductSimple.SubSectionCode = "";
				if (record.Length > 7) //8,9..
				{
					string subSectionCode = record[7].Trim(' ');
					newProductSimple.SubSectionCode = subSectionCode;
				}

				newProductSimple.SubSectionName = "";
				if (record.Length > 8) //9,10..
				{
					string subsectionName = record[8].Trim(' ');
					newProductSimple.SubSectionName = subsectionName;
				}

				//Field12  SupplierCode	 11
				//Field13  SupplierName	 12
				newProductSimple.SupplierCode = DomainUnknownCode.UnknownSupplier;
				if (record.Length > 11)	//11,12...
				{
					string supplierCode = record[11].Trim(' ');
					newProductSimple.SupplierCode = supplierCode;
				}

				newProductSimple.SupplierName = "";
				if (record.Length > 12)	//12,13...
				{
					string supplierName = record[12].Trim(' ');
					newProductSimple.SupplierName = supplierName;
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
