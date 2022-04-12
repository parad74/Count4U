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
	public class PropertyStrPropertyDecoratorNativExportErpParser4 : IPropertyStrParser
	{
		private readonly ILog _log;
		private Dictionary<string, PropertyStr> _propertyStrDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PropertyStrPropertyDecoratorNativExportErpParser4(IServiceLocator serviceLocator,
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
				propertyStr.DomainObject = DomainObjectTypeEnum.PropertyExportErpDecorator4.ToString();
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
				//bool ret = _inventProductColumnName.TryGetValue(recordEmpty[2].CutLength(49).ToLower(), out columnName);                 //	 PropertyName
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
			Dictionary<string, string> _columnName = new Dictionary<string, string>();
			_columnName["inventproduct.barcode"] = "Uid";
			_columnName["inventproduct.serialnumber"] = "SerialNumberLocal";
			_columnName["inventproduct.makat"] = "ItemCode";
			_columnName["inventproduct.suppliercode"] = "SerialNumberSupplier";
			_columnName["inventproduct.quantityedit"] = "Quantity";
			_columnName["inventproduct.ipvaluestr1"] = "PropertyStr1Code";
			_columnName["propertystr.name1"] = "PropertyStr1Name";
			_columnName["inventproduct.ipvaluestr2"] = "PropertyStr2Code";
			_columnName["propertystr.name2"] = "PropertyStr2Name";
			_columnName["inventproduct.ipvaluestr3"] = "PropertyStr3Code";
			_columnName["propertystr.name3"] = "PropertyStr3Name";
			_columnName["inventproduct.ipvaluestr4"] = "PropertyStr4Code";
			_columnName["propertystr.name4"] = "PropertyStr4Name";
			_columnName["inventproduct.ipvaluestr5"] = "PropertyStr5Code";
			_columnName["propertystr.name5"] = "PropertyStr5Name";
			_columnName["inventproduct.ipvaluestr6"] = "PropertyStr6Code";
			_columnName["propertystr.name6"] = "PropertyStr6Name";
			_columnName["inventproduct.ipvaluestr7"] = "PropertyStr7Code";
			_columnName["propertystr.name7"] = "PropertyStr7Name";
			_columnName["inventproduct.ipvaluestr8"] = "PropertyStr8Code";
			_columnName["propertystr.name8"] = "PropertyStr8Name";
			_columnName["inventproduct.ipvaluestr9"] = "PropertyStr9Code";
			_columnName["propertystr.name9"] = "PropertyStr9Name";
			_columnName["inventproduct.ipvaluestr10"] = "PropertyStr10Code";
			_columnName["propertystr.name10"] = "PropertyStr10Name";
			_columnName["inventproduct.ipvaluestr11"] = "PropertyStr11Code";
			_columnName["propertystr.name11"] = "PropertyStr11Name";
			_columnName["inventproduct.ipvaluestr12"] = "PropertyStr12Code";
			_columnName["propertystr.name12"] = "PropertyStr12Name";
			_columnName["inventproduct.ipvaluestr13"] = "PropertyStr13Code";
			_columnName["propertystr.name13"] = "PropertyStr13Name";
			_columnName["inventproduct.ipvaluestr14"] = "PropertyStr14Code";
			_columnName["propertystr.name14"] = "PropertyStr14Name";
			_columnName["inventproduct.ipvaluestr15"] = "PropertyStr15Code";
			_columnName["propertystr.name15"] = "PropertyStr15Name";
			_columnName["inventproduct.ipvaluestr16"] = "PropertyStr16Code";
			_columnName["propertystr.name16"] = "PropertyStr16Name";
			_columnName["inventproduct.ipvaluestr17"] = "PropertyStr17Code";
			_columnName["propertystr.name17"] = "PropertyStr17Name";
			_columnName["inventproduct.ipvaluestr18"] = "PropertyStr18Code";
			_columnName["propertystr.name18"] = "PropertyStr18Name";
			_columnName["inventproduct.ipvaluestr19"] = "PropertyStr19Code";
			_columnName["propertystr.name19"] = "PropertyStr19Name";
			_columnName["inventproduct.ipvaluestr20"] = "PropertyStr20Code";
			_columnName["propertystr.name20"] = "PropertyStr20Name";
			_columnName["previousinventory.propextenstion1"] = "PropExtenstion1";
			_columnName["previousinventory.propextenstion2"] = "PropExtenstion2";
			_columnName["previousinventory.propextenstion3"] = "PropExtenstion3";
			_columnName["previousinventory.propextenstion4"] = "PropExtenstion4";
			_columnName["previousinventory.propextenstion5"] = "PropExtenstion5";
			_columnName["previousinventory.propextenstion6"] = "PropExtenstion6";
			_columnName["previousinventory.propextenstion7"] = "PropExtenstion7";
			_columnName["previousinventory.propextenstion8"] = "PropExtenstion8";
			_columnName["previousinventory.propextenstion9"] = "PropExtenstion9";
			_columnName["previousinventory.propextenstion10"] = "PropExtenstion10";
			_columnName["previousinventory.propextenstion11"] = "PropExtenstion11";
			_columnName["previousinventory.propextenstion12"] = "PropExtenstion12";
			_columnName["previousinventory.propextenstion13"] = "PropExtenstion13";
			_columnName["previousinventory.propextenstion14"] = "PropExtenstion14";
			_columnName["previousinventory.propextenstion15"] = "PropExtenstion15";
			_columnName["previousinventory.propextenstion16"] = "PropExtenstion16";
			_columnName["previousinventory.propextenstion17"] = "PropExtenstion17";
			_columnName["previousinventory.propextenstion18"] = "PropExtenstion18";
			_columnName["previousinventory.propextenstion19"] = "PropExtenstion19";
			_columnName["previousinventory.propextenstion20"] = "PropExtenstion20";
			_columnName["previousinventory.propextenstion21"] = "PropExtenstion21";
			_columnName["previousinventory.propextenstion22"] = "PropExtenstion22";
			_columnName["itur.erpiturcode"] = "LocationCode";
			_columnName["itur.description"] = "LocationDescription";
			_columnName["itur.level1"] = "LocationLevel1Code";
			_columnName["itur.name1"] = "LocationLevel1Name";
			_columnName["itur.level2"] = "LocationLevel2Code";
			_columnName["itur.name2"] = "LocationLevel2Name";
			_columnName["itur.level3"] = "LocationLevel3Code";
			_columnName["itur.name3"] = "LocationLevel3Name";
			_columnName["itur.level4"] = "LocationLevel4Code";
			_columnName["itur.name4"] = "LocationLevel4Name";
			_columnName["itur.invstatus"] = "LocationInvStatus";
			_columnName["itur.nodetype"] = "LocationNodeType";
			_columnName["itur.levelnum"] = "LocationLevelNum";
			_columnName["itur.total"] = "LocationTotal";
			_columnName["inventproduct.modifydate"] = "DateModified";
			_columnName["inventproduct.createdate"] = "DateCreated";
			_columnName["inventproduct.itemstatus"] = "ItemStatus";
			_columnName["inventproduct.imputtypecodefrompda"] = "UnitTypeCode";
			_columnName["inventproduct.productname"] = "CatalogItemName";
			_columnName["itur.iturcode"] = "IturCode";

			return _columnName;
		}
	}



}
