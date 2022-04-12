using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Count4U.Model;
using Count4U.Model.Main;
using System.Threading;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class ProductCatalogFRSVisionMirkamParser : BaseProductCatalogParser, IProductSimpleParser
	{
		public ProductCatalogFRSVisionMirkamParser(IServiceLocator serviceLocator,
			ILog log)
			: base(serviceLocator, log)
		{
			base.Dtfi.ShortDatePattern = @"dd/MM/yyyy";
			base.Dtfi.ShortTimePattern = @"hh:mm:ss";
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

			//if (productMakatDBDictionary == null) productMakatDBDictionary = new Dictionary<string, ProductMakat>();
			base.SupplierDictionary.Clear();
			base.ErrorBitList.Clear();

			string separator = ",";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			MaskPackage maskPackage = parms.GetMaskPackageFromParms();
			bool makatApplyMask = false;
			bool barcodeApplyMask = false;
			GetApplyMasks(importType, maskPackage, out makatApplyMask, out barcodeApplyMask);

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

				if (record == null) continue;

				if (record.Length < 14)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

//	Field1: Item Code			0
//Field2: Name					1
//Field4: PriceBuy				3
//Field6: SupplierCode		5
//Field7: SupplierName		6
//Field8: Barcode				7
//Field9: PriceSell				8
//Field12: SectionID			11
//Field17: SectionCode		16

							
				string makat = record[0].Trim();
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
					continue;
				}
				//======Makat ====== Product ========
				ProductSimpleString newMakatProductSimpleString = new ProductSimpleString();
				Product newMakatProductSimple = new Product();

				newMakatProductSimpleString.MakatOriginal = makat;
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
				string productName = record[1].ReverseDosHebrew(invertLetter, rt2lf); 
				newMakatProductSimpleString.Makat = makat;
				newMakatProductSimpleString.Name = productName;
				newMakatProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newMakatProductSimpleString.SectionCode =  DomainUnknownCode.UnknownSection;
				if (record.Length > 16)				//16
				{
					newMakatProductSimpleString.SectionCode = record[16].Trim();
				}
				newMakatProductSimpleString.SupplierCode = record[5].Trim();
				newMakatProductSimpleString.PriceBuy = record[3].Trim();
				newMakatProductSimpleString.PriceSale = record[8].Trim();

				//================Product=======================================
				int retBit = newMakatProductSimple.ValidateError(newMakatProductSimpleString, base.Dtfi);  //makat error
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

				retBit = newMakatProductSimple.ValidateWarning(newMakatProductSimpleString, base.Dtfi);	 //Warning
				if (retBit != 0)
				{
					base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
				}
				//	Field1: Item Code			0
				//Field2: Name					1
				//Field4: PriceBuy				3
				//Field6: SupplierCode		5
				//Field7: SupplierName		6
				//Field8: Barcode				7
				//Field9: PriceSell				8
				//Field12: SectionID			11
				//Field17: SectionCode		16
				//=======Supplier=====================================
				if (importType.Contains(ImportDomainEnum.ImportSupplier) == true)
				{
					if (base.SupplierDictionary.ContainsKey(newMakatProductSimpleString.SupplierCode) == false)
					{
						base.SupplierDictionary.Add(newMakatProductSimpleString.SupplierCode,
							new Supplier { SupplierCode = newMakatProductSimpleString.SupplierCode, Name = record[6] });
					}
				}
				//========	  productMakatDBDictionary ==========
				productMakatDBDictionary[makat] = null;
		
				newMakatProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
				newMakatProductSimple.InputTypeCode = InputTypeCodeEnum.B.ToString();

				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

				yield return newMakatProductSimple;
			} //foreach
		}
	}
}
