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
	public class ProductCatalogTafnitMatrixParser : BaseProductCatalogParser, IProductSimpleParser
	{
		public ProductCatalogTafnitMatrixParser(IServiceLocator serviceLocator,
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

			string separator = ";";
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

			string pathDB = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			bool withQuantityERP = parms.GetBoolValueFromParm(ImportProviderParmEnum.WithQuantityERP);

			ISectionRepository sectionRepository = ServiceLocator.GetInstance<ISectionRepository>();
			Dictionary<string, Section> sectionDictionary = sectionRepository.GetSectionDictionary_NameKey(pathDB);
			IFamilyRepository familyRepository = ServiceLocator.GetInstance<IFamilyRepository>();
			Dictionary<string, Family> familyDictionary = familyRepository.GetFamilyDictionary_DescriptionKey(pathDB);

			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding,
				separators,
				countExcludeFirstString))
			{

				if (record == null) continue;

				if (record.Length < 7)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				//Field1: Item Code						0
				//Field2: Item Name						1
				//Field3: Barcode							2
				//Field4: PriceBuy 							3
				//Field5: PriceSell 							4
				//Field6: QuantityExpectedERP 	5					
				//Field7: SectionName 					6
				//Field8: SubSection1						7
				//Field9: SubSection2					8
				//Field10: SubSection3					9
				//9400111  ;сйеи двйе - дблшдм хто шеичши;6934510501211;15.7675;39.90;6;нйтецтц;дрйгт дчйшеиеое дшйцй;хт йчзщо;амм


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

				string fullCode = record[6].Trim().ReverseDosHebrew(invertLetter, rt2lf) + "#" + record[7].Trim().ReverseDosHebrew(invertLetter, rt2lf)
				+ "#" + record[8].Trim().ReverseDosHebrew(invertLetter, rt2lf) + "#" + record[9].Trim().ReverseDosHebrew(invertLetter, rt2lf);
				string familyCode = "none";
				string familyName = "";
				if (familyDictionary.ContainsKey(fullCode) == true)
				{
					Family family = familyDictionary[fullCode];
					familyCode = family.FamilyCode;
					familyName = family.Name;
				}

				string sectionName = record[6].Trim().ReverseDosHebrew(invertLetter, rt2lf);
				string sectionCode = "none";
				if (sectionDictionary.ContainsKey(sectionName) == true)
				{
					Section section = sectionDictionary[sectionName];
					sectionCode = section.SectionCode;
				}
			
				

				string productName = record[1].ReverseDosHebrew(invertLetter, rt2lf);
				newMakatProductSimpleString.Makat = makat;
				newMakatProductSimpleString.Name = productName;
				newMakatProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newMakatProductSimpleString.PriceBuy = record[3].Trim();
				newMakatProductSimpleString.PriceSale = record[4].Trim();
				newMakatProductSimpleString.BalanceQuantityERP = "0";
				if (withQuantityERP == true)
				{
					newMakatProductSimpleString.BalanceQuantityERP = record[5].Trim();
				}
				newMakatProductSimpleString.SectionCode = sectionCode;
				newMakatProductSimpleString.FamilyCode = familyCode;
				newMakatProductSimpleString.Family = familyName;
				
				//Field4: PriceBuy 							3
				//Field5: PriceSell 							4
				//Field6: QuantityExpectedERP 	5					
				//Field7: SectionName 					6
				//================Product=======================================
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

				//========	  productMakatDBDictionary ==========
				productMakatDBDictionary[makat] = null;

				newMakatProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
				newMakatProductSimple.InputTypeCode = InputTypeCodeEnum.B.ToString();

				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

				yield return newMakatProductSimple;
				//======Makat ====== Product ========
			} //foreach
		}
	}
}
