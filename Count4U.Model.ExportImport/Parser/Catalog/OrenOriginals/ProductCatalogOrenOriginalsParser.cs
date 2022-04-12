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
	public class ProductCatalogOrenOriginalsParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogOrenOriginalsParser(IServiceLocator serviceLocator,
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

			if (record.Length < 5)
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
				continue;
			}
			//	Field1: Barcode			//0
			//Field2: Name				//1			
			//Field3: Barcode2			//2
			//Field4+5: Item Code *	//3+4

			//======Makat ====== 
			string makat = record[3].Trim() + record[4].Trim();
			if (string.IsNullOrWhiteSpace(makat) == true)
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
				continue;
			}
			
			ProductSimpleString newMakatProductSimpleString = new ProductSimpleString();
				Product newProductSimple = new Product();

				newMakatProductSimpleString.MakatOriginal = makat;
				if (makatApplyMask == true)
				{
					makat = maskPackage.MakatMaskTemplate.FormatString(makat, maskPackage.MakatMaskRecord.Value);
				}

				if (productMakatDBDictionary.ContainsKey(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatExistInDB, record.JoinRecord(separator)));
					continue;
				}

				//================Product=======================================
				//	Field1: Barcode			//0
				//Field2: Name				//1			
				//Field3: Barcode2			//2
				//Field4+5: Item Code *	//3+4

				string productName = record[1].ReverseDosHebrew(invertLetter, rt2lf);
				newMakatProductSimpleString.Makat = makat;
				newMakatProductSimpleString.Name = productName;
				newMakatProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();

				int retBit = newProductSimple.ValidateError(newMakatProductSimpleString, this.Dtfi);
				if (retBit != 0)  //Error
				{
					this._errorBitList.Add(new BitAndRecord 	{Bit = retBit, Record = record.JoinRecord(separator),	ErrorType = MessageTypeEnum.Error});
					continue;
				}

				retBit = newProductSimple.ValidateWarning(newMakatProductSimpleString, this.Dtfi);
				if (retBit != 0)
				{
					this._errorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
				}

				//================productMakatDBDictionary=======================================
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
