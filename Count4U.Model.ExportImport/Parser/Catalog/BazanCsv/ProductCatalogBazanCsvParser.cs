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
	public class ProductCatalogBazanCsvParser : BaseProductCatalogParser, IProductSimpleParser
	{
		public ProductCatalogBazanCsvParser(IServiceLocator serviceLocator,
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

			string separator = ",";
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

			foreach (String rec in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				if (rec == null) continue;
				if (string.IsNullOrWhiteSpace(rec) == true) { continue; }

				String[] record = rec.Split(',');
				String[] recordNew = new String[7]; 

				if (record.Length < 7)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				//	Field1: Site							//0
				//Field2: Item Code				//1 Makat
				//Field3: Mlai Unique				//2
				//Field4: Mlai Unique code		//3
				//Field5: Item Name				//4	Name
				//Field6: Unlimit						//5
				//Field7: Unit Type					//6 UnitType
				//Field8: Unlimit Value			//7
				//Field9: Group						//8
				//Field10: Section					//9
				//Field11: Batch						//10
				
				//=======================
				//Field1: Item Code				//0
				//Field2: Item Name				//1
				//Field3: Unit Type					//2  UnitTypeCode
				//Field4: Unit Type Name		//3  SupplierCode
				//Field5: Group						//4  Family
				//Field6: Section						//5  SectionCode
				//Field7: Serial						//6  FamilyCode
				//Item Code, Name, Unit Type Name, Serial
//Field6: Unit Type
//Field7: Group
//Field8: Section



							
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
				string productName = record[1].ReverseDosHebrew(invertLetter, rt2lf);
				productName = productName.TrimSpaceInText();
				newMakatProductSimpleString.Makat = makat;
				newMakatProductSimpleString.Name = productName;
				newMakatProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newMakatProductSimpleString.UnitTypeCode = record[2].Trim();  //Unit Type	
				newMakatProductSimpleString.SupplierCode = record[3].ReverseDosHebrew(invertLetter, rt2lf);   //Unit Type Name
				newMakatProductSimpleString.Family = record[4].Trim();				//Group
				newMakatProductSimpleString.SectionCode = record[5].Trim();  //SectionCode
				newMakatProductSimpleString.FamilyCode = record[6].Trim();   //Serial

				//Field3: Unit Type					//2  UnitTypeCode
				//Field4: Unit Type Name		//3  SupplierCode
				//Field5: Group						//4  Family
				//Field6: Section						//5  SectionCode
				//Field7: Serial						//6  FamilyCode
				//for (int i = 0; i < record.Length; i++)
				//{
				//	if (i < 11)
				//	{
				//		try { recordNew[i] = record[i]; }
				//		catch
				//		{
				//			base.Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.Error, record.JoinRecord(separator)));
				//		}
				//	}
				//}

				//if (record.Length >= 6)
				//{
				
					//record[6] = "";
				//}
				//record[1] ="";
				//record[4] = "";

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
				//string descr = recordNew.JoinRecord(",");
				//if (descr.Length > 85)
				//{
				//	descr = descr.Substring(0, 85) + ",,,,,,,,,,,";
				//}
				//newMakatProductSimple.Description = descr;
				//newMakatProductSimple.Description = record[3].Trim(); //Unit Type Name

				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

				yield return newMakatProductSimple;
			} //foreach
		}
		
	}
}
