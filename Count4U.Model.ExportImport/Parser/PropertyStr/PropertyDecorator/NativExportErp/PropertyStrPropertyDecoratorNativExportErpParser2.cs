using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class PropertyStrPropertyDecoratorNativExportErpParser2 : IPropertyStrParser
	{
		private readonly ILog _log;
		private Dictionary<string, PropertyStr> _propertyStrDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PropertyStrPropertyDecoratorNativExportErpParser2(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._propertyStrDictionary = new Dictionary<string, PropertyStr>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, PropertyStr> PropertyStrDictionary
		{
			get { return this._propertyStrDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, PropertyStr> GetPropertyStrs(string fromPathFile,
			DomainObjectTypeEnum domainObjectType,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, PropertyStr> propertyStrFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			int sheetNumberXlsx = parms.GetIntValueFromParm(ImportProviderParmEnum.SheetNumberXlsx);                    // start from 1
			if (sheetNumberXlsx == 0) sheetNumberXlsx = 1;

			string sheetNameXlsx = parms.GetStringValueFromParm(ImportProviderParmEnum.SheetNameXlsx);

			this._propertyStrDictionary.Clear();
			this._errorBitList.Clear();

			Random rnd = new Random();
			string separator = " ";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			bool invertLetter = false;
			bool rt2lf = false;
			if (parms != null)
			{
				if (encoding == Encoding.GetEncoding(862) || encoding == Encoding.GetEncoding(1255))
				{
					invertLetter = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLetters);
					rt2lf = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWords);
				}
			}

			Dictionary<string, string> _columnName = FillInventProductColumnNames();

			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString,
				sheetNameXlsx,
				sheetNumberXlsx))
			{
				if (record == null) continue;
				//DomainObject //PropertyExportErpDecorator
				//include 						//Code								 0
				//index				 	  //TypeCode						1
				//Table					   // constant InventProduct   2
				// PropertyName		   //PropertyStrCode 			3
				//Title 						// Name						4   //Option


				if (record.Length < 4)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				String[] recordEmpty = { "", "", "", "", "" };

				PropertyStr newPropertyStr = new PropertyStr();
				PropertyStr propertyStr = new PropertyStr();
				int count = 5;
				if (record.Count() < 5)
				{
					count = record.Count();
				}

				for (int i = 0; i < count; i++)
				{
					recordEmpty[i] = record[i].Trim();
				}

				//DomainObject //PropertyExportErpDecorator
				//include 						//Code								 0
				//index				 	  //TypeCode						1
				//Table					   // constant InventProduct   2
				// PropertyName		   //PropertyStrCode 			3
				//Title 						// Name						4   //Option

				//include
				propertyStr.DomainObject = DomainObjectTypeEnum.PropertyExportErpDecorator2.ToString();
				if (recordEmpty[0].CutLength(49) != "1") continue;
				propertyStr.Code = recordEmpty[0].CutLength(49);     //	 include			 +-

				//index
				int index = -1;
				bool retindex = Int32.TryParse(recordEmpty[1].CutLength(49), out index);
				if (retindex == false) continue;
				propertyStr.TypeCode = index.ToString();     //	 index

				// PropertyName
				string columnName = "";
				//	bool ret = _columnName.TryGetValue(recordEmpty[2].CutLength(49).ToLower(), out columnName);                 //	 PropertyName
				bool ret = _columnName.TryGetValue(recordEmpty[2].CutLength(49).ToLower() + "." + recordEmpty[3].CutLength(49).ToLower(), out columnName);                 //	 PropertyName

				if (ret == false) continue;
				propertyStr.PropertyStrCode = columnName;      //	 PropertyName

				//Title
				propertyStr.Name = recordEmpty[4].CutLength(49);     //	 Title

				string uid = propertyStr.PropertyStrCode.ToLower();

				int retBit = newPropertyStr.ValidateError(propertyStr);
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
				else //	Error  retBit == 0 
				{
					retBit = newPropertyStr.ValidateWarning(propertyStr); //Warning
					if (propertyStrFromDBDictionary.ContainsKey(uid) == false)
					{
						this._propertyStrDictionary[uid] = newPropertyStr;
						propertyStrFromDBDictionary[uid] = null;
					}
					if (retBit != 0)
					{
						this._errorBitList.Add(new BitAndRecord
						{
							Bit = retBit,
							Record = record.JoinRecord(separator),
							ErrorType = MessageTypeEnum.WarningParser
						});
					}
				}
			}
			return this._propertyStrDictionary;
		}



		private Dictionary<string, string> FillInventProductColumnNames()
		{
			Dictionary<string, string> iturAnalyzesСolumnName = new Dictionary<string, string>();
			iturAnalyzesСolumnName["inventproduct.makat"] = "Makat";
			iturAnalyzesСolumnName["inventproduct.barcode"] = "Barcode";
			iturAnalyzesСolumnName["inventproduct.productName"] = "ProductName";
			iturAnalyzesСolumnName["inventproduct.typemakat"] = "TypeMakat";
			iturAnalyzesСolumnName["inventproduct.sectioncode"] = "SectionCode";
			iturAnalyzesСolumnName["inventproduct.suppliercode"] = "SupplierCode";
			iturAnalyzesСolumnName["inventproduct.unittypecode"] = "UnitTypeCode";
			iturAnalyzesСolumnName["product.price"] = "Price";
			iturAnalyzesСolumnName["product.pricebuy"] = "PriceBuy";
			iturAnalyzesСolumnName["product.pricesale"] = "PriceSale";
			iturAnalyzesСolumnName["product.priceextra"] = "PriceExtra";
			iturAnalyzesСolumnName["product.fromcatalogtype"] = "FromCatalogType";
			iturAnalyzesСolumnName["section.name"] = "SectionName";
			iturAnalyzesСolumnName["supplier.name"] = "SupplierName";
			iturAnalyzesСolumnName["inventproduct.familycode"] = "FamilyCode";
			iturAnalyzesСolumnName["product.countinparentpack"] = "CountInParentPack";
			iturAnalyzesСolumnName["ituranalyzes.quantitydifference"] = "QuantityDifference";
			iturAnalyzesСolumnName["ituranalyzes.quantityedit"] = "QuantityEdit";
			iturAnalyzesСolumnName["ituranalyzes.quantityoriginal"] = "QuantityOriginal";
			iturAnalyzesСolumnName["ituranalyzes.valuebuydifference"] = "ValueBuyDifference";
			iturAnalyzesСolumnName["ituranalyzes.valuebuyedit"] = "ValueBuyEdit";
			iturAnalyzesСolumnName["ituranalyzes.valuebuyoriginal"] = "ValueBuyQriginal";
			iturAnalyzesСolumnName["ituranalyzes.quantityinpackedit"] = "QuantityInPackEdit";
			iturAnalyzesСolumnName["ituranalyzes.quantityoriginalerp"] = "QuantityOriginalERP";
			iturAnalyzesСolumnName["ituranalyzes.valueoriginalerp"] = "ValueOriginalERP";
			iturAnalyzesСolumnName["ituranalyzes.quantitydifferenceoriginalerp"] = "QuantityDifferenceOriginalERP";
			iturAnalyzesСolumnName["ituranalyzes.valuedifferenceoriginalerp"] = "ValueDifferenceOriginalERP";
			iturAnalyzesСolumnName["ituranalyzes.balancequantitypartialerp"] = "BalanceQuantityPartialERP";


			return iturAnalyzesСolumnName;
		}
	}



}
