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
	public class ProductCatalogForYarpaWindowsParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogForYarpaWindowsParser(IServiceLocator serviceLocator,
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

			string separator = ",";
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

			decimal codeCh = 0x200E;//33286;//8206;//(decimal)ch;	  //"\u200E"
			char oldCh = (char)codeCh;

			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding,
				separators,
				countExcludeFirstString))
			{
//P,   108,OUT ,  ,VET.CALCIUM 150 TAB      ,V,     29.15,   0,  0,  0,    0,  0,  0,       18,    0,7290004455637,
//P,   109,DIP ,  ,PLAK CON.REF.ORTHO OD17  ,D,      30.1,   0,  0,  0,    0,  0,  0,     19.5,    0,4210201151036,

				if (record == null) continue;

				if (record.Length < 14)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}
		
				string makat = record[1].Trim();
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
					continue;
				}
		

						
				//ItemCode,	ItemName,  PriceSale,  QuantityInPack, PriceBuy
				//1				, 4					, 6				,10						,13
	
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
				//ItemCode,	ItemName,  PriceSale,  QuantityInPack, PriceBuy
				//1				, 4					, 6				,10					,13

				newProductSimpleString.Makat = makat;
				string name = record[4].ReverseDosHebrew(invertLetter, rt2lf);
				//char[] charSet1 = name.ToCharArray();
				//for (int i = 0; i <= charSet1.Length - 1; i++)
				//{
				//    char ch1 = charSet1[i];
				//}
				
				name = name.Replace(oldCh, '%');
				name = name.Replace("%", "");
				//char[] charSet2 = name.ToCharArray();
				//for (int i = 0; i <= charSet2.Length - 1; i++)
				//{
				//    char ch2 = charSet2[i];
				//}

				newProductSimpleString.Name = name;
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				//newProductSimpleString.UnitTypeCode = record[2].Trim();
				newProductSimpleString.BalanceQuantityERP = "0";
				//if (withQuantityERP == true)
				//{
				//    newProductSimpleString.BalanceQuantityERP = record[3].FormatComa2();
				//}
				//newProductSimpleString.SectionCode = record[4].Trim();
				string priceSale = record[6].Trim(' ');
				newProductSimpleString.PriceSale = priceSale;

				string countInParentPack = record[10];
				newProductSimpleString.CountInParentPack = countInParentPack;
			
				string priceBuy = record[13].Trim(' ');
				newProductSimpleString.PriceBuy = priceBuy;
				newProductSimpleString.PriceString = String.IsNullOrEmpty(priceBuy) ? "0" : priceBuy;

				//newProductSimpleString.SectionCode = "";
				//newProductSimpleString.SupplierCode = "";
				//newProductSimpleString.UnitTypeCode = "";
		
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

			yield return newProductSimple;

			} //foreach
		}
	}
}
