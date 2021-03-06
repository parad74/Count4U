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
	public class ProductCatalogIturERPCodeQuantityParser : BaseProductCatalogParser, IProductSimpleParser
	{
		public ProductCatalogIturERPCodeQuantityParser(
		ILog log,  IServiceLocator serviceLocator )
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

			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Dictionary<string, Itur> iturDictionary = iturRepository.GetERPIturDictionary(dbPath);

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

				if (record.Length < 3)
				{
					continue;
				}
				//Makat													0
				//Itur Code ERP										1
				//Quantity expected per item in Itur		2

				string makat = record[0].Trim();
				string iturCodeErp = record[1].Trim();
				
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					continue;
				}
				if (string.IsNullOrWhiteSpace(iturCodeErp) == true)
				{
					continue;
				}

				string barcode = makat + "##" + iturCodeErp;

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
				//if (barcodeApplyMask == true)
				//{
				//	barcode = maskPackage.BarcodeMaskTemplate.FormatString(
				//	barcode, maskPackage.BarcodeMaskRecord.Value);
				//}

				if (makat == barcode) continue;

				if (productMakatDBDictionary.ContainsKey(barcode) == true)
				{
					continue;
				}
				newBarcodeProductSimpleString.Makat = barcode;

				newBarcodeProductSimpleString.TypeCode = TypeMakatEnum.B.ToString();
				newBarcodeProductSimpleString.ParentMakat = makat;
				newBarcodeProductSimpleString.Name = "";
				newBarcodeProductSimpleString.PriceString = "0";
				newBarcodeProductSimpleString.SupplierCode = "";
				newBarcodeProductSimpleString.BalanceQuantityERP = record[2].Trim();

				//================Product=======================================
				int retBit = 0;
		
				retBit = newBarcodeProductSimple.ValidateError(newBarcodeProductSimpleString, base.Dtfi);  //makat error
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

				retBit = newBarcodeProductSimple.ValidateWarning(newBarcodeProductSimpleString, this.Dtfi);	 //Warning
				if (retBit != 0)
				{
					base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
				}

				//================productMakatDBDictionary=======================================
				productMakatDBDictionary[barcode] = null;

				if (iturDictionary.ContainsKey(iturCodeErp) == true)
				{
					newBarcodeProductSimple.MakatERP = iturCodeErp;
					newBarcodeProductSimple.Tag =  iturDictionary[iturCodeErp].IturCode; //IturCode
					newBarcodeProductSimple.IsUpdateERP = true;
				}
				else
				{
					newBarcodeProductSimple.MakatERP = ""; //IturCode
					newBarcodeProductSimple.Tag = "";
					newBarcodeProductSimple.IsUpdateERP = false;
				}

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
