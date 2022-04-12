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
	
	public class ProductCatalogGazitGlobalXlsxXParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogGazitGlobalXlsxXParser(IServiceLocator serviceLocator,
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



  		foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding,
				separators,
				countExcludeFirstString))
			{
//Field3: Section Code						 2
//Field4: Item Code							 3

//Field7: Name							6
//Field8: Buy Price  						7 

//Field10: Supplier Code 				9
				if (record == null) continue;

				if (record.Length < 7)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}
		
				string makat = record[3].Trim();
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
				//Field7: Name							6
				try
				{
					newProductSimpleString.Makat = makat;
					string name = record[6].ReverseDosHebrew(invertLetter, rt2lf);

					newProductSimpleString.Name = name;
					newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
					newProductSimpleString.ParentMakat = "";
					//newProductSimpleString.UnitTypeCode = record[2].Trim();
					newProductSimpleString.BalanceQuantityERP = "0";
	


					//Field8: Buy Price  						7 
					newProductSimpleString.PriceBuy = "0";
					if (record.Length > 7) //8,9
					{
						string priceBuy = record[7].Trim(' ');
						newProductSimpleString.PriceBuy = priceBuy;
					}

					//Field11: Sell Price			10
					newProductSimpleString.PriceSale = "0";
					if (record.Length > 10)
					{
						string priceSale = record[10].Trim(' ');
						newProductSimpleString.PriceSale = priceSale;
					}

					//Field3: Section Code					2
					newProductSimpleString.SectionCode = DomainUnknownCode.UnknownSection;
					string sectionCode = record[2].Trim(' ');		
					if (string.IsNullOrWhiteSpace(sectionCode) == false)
					{
						newProductSimpleString.SectionCode = sectionCode.LeadingZero3();
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

					//Field10: Supplier Code 				9
					newProductSimpleString.SupplierCode = DomainUnknownCode.UnknownSupplier;
					if (record.Length > 9)				  //10...
					{
						string supplierCode = record[9].Trim(' ');		 //	record[7]
						if (string.IsNullOrWhiteSpace(supplierCode) == false)
						{
							newProductSimpleString.SupplierCode = supplierCode;
						}
					}

					newProductSimpleString.FamilyCode = DomainUnknownCode.UnknownFamily;
					//if (record.Length > 9)				  //9...
					//{
					//	string familyCode = record[9].Trim(' ');		 //	record[9]
					//	newProductSimpleString.FamilyCode = familyCode;
					//}

					newProductSimpleString.UnitTypeCode = DomainUnknownCode.UnknownUnitType;
					//if (record.Length > 10)				  //10
					//{
					//	string unitTypeCode = record[10].Trim(' ');		 //	record[10]
					//	newProductSimpleString.UnitTypeCode = unitTypeCode;
					//}

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
				}
				catch (Exception exc)
				{
					string ret = k.ToString();
				}

			yield return newProductSimple;

			} //foreach
		}
	}
}
