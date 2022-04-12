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
	public class ProductCatalogOrenOriginalsParser2 : BaseProductCatalogParser, IProductSimpleParser
	{
		public ProductCatalogOrenOriginalsParser2(
			IServiceLocator serviceLocator, ILog log) 
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
			if (fileXlsx == true) 
			{
				fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); 
			}
			else
			{
				fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); 
			}
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
			Dictionary<string, Itur> erpIturDictionary = new Dictionary<string, Itur>();

			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			if (string.IsNullOrWhiteSpace(dbPath) == false)
			{
				erpIturDictionary = iturRepository.GetERPIturDictionary(dbPath);
			}
			//CatalogItemNameStart = 1,  //OrenWarehouseName not used
			//CatalogItemNameEnd = 10,
			//	CatalogItemCodeStart = 11,
			//	CatalogItemCodeEnd = 30,
			//	QuantityERPStart = 31,
			//	QuantityERPEnd = 38,
			//	QuantityInPackStart = 39, //ERP Itur Code 
			//	QuantityInPackEnd = 100 // заменить на длину строки
			//OrenWarehouseName(Col 1-10) not used
			//Item Code (Col 11-30)  Drop the "-" & If ".5" change to "V" – after manipulate goes into Makat
			//Quantity Edit (Col 31-38)
			//ERP Itur Code (Col 39-??)*

			List<Point> startEndSubstring = new List<Point>();

			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogItemNameStart,
				End = catalogParserPoints.CatalogItemNameEnd,
				Length = catalogParserPoints.CatalogItemNameEnd - catalogParserPoints.CatalogItemNameStart + 1
			});	 	//	 (col 1-10)

			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogItemCodeStart,
				End = catalogParserPoints.CatalogItemCodeEnd,
				Length = catalogParserPoints.CatalogItemCodeEnd - catalogParserPoints.CatalogItemCodeStart + 1
			});	 	//	 (col 11-30)

			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.QuantityERPStart,
				End = catalogParserPoints.QuantityERPEnd,
				Length = catalogParserPoints.QuantityERPEnd - catalogParserPoints.QuantityERPStart + 1
			});	//		(Col 31-38)

			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.QuantityInPackStart,
				End = catalogParserPoints.QuantityInPackEnd,
				Length = catalogParserPoints.QuantityInPackEnd - catalogParserPoints.QuantityInPackStart + 1
			});	//		 (Col 39-??)*

			int count = startEndSubstring.Count;


			foreach (String rec in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				if (string.IsNullOrWhiteSpace(rec) == true) { continue; }

				int len = rec.Length;

				if (len < catalogParserPoints.CatalogItemCodeStart)
				{
					continue;
				}

				String[] record = { "", "", "","" };
				//Field2: OrenWarehouseName 0
				//Field2: Item Code					1 
				//Field3: Quantity Expected Per Item code in Itur code ERP ***		3 // new2
				//Field4: Itur code ERP				2// new3

				startEndSubstring[3].End = Math.Min (startEndSubstring[3].End , len-1);
				startEndSubstring[3].Length = startEndSubstring[3].End - startEndSubstring[3].Start + 1;

				for (int i = 0; i < 3; i++) //!!count 
				{
					if (startEndSubstring[i].End < len) 
					{
						record[i] = rec.Substring(startEndSubstring[i].Start, startEndSubstring[i].Length);
					}
				}

				record[3] = rec.Substring(startEndSubstring[3].Start);
				
				string makat = record[1].Trim();
						if (string.IsNullOrWhiteSpace(makat) == true)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatIsEmpty, record.JoinRecord(separator)));
					continue;
				}

				if (makatApplyMask == true)
				{
					makat = maskPackage.MakatMaskTemplate.FormatString(
						makat, maskPackage.MakatMaskRecord.Value);
				}

				makat = makat.Replace("-",String.Empty);
				makat = makat.Replace(".5", "V");

				if (makat.Contains("110002473") == true)
				{
					makat = makat;
				}
				string iturCodeERP = record[3].Trim();
				if (string.IsNullOrWhiteSpace(iturCodeERP) == true)
				{
					continue;
				}

			
				string iturCode = iturCodeERP;
				if (erpIturDictionary.ContainsKey(iturCodeERP) == true)
				{
					iturCode = erpIturDictionary[iturCodeERP].IturCode;
				}

				//string barcode = makat + "##" + iturCodeERP;
				string barcode = makat + "##" + iturCode;

				ProductSimpleString newProductSimpleString = new ProductSimpleString();
				Product newProductSimple = new Product();
			
				newProductSimpleString.MakatOriginal = barcode;
				

				if (makat.ToUpper() == barcode.ToUpper())
				{
					continue;
				}

				if (productMakatDBDictionary.ContainsKey(barcode) == true)
				{
					Log.Add(MessageTypeEnum.Warning, String.Format(ParserFileErrorMessage.BarcodeExistInDB, record.JoinRecord(separator)));
					continue;
				}
				//===========Product=======================================
	
				newProductSimpleString.Makat = barcode;
				newProductSimpleString.MakatOriginal = barcode;
				newProductSimpleString.Name = "";
				newProductSimpleString.TypeCode = TypeMakatEnum.B.ToString();
				newProductSimpleString.ParentMakat = makat;

				newProductSimpleString.BalanceQuantityERP = "0";
				if (record.Length > 2)
				{
					string balanceQuantityERP = record[2].Trim(' ');
					newProductSimpleString.BalanceQuantityERP = balanceQuantityERP;
				}
				//newProductSimpleString.PriceString = "0.0";
				//newProductSimpleString.PriceSale = "0.0";
				//newProductSimpleString.PriceBuy = "0.0";
				//string countInParentPack = record[1];
				//newProductSimpleString.CountInParentPack = countInParentPack;
				//newProductSimpleString.BalanceQuantityERP = "0.0";
				//newProductSimpleString.SupplierCode = "";
				//newProductSimpleString.SectionCode = "";
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

				productMakatDBDictionary[barcode] = null;			   //newProductSimple.Makat
				newProductSimple.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
				newProductSimple.InputTypeCode = InputTypeCodeEnum.B.ToString();
				newProductSimple.IsUpdateERP = true;					//!!!
				newProductSimple.Tag = iturCodeERP + "##" + iturCode;
				newProductSimple.Name = iturCodeERP;
	
				if (cancellationToken.IsCancellationRequested == true) break;
				k++;
				if (k % 100 == 0) countAction(k);

				yield return newProductSimple;

			} //foreach
		}

		

	
	}

	
}
