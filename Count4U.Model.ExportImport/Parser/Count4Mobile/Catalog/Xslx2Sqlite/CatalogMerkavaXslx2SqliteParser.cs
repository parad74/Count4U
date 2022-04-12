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
using Count4U.Model.Count4Mobile;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4U;

namespace Count4U.Model.Count4Mobile
{
	public class CatalogMerkavaXslx2SqliteParser : BaseCatalogSOLiteParser, ICatalogSQLiteParser
	{

		public CatalogMerkavaXslx2SqliteParser(IServiceLocator serviceLocator,
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
		public IEnumerable<Catalog> GetCatalogs(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> catalogMakatDBDictionary,
			 List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{

			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("ExcelFileParser is null");

			
			int sheetNumberXlsx = parms.GetIntValueFromParm(ImportProviderParmEnum.SheetNumberXlsx);					// start from 1
			if (sheetNumberXlsx == 0) sheetNumberXlsx = 1;

			string sheetNameXlsx = parms.GetStringValueFromParm(ImportProviderParmEnum.SheetNameXlsx);				
			
			long k = 0;
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

		//	base.SupplierDictionary.Clear();
			base.ErrorBitList.Clear();

			string separator = ",";
			string separatorParms = parms.GetStringValueFromParm(ImportProviderParmEnum.Delimiter);
			if (string.IsNullOrWhiteSpace(separatorParms) == false) separator = separatorParms; 
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}
	
			MaskPackage maskPackage = parms.GetMaskPackageFromParms();
			//bool makatApplyMask = false;
			//bool barcodeApplyMask = false;
		//	GetApplyMasks(importType, maskPackage, out makatApplyMask, out barcodeApplyMask);
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
				countExcludeFirstString,
				sheetNameXlsx,
				sheetNumberXlsx))
			{
				//Field1 goes into Makat		    0
				//Field2 goes into Name			1
				//Field3 goes into ItemType	    2
 
				if (record == null) continue;

				if (record.Length < 3)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}
		
				string makat = record[0].Trim();
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
					continue;
				}

				//Catalog newProductSimpleString = new Catalog();
				Catalog newProductSimple = new Catalog();
		
					//	newProductSimpleString.MakatOriginal = makat;

				//if (makatApplyMask == true)
				//{
				//	makat = maskPackage.MakatMaskTemplate.FormatString(
				//		makat, maskPackage.MakatMaskRecord.Value);
				//}


				if (catalogMakatDBDictionary.ContainsKey(makat) == true)
				{
					base.Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatExistInDB, record.JoinRecord(separator)));
					continue;
				}
				//===========Product=======================================
				//Field1 goes into Makat		    0
				//Field2 goes into Name			1
				//Field3 goes into ItemType	    2

				newProductSimple.Uid = Guid.NewGuid().ToString();
				newProductSimple.ItemCode = makat;
				string name = record[1].ReverseDosHebrew(invertLetter, rt2lf);
				newProductSimple.ItemName = name;
				newProductSimple.UnitTypeCode = record[2].Trim();
					
				//name = name.Replace(oldCh, '%');
				//name = name.Replace("%", "");
				//newProductSimpleString.Name = name;
				//newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				//newProductSimpleString.ParentMakat = "";
				//newProductSimpleString.UnitTypeCode = record[2].Trim();
				//newProductSimpleString.BalanceQuantityERP = "0";

				//newProductSimpleString.PriceBuy = "0";
				//if (record.Length > 2) //3,4
				//{
				//	string priceBuy = record[2].Trim(' ');
				//	newProductSimpleString.PriceBuy = priceBuy;
				//}

				//newProductSimpleString.PriceSale = "0";
				//if (record.Length > 3)
				//{
				//	string priceSale = record[3].Trim(' ');
				//	newProductSimpleString.PriceSale = priceSale;
				//}

				//int retBit = newProductSimple.ValidateError(newProductSimpleString, this.Dtfi);
				//if (retBit != 0)  //Error
				//{
				//	base.ErrorBitList.Add(new BitAndRecord
				//	{
				//		Bit = retBit,
				//		Record = record.JoinRecord(separator),
				//		ErrorType = MessageTypeEnum.Error
				//	});
				//	continue;
				//}


				//retBit = newProductSimple.ValidateWarning(newProductSimpleString, this.Dtfi);
				//if (retBit != 0)
				//{
				//	base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
				//}

				catalogMakatDBDictionary[makat] = null;  //newProductSimple.Makat,
				//newProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
				//newProductSimple.InputTypeCode = InputTypeCodeEnum.B.ToString();

				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

			yield return newProductSimple;

			} //foreach
		}

		
	}
}
