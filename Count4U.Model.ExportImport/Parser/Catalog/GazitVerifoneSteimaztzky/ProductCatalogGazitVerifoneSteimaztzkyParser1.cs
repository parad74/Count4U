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
	public class ProductCatalogGazitVerifoneSteimaztzkyParser1 : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogGazitVerifoneSteimaztzkyParser1(IServiceLocator serviceLocator,
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
			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding,
				separators,
				countExcludeFirstString))
			{

				if (record == null) continue;

				if (record.Length < 2)
				{
					continue;
				}
				//Field1: Item Code 										0
				//Field2: Name												1
				// Field3: Barcode1**– 1 cut last digit				2
				//Field4: Barcode2											3
				//Field5: Barcode3											4
				//Field6: Barcode4 											5
				//Field7: Barcode5 											6
				//Field8: Barcode6 											7
				//Field9: Barcode7 											8
				//Field10: Barcode8											9
	
				string makat = record[0].Trim();
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					continue;
				}

				int count = 10;
				if (record.Length < 10) count = record.Length;

				for (int i = 2; i < count; i++)
				{
					
					string barcode = record[i].Trim();
					//string barcode = record[0].Trim();
					if (string.IsNullOrWhiteSpace(barcode) == true)
					{
						continue;
					}
					if (i == 2)
					{
						if (barcode.Length > 0)
						{
							barcode = barcode.Substring(0, barcode.Length - 1);
						}
					}

					ProductSimpleString newBarcodeProductSimpleString = new ProductSimpleString();
					Product newBarcodeProductSimple = new Product();

					newBarcodeProductSimpleString.MakatOriginal = barcode;
					if (barcodeApplyMask == true)
					{
						barcode = maskPackage.BarcodeMaskTemplate.FormatString(
						barcode, maskPackage.BarcodeMaskRecord.Value);
					}


					if (makatApplyMask == true)
					{
						makat = maskPackage.MakatMaskTemplate.FormatString(
							makat, maskPackage.MakatMaskRecord.Value);
					}

					if (makat == barcode)
					{
						continue;
					}

					if (productMakatDBDictionary.ContainsKey(barcode) == true)
					{
						//Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.BarcodeExistInDB, record.JoinRecord(separator)));
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
