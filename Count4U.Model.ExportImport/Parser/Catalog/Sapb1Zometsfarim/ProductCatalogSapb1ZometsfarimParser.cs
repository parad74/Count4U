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
	public class ProductCatalogSapb1ZometsfarimParser : BaseProductCatalogParser, IProductSimpleParser
	{

		public ProductCatalogSapb1ZometsfarimParser(IServiceLocator serviceLocator,
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
			string separatorParms = parms.GetStringValueFromParm(ImportProviderParmEnum.Delimiter);
			if (string.IsNullOrWhiteSpace(separatorParms) == false) separator = separatorParms; 
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

			int i = 0;
			foreach (String row in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				i++;
				if (string.IsNullOrWhiteSpace(row) == true) { continue; }

				string temp = row.Trim('"');

				temp = temp.Replace('"', '%');
				temp = temp.Replace("%,%", "~");
				string[] recordSplit = temp.Split('~');
				if (recordSplit == null) continue;

				if (recordSplit.Length < 4)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, row));
					continue;
				}

				String[] recordEmpty = { "", "", "", "", "", "", "", "", "", "", "", "", };

				int count = 12;
				if (recordSplit.Count() < 12)
				{
					count = recordSplit.Count();
				}

				for (int j = 0; j < count; j++)
				{
					string trim = recordSplit[j].Trim();
					trim = trim.Replace('%', '"');
					recordEmpty[j] = trim;
				}
				//Field1: Item Code							  0
				//Field2: Barcode1							   1
				//Field3: Barcode2							  2
				//Field4: Item Name 						3
				//Field5: SupplierCode 						4
				//Field6: SupplierName 						5
				//Field7: SectionID							   6
				//Field8: SectionName						 7
				//Field9: SubSectionID					   8
				//Field10: SubSectionName				9
				//Field11: PriceSell							10
				//Field12: PriceBuy							   11


				string makat = recordEmpty[0].Trim();
				if (string.IsNullOrWhiteSpace(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, recordEmpty.JoinRecord(separator)));
					continue;
				}

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
					continue;
				}
				//===========Product=======================================
				//Field1: Item Code							  0
				//Field2: Barcode1							   1
				//Field3: Barcode2							  2
				//Field4: Item Name 						3
				//Field5: SupplierCode 						4
				//Field6: SupplierName 						5
				//Field7: SectionID							   6
				//Field8: SectionName						 7
				//Field9: SubSectionID					   8
				//Field10: SubSectionName				9
				//Field11: PriceSell							10
				//Field12: PriceBuy							   11


				newProductSimpleString.Makat = makat;
				string name = recordEmpty[3].ReverseDosHebrew(invertLetter, rt2lf);
					
				name = name.Replace(oldCh, '%');
				name = name.Replace("%", "");
		
				newProductSimpleString.Name = name;
				newProductSimpleString.TypeCode = TypeMakatEnum.M.ToString();
				newProductSimpleString.ParentMakat = "";
				//newProductSimpleString.UnitTypeCode = record[2].Trim();
				newProductSimpleString.BalanceQuantityERP = "0";

				//Field11: PriceSell							10
				//Field12: PriceBuy							   11

				newProductSimpleString.PriceSale = "0";
				if (string.IsNullOrWhiteSpace(recordEmpty[10]) == false)
				{
					newProductSimpleString.PriceSale = recordEmpty[10];
				}

				newProductSimpleString.PriceBuy = "0";
				if (string.IsNullOrWhiteSpace(recordEmpty[11]) == false)
				{
					newProductSimpleString.PriceBuy = recordEmpty[11];
				}

				//Field5: SupplierCode 						4
				newProductSimpleString.SupplierCode = "";
				if (string.IsNullOrWhiteSpace(recordEmpty[4]) == false)
				{
					newProductSimpleString.SupplierCode = recordEmpty[4];//.LeadingZero3(); 
				}

				//Field7: SectionCode							   6
				//Field9: SubSectionCode					   8
				newProductSimpleString.SectionCode = "";
				if (string.IsNullOrWhiteSpace(recordEmpty[8]) == true)			 //SubSectionCode	empty
				{
					if (string.IsNullOrWhiteSpace(recordEmpty[6]) == false)		  //SectionCode	  not empty
					{
						newProductSimpleString.SectionCode = recordEmpty[6];//.LeadingZero3(); 
					}
				}
				else
				{
					newProductSimpleString.SectionCode = recordEmpty[8];//.LeadingZero3(); 	//SubSectionCode	  not empty
				}

				int retBit = newProductSimple.ValidateError(newProductSimpleString, this.Dtfi);
				if (retBit != 0)  //Error
				{
					base.ErrorBitList.Add(new BitAndRecord
					{
						Bit = retBit,
						Record = recordEmpty.JoinRecord(separator),
						ErrorType = MessageTypeEnum.Error
					});
					continue;
				}

				retBit = newProductSimple.ValidateWarning(newProductSimpleString, this.Dtfi);
  				if (retBit != 0)
				{
					base.ErrorBitList.Add(new BitAndRecord { Bit = retBit, Record = recordEmpty.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
				}

				productMakatDBDictionary[makat] = null;  //newProductSimple.Makat,
				newProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
				newProductSimple.InputTypeCode = InputTypeCodeEnum.B.ToString();
				newProductSimple.IsUpdateERP = false;

				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

			yield return newProductSimple;

			} //foreach
		}
	}
}
