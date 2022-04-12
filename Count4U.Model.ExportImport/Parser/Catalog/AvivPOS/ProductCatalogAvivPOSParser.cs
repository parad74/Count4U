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
	public class ProductCatalogAvivPOSParser : BaseProductCatalogParser, IProductSimpleParser
	{
		public ProductCatalogAvivPOSParser(IServiceLocator serviceLocator,
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

				if (record.Length < 8)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}
				// Item Code ,  Barcode, Item Name, ERP Quantity Expected,  Price Sell, Price Buy, Supplier Code, Supplier Name
					//0			,1					,2					,3									,4				,5				,6					,7						

				string makat = record[0].Trim();	   //case 1:	Item has 2 codes (field1 -long item code & field 2- short item code)
				if (string.IsNullOrWhiteSpace(makat) == true)  // case 2: field1 empty
				{
					makat = record[1].Trim();							 //case 2:	  Item has only 1 code (field2 - short item code)
					if (string.IsNullOrWhiteSpace(makat) == true)
					{
						Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
						continue;
					}
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
				//Barcode,  Item Code , Item Name, ERP Quantity Expected,  Price Sell, Price Buy, Supplier Code, Supplier Name
				//0			,1					,2					,3									,4				,5				,6					,7						

				string productName = record[2].ReverseDosHebrew(invertLetter, rt2lf); 
				newMakatProductSimpleString.Makat = makat;
				newMakatProductSimpleString.Name = productName;
				newMakatProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				string priceBuy = record[5].Trim();
				priceBuy = String.IsNullOrEmpty(priceBuy) ? "0" : priceBuy;
				newMakatProductSimpleString.PriceBuy = priceBuy; 
				string priceSell = record[4].Trim();
				priceSell = String.IsNullOrEmpty(priceSell) ? "0" : priceSell;
				newMakatProductSimpleString.PriceSale = priceSell;
				newMakatProductSimpleString.SupplierCode = record[6].Trim();


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

				//=======Supplier=====================================
				if (importType.Contains(ImportDomainEnum.ImportSupplier) == true)
				{
					if (base.SupplierDictionary.ContainsKey(newMakatProductSimpleString.SupplierCode) == false)
					{
						base.SupplierDictionary.Add(newMakatProductSimpleString.SupplierCode,
							new Supplier { SupplierCode = newMakatProductSimpleString.SupplierCode.Trim(), Name = record[7].Trim() });
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
