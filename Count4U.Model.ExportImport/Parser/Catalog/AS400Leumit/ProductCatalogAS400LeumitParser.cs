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
	public class ProductCatalogAS400LeumitParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogAS400LeumitParser(IServiceLocator serviceLocator,
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

			decimal codeCh44 = 44; //,
			decimal codeCh34 = 34;	//"
			char oldCh44 = (char)codeCh44;
			char oldCh34 = (char)codeCh34;
				
			//foreach (String[] record in this.FileParser.GetRecords(fromPathFile,
			//	encoding,
			//	separators,
			//	countExcludeFirstString))
			foreach (String rec in fileParser.GetRecords(fromPathFile,
					encoding,
					countExcludeFirstString))
			{
				if (rec == null) continue;

				string rec1 = rec.Replace(oldCh34, '^'); // " =>^


				//String[] record = rec1.Split("^".ToCharArray(), StringSplitOptions.None);

				String[] record = rec1.Split(",".ToCharArray(), StringSplitOptions.None);
				string makat = record[0].Trim();
				string nameRecord1 = record[1];

				if (record.Length < 7)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				// проверка на неправильную строку
				string open = @",^";
				string close = @"^,";
				int indexOpen = rec1.IndexOf(open);
				
				if (indexOpen > -1)
				{
					string rec2 = rec1.Replace(open, "$"); //замена левой скобки   ,^  =>  $
					string rec3 = rec2.Replace(close, "|"); //замена правой скобки   ^, =>  |
					//делим на 2 подстроки до открывающейся скобки и после
					String[] record3 = rec3.Split("$".ToCharArray(), StringSplitOptions.None);
					string rec4 = "";
					if (record3.Length > 1) rec4 = record3[1];  // после $

					//делим на 2 подстроки до открывающейся скобки и после
					String[] record4 = rec4.Split("|".ToCharArray(), StringSplitOptions.None);
					if (record4.Length > 0)
					{
						nameRecord1 = record4[0].Replace('^','"');  // name после $ и до |
						record[1] = nameRecord1;
					}

					if (record4.Length > 1)
					{
						string rec5 = record4[1];  // name после |  
						// должны быть записи начиная с 3 
						String[] record5 = rec5.Split(",".ToCharArray(), StringSplitOptions.None);
						if (record5.Length > 0) record[2] = record5[0];
						if (record5.Length > 1) record[3] = record5[1];
						if (record5.Length > 2) record[4] = record5[2];
						if (record5.Length > 3) record[5] = record5[3];
						if (record5.Length > 4) record[6] = record5[4];
						//if (record5.Length > 5) record[7] = record5[5];
					}
				}


			
				//D33814,HOL.DRAIN.POUCH 70MM 3814,PL10,042545,5390166010605,56.010,10,5.601
				//Field1: Item Code				0
				//Field2: Name						1
				//Field3: Name2					2
				//Field6: Price Buy				    5
				//Field7: Quantity In Pack 	6
			

				ProductSimpleString newProductSimpleString = new ProductSimpleString();
				Product newProductSimple = new Product();

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
				//Field1: Item Code				0
				//Field2: Name						1
				//Field3: Name2					2
				//Field6: Price Buy				    5
				//Field7: Quantity In Pack 	6

				string productName = record[1].ReverseDosHebrew(invertLetter, rt2lf);
				string productName1 = record[2].ReverseDosHebrew(invertLetter, rt2lf); 
				newProductSimpleString.Makat = makat;
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				newProductSimpleString.Name = productName + "," + productName1;
				newProductSimpleString.PriceBuy = record[5].Trim();
				newProductSimpleString.CountInParentPack = record[6].Trim();


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
