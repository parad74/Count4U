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
	public class ProductCatalogAS400JaforaParser : BaseProductCatalogParser, IProductSimpleParser
	{
		public ProductCatalogAS400JaforaParser(IServiceLocator serviceLocator,
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

            bool onlyWithMask = parms.GetBoolValueFromParm(ImportProviderParmEnum.OnlyWithMask);


			long k = 0;
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			base.SupplierDictionary.Clear();
			base.ErrorBitList.Clear();

			string separator = SeparatorField.Tab;
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

				if (record.Length < 3)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				//	Field1: Item Code			//0
				//Field2: UnitTypeCode		//1
				//Field3: Name					//2
				//Field4: ERPIturExpected //3

			
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

				string productName = record[2].ReverseDosHebrew(invertLetter, rt2lf);

                if (onlyWithMask == true)  //Only xxxx-xxxx-xx or xxxx-xxxx
                {
                    if (IsOnlyWithMask (makat) == false)
                        continue;
                }
				newMakatProductSimpleString.Makat = makat;
				newMakatProductSimpleString.Name = productName;
				newMakatProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				//string priceSalle = record[7].Trim();
				//priceSalle = String.IsNullOrEmpty(priceSalle) ? "0" : priceSalle;
				//newMakatProductSimpleString.PriceSale = priceSalle;

				//if (withQuantityERP == true)
				//{
				//	string quantityERP = record[2].Trim();
				//	quantityERP = String.IsNullOrEmpty(quantityERP) ? "0" : quantityERP;
				//	newMakatProductSimpleString.BalanceQuantityERP = quantityERP;
				//}			

				//================Product=======================================
				int retBit = newMakatProductSimple.ValidateError(newMakatProductSimpleString, base.Dtfi);  //makat error
				if (retBit != 0)  //Error
				{
					base.ErrorBitList.Add(new BitAndRecord {Bit = retBit,	Record = record.JoinRecord(separator),	ErrorType = MessageTypeEnum.Error	});
					continue;
				}

				retBit = newMakatProductSimple.ValidateWarning(newMakatProductSimpleString, base.Dtfi);	 //Warning
				if (retBit != 0)
				{
					base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
				}
				//========	  productMakatDBDictionary ==========
				productMakatDBDictionary[makat] = null;
				newMakatProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
				newMakatProductSimple.InputTypeCode = InputTypeCodeEnum.B.ToString();
				newMakatProductSimple.UnitTypeCode = record[1].Trim();
				if (record.Length > 3)
				{
					newMakatProductSimple.IturCodeExpected = record[3].Trim();
				}

				if (record.Length > 4)
				{
					string sectionType = record[4].Trim();
					string sectionCode = "";
					if (string.IsNullOrWhiteSpace(sectionType) == true) sectionCode = "0";
					else if (sectionType == "X") sectionCode = "1";
					newMakatProductSimple.SectionCode = sectionCode;
				}

				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

				yield return newMakatProductSimple;
			} //foreach
		}

        private bool IsOnlyWithMask (string makat) 
        {
            //Only xxxx-xxxx-xx or xxxx-xxxx
            string[] parts = makat.Split('-');
            int len = parts.Length;
            if (len < 2 || len > 3) return false;
            
            if (parts[0].Length != 4) return false;
            if (parts[1].Length != 4) return false;
            if (len == 3)if (parts[2].Length != 2) return false;

            return true;
        }
	}
}
