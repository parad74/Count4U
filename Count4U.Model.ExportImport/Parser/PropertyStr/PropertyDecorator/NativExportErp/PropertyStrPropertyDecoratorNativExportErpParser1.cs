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
	public class PropertyStrPropertyDecoratorNativExportErpParser1 : IPropertyStrParser
	{
		private readonly ILog _log;
		private Dictionary<string, PropertyStr> _propertyStrDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PropertyStrPropertyDecoratorNativExportErpParser1(IServiceLocator serviceLocator,
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
				propertyStr.DomainObject = DomainObjectTypeEnum.PropertyExportErpDecorator1.ToString();
				if (recordEmpty[0].CutLength(49) != "1") continue;
				propertyStr.Code = recordEmpty[0].CutLength(49);     //	 include			 +-

				//index
				int index = -1;
				bool retindex = Int32.TryParse(recordEmpty[1].CutLength(49), out index);
				if (retindex == false) continue;
				propertyStr.TypeCode = index.ToString();     //	 index

				// PropertyName
				string columnName = "";
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
			//inventProductColumnName["documentheadercode"] = "DocumentHeaderCode";
			iturAnalyzesСolumnName["location.name"] = "LocationName";
			iturAnalyzesСolumnName["itur.number"] = "Itur_Number";
			iturAnalyzesСolumnName["itur.erpiturcode"] = "ERPIturCode";
			iturAnalyzesСolumnName["itur.locationcode"] = "LocationCode";
			iturAnalyzesСolumnName["itur.numberprefix"] = "Itur_NumberPrefix";
			iturAnalyzesСolumnName["itur.numbersufix"] = "Itur_NumberSufix";
			iturAnalyzesСolumnName["itur.name"] = "IturName";
			iturAnalyzesСolumnName["inventproduct.iturcode"] = "IturCode";
			iturAnalyzesСolumnName["documentheader.workerguid"] = "Doc_WorkerGUID";
			iturAnalyzesСolumnName["documentheader.name"] = "Doc_Name";
			iturAnalyzesСolumnName["inventproduct.documentheadercode"] = "DocumentHeaderCode";
			iturAnalyzesСolumnName["inventproduct.documentcode"] = "DocumentCode";
			iturAnalyzesСolumnName["inventproduct.makat"] = "Makat";
			iturAnalyzesСolumnName["inventproduct.inputtypecode"] = "InputTypeCode";
			iturAnalyzesСolumnName["inventproduct.barcode"] = "Barcode";
			iturAnalyzesСolumnName["inventproduct.modifydate"] = "ModifyDate";
			iturAnalyzesСolumnName["inventproduct.serialnumber"] = "SerialNumber";
			iturAnalyzesСolumnName["inventproduct.imputtypecodefrompda"] = "ImputTypeCodeFromPDA";
			iturAnalyzesСolumnName["inventproduct.productname"] = "ProductName";
			iturAnalyzesСolumnName["inventproduct.sectionnum"] = "SectionNum";
			iturAnalyzesСolumnName["inventproduct.typemakat"] = "TypeMakat";
			iturAnalyzesСolumnName["inventproduct.sectioncode"] = "SectionCode";
			iturAnalyzesСolumnName["inventproduct.suppliercode"] = "SupplierCode";
			iturAnalyzesСolumnName["inventproduct.unittypecode"] = "UnitTypeCode";
			iturAnalyzesСolumnName["inventproduct.subsectioncode"] = "SubSessionCode";
			iturAnalyzesСolumnName["product.price"] = "Price";
			iturAnalyzesСolumnName["product.pricebuy"] = "PriceBuy";
			iturAnalyzesСolumnName["product.pricesale"] = "PriceSale";
			iturAnalyzesСolumnName["product.priceextra"] = "PriceExtra";
			iturAnalyzesСolumnName["product.typecode"] = "TypeCode";
			iturAnalyzesСolumnName["product.fromcatalogtype"] = "FromCatalogType";
			iturAnalyzesСolumnName["section.name"] = "SectionName";
			iturAnalyzesСolumnName["product.countinparentpack"] = "CountInParentPack";
			iturAnalyzesСolumnName["ituranalyzes.quantitydifference"] = "QuantityDifference";
			iturAnalyzesСolumnName["ituranalyzes.quantityedit"] = "QuantityEdit";
			iturAnalyzesСolumnName["ituranalyzes.quantityoriginal"] = "QuantityOriginal";
			iturAnalyzesСolumnName["ituranalyzes.valuebuydifference"] = "ValueBuyDifference";
			iturAnalyzesСolumnName["ituranalyzes.valuebuyedit"] = "ValueBuyEdit";
			iturAnalyzesСolumnName["ituranalyzes.valuebuyoriginal"] = "ValueBuyQriginal";
			iturAnalyzesСolumnName["inventproduct.quantityinpackedit"] = "QuantityInPackEdit";
			iturAnalyzesСolumnName["inventproduct.familycode"] = "FamilyCode";
			iturAnalyzesСolumnName["family.familyname"] = "FamilyName";
			iturAnalyzesСolumnName["family.familytype"] = "FamilyType";
			iturAnalyzesСolumnName["family.familysize"] = "FamilySize";
			iturAnalyzesСolumnName["family.familyextra1"] = "FamilyExtra1";
			iturAnalyzesСolumnName["family.familyextra2"] = "FamilyExtra2";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr1"] = "IPValueStr1";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr2"] = "IPValueStr2";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr3"] = "IPValueStr3";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr4"] = "IPValueStr4";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr5"] = "IPValueStr5";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr6"] = "IPValueStr6";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr7"] = "IPValueStr7";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr8"] = "IPValueStr8";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr9"] = "IPValueStr9";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr10"] = "IPValueStr10";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr11"] = "IPValueStr11";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr12"] = "IPValueStr12";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr13"] = "IPValueStr13";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr14"] = "IPValueStr14";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr15"] = "IPValueStr15";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr16"] = "IPValueStr16";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr17"] = "IPValueStr17";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr18"] = "IPValueStr18";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr19"] = "IPValueStr19";
			iturAnalyzesСolumnName["inventproduct.ipvaluestr20"] = "IPValueStr20";
			iturAnalyzesСolumnName["inventproduct.ipvaluefloat1"] = "IPValueFloat1";
			iturAnalyzesСolumnName["inventproduct.ipvaluefloat2"] = "IPValueFloat2";
			iturAnalyzesСolumnName["inventproduct.ipvaluefloat3"] = "IPValueFloat3";
			iturAnalyzesСolumnName["inventproduct.ipvaluefloat4"] = "IPValueFloat4";
			iturAnalyzesСolumnName["inventproduct.ipvaluefloat5"] = "IPValueFloat5";
			iturAnalyzesСolumnName["inventproduct.ipvalueint1"] = "IPValueInt1";
			iturAnalyzesСolumnName["inventproduct.ipvalueint2"] = "IPValueInt2";
			iturAnalyzesСolumnName["inventproduct.ipvalueint3"] = "IPValueInt3";
			iturAnalyzesСolumnName["inventproduct.ipvalueint4"] = "IPValueInt4";
			iturAnalyzesСolumnName["inventproduct.ipvalueint5"] = "IPValueInt5";
			iturAnalyzesСolumnName["inventproduct.ipvaluebit1"] = "IPValueBit1";
			iturAnalyzesСolumnName["inventproduct.ipvaluebit2"] = "IPValueBit2";
			iturAnalyzesСolumnName["inventproduct.ipvaluebit3"] = "IPValueBit3";
			iturAnalyzesСolumnName["inventproduct.ipvaluebit4"] = "IPValueBit4";
			iturAnalyzesСolumnName["inventproduct.ipvaluebit5"] = "IPValueBit5";

			return iturAnalyzesСolumnName;
		}
	}



}
