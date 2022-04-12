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
	public class ProductCatalogForComaxASPParser : BaseProductCatalogParser, IProductSimpleParser
	{
		public ProductCatalogForComaxASPParser(IServiceLocator serviceLocator,
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

				if (record.Length < 6)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}
				//items.asp
				//Item Code,Description,					BarCode,				SAPAKCODE,SAPAK NAME,Value 1
				//10015,		чеф 1 ч в омбп 7 йзй,	7290003679669,	1,					1,					 0.00
				//0,					1								2								3				4						5
				//Makat		  Name							Barcode														PriceString

			
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
				string price = record[5].Trim();
				newMakatProductSimpleString.PriceString = String.IsNullOrEmpty(price) ? "0" : price;
				newMakatProductSimpleString.PriceBuy = String.IsNullOrEmpty(price) ? "0" : price;
				newMakatProductSimpleString.SupplierCode = record[3].Trim();


				//================Product=======================================
				//Makat appears more than once on items.asp file: 
				//Ignore the second Makat, 
				//write into the log file the line which contains the second appearance of the exists Makat 
				// 1)  Makat + Name + TypeMakatEnum.M + SapplierCode  + SapplierName	 + Price
				// 2) Barcode + TypeMakatEnum.B + ParentMakat = Makat  (other record)

				// 1)  Makat + Name + TypeMakatEnum.M + SapplierCode  + SapplierName	 + Price
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
							new Supplier { SupplierCode = newMakatProductSimpleString.SupplierCode, Name = record[4] });
					}
				}
				//========	  productMakatDBDictionary ==========
				productMakatDBDictionary[makat] = null;
				//productMakatDBDictionary.Add(newMakatProductSimple.Makat,
				//    new ProductMakat
				//    {
				//        Makat = newMakatProductSimple.Makat,
				//        Name = newMakatProductSimple.Name,
				//        ParentMakat = "",
				//        MakatOriginal = newMakatProductSimple.MakatOriginal,
				//        TypeCode = TypeMakatEnum.M.ToString()
				//    });

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
