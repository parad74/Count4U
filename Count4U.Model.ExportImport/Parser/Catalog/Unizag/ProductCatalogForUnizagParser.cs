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
	public class ProductCatalogForUnizagParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogForUnizagParser(IServiceLocator serviceLocator,
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

			//decimal codeCh = 0x200E;//33286;//8206;//(decimal)ch;	  //"\u200E"
			decimal codeCh = 0x200F;
			char oldCh = (char)codeCh;
			//string LTRMark = "\u200E";
			//string RTMark = "\u200F";

		
			foreach (String rec in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				string rec1 = rec.Replace(oldCh, '%');
				rec1 = rec1.Replace("%", "");
				String[] record = rec1.Split(separators, StringSplitOptions.None);
				if (record == null) continue;

				if (record.Length < 3)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}
				//	 countExcludeFirstString = 1
				//2,добешвш бмзорйд, 3.99
				//0,	1							2

				ProductSimpleString newProductSimpleString = new ProductSimpleString();
				Product newProductSimple = new Product();
				string makat = record[0].Trim();

				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
					continue;
				}

				newProductSimpleString.MakatOriginal = makat;
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
				//Makat appears more than once on items.asp file: 
				//Ignore the second Makat, 
				//write into the log file the line which contains the second appearance of the exists Makat 
				//string productName = record[1].ReverseDosHebrew(invertLetter, rt2lf);
				//string productName = record[1];

				string price = record[2].Trim();
				newProductSimpleString.PriceSale = String.IsNullOrEmpty(price) ? "0" : price;
				newProductSimpleString.PriceString = String.IsNullOrEmpty(price) ? "0" : price;

				//string productName1 = record[1].ResaveText(encoding);
				string productName = record[1].Trim();
				
				//string productName1 = productName.TrimEnd(')');
				//if (productName.CompareTo(productName1) != 0)
				//{
				//    productName = @"(" + productName1;
				//}

				//string productName = rec.Replace(record[0].Trim(), "");
				//string productName12 = productName1.TrimStart(" ,".ToCharArray());
				//string productName2 = productName12.Replace(record[2].Trim(), "");
				//string productName = productName2.Replace(",", "");
				//productName = productName.Replace(oldCh, '%');
				//productName = productName.Replace("%", "");
				newProductSimpleString.Name = productName.ReverseDosHebrew(invertLetter, rt2lf);
				newProductSimpleString.Makat = makat;

				//char[] charSet1 = productName.ToCharArray();
				//for (int i = 0; i <= charSet1.Length - 1; i++)
				//{
				//    char ch1 = charSet1[i];
				//}

				//string[] _productName = new string[5];
				//_productName[0] = new string(charSet1);
				//_productName[1] = LTRMark + new string(charSet1);
				//_productName[2] = LTRMark + new string(charSet1) + LTRMark;
				//_productName[3] = LTRMark + productName + LTRMark;
				//_productName[4] = LTRMark + productName;
				//for (int i = 0; i <= 4; i++)
				//{
				//    string str = _productName[i];
				//}
   			
				//productName = productName.Replace(oldCh, '%');
				//productName = productName.Replace("%", "");
				//char[] charSet2 = productName.ToCharArray();
				//for (int i = 0; i <= charSet2.Length - 1; i++)
				//{
				//    char ch2 = charSet2[i];
				//}

				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
			
				int retBit = newProductSimple.ValidateError(newProductSimpleString, this.Dtfi);
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

				retBit = newProductSimple.ValidateWarning(newProductSimpleString, this.Dtfi);
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
