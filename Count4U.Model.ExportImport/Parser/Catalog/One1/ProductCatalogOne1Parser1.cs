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
	public class ProductCatalogOne1Parser1 : BaseProductCatalogParser, IProductSimpleParser
	{
		public ProductCatalogOne1Parser1(IServiceLocator serviceLocator,
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
			base.ProductDictionary.Clear();

			string separator = ",";
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
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				// Barcode, ItemCode 
				//0				,1		
				
				string makat = record[1].Trim();
				string barcode = record[0].Trim();
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					base.Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.ParentMakatIsEmpty, record.JoinRecord(separator)));
					continue;
				}
				if (string.IsNullOrWhiteSpace(barcode) == true)
				{
					base.Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.BarcodeIsEmpty, record.JoinRecord(separator)));
					continue;
				}

				//======Makat ====== Product ========
				if (makatApplyMask == true)
				{
					makat = maskPackage.MakatMaskTemplate.FormatString(
						makat, maskPackage.MakatMaskRecord.Value);
				}
				//======== Barcode ==========	Product ========
				ProductSimpleString newBarcodeProductSimpleString = new ProductSimpleString();
				Product newBarcodeProductSimple = new Product();

				newBarcodeProductSimpleString.MakatOriginal = barcode;
				if (barcodeApplyMask == true)
				{
					barcode = maskPackage.BarcodeMaskTemplate.FormatString(
					barcode, maskPackage.BarcodeMaskRecord.Value);
				}

				if (barcode.Trim().ToUpper() == makat.Trim().ToUpper())
				{
					continue;
				}
				if (productMakatDBDictionary.ContainsKey(barcode) == true)
				{
					base.Log.Add(MessageTypeEnum.Warning, String.Format(ParserFileErrorMessage.BarcodeExistInDB, record.JoinRecord(separator)));
					continue;
				}
				newBarcodeProductSimpleString.Makat = barcode;

				newBarcodeProductSimpleString.TypeCode = TypeMakatEnum.B.ToString();
				newBarcodeProductSimpleString.ParentMakat = makat;
				newBarcodeProductSimpleString.Name = "";
				newBarcodeProductSimpleString.PriceBuy = "0";
				newBarcodeProductSimpleString.PriceSale = "0";
				newBarcodeProductSimpleString.SectionCode = "";
				//================Product=======================================
				int retBit = 0;
				// 2) Barcode + TypeMakatEnum.B + ParentMakat = Makat  (other record)
	
				retBit = newBarcodeProductSimple.ValidateError(newBarcodeProductSimpleString, base.Dtfi);  //makat error
				if (retBit != 0)  //Error
				{
					base.ErrorBitList.Add(new BitAndRecord	{Bit = retBit,	Record = record.JoinRecord(separator),	ErrorType = MessageTypeEnum.Error	});
					continue;
				}

				retBit = newBarcodeProductSimple.ValidateWarning(newBarcodeProductSimpleString, this.Dtfi);	 //Warning
				if (retBit != 0)
				{
					base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
				}

				//================productMakatDBDictionary=======================================
				productMakatDBDictionary[barcode] = null;
				newBarcodeProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
				newBarcodeProductSimple.InputTypeCode = InputTypeCodeEnum.B.ToString();

				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

				yield return newBarcodeProductSimple;
			} //foreach
		}

	}
}
