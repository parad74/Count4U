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
	public class PropertyStrProfileNativXslx2SdfParser : IPropertyStrParser
	{
		private readonly ILog _log;
		private Dictionary<string, PropertyStr> _propertyStrDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PropertyStrProfileNativXslx2SdfParser(IServiceLocator serviceLocator,
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

			int sheetNumberXlsx = parms.GetIntValueFromParm(ImportProviderParmEnum.SheetNumberXlsx);					// start from 1
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


			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString,
				sheetNameXlsx,
				sheetNumberXlsx))
			{
				if (record == null)	continue;
				//XPath				0
				//Index 				1
				//Property_mINV				2
				//Property_Count4U			3

				//Name =						//XPath								0		"UIDKey";
				//TypeCode = 				//Index 								1
				//Code =						//Property_Count4U			2		"Makat";   - fromCount4U
				//PropertyStrCode = 	//Property_mINV				3		"ItemCode"; - from PDA
				//DomainObject = "Profile";
				
	
				if (record.Length < 4)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				String[] recordEmpty = { "", "", "", "" };

				PropertyStr newPropertyStr = new PropertyStr();
				PropertyStr propertyStr = new PropertyStr();
				int count = 4;
					if (record.Count() < 4)
					{
						count = record.Count();
					}
				 
					for (int i = 0; i < count; i++)
					{
						recordEmpty[i] = record[i].Trim();
					}

				//Name =						//XPath								0		"UIDKey";
				//TypeCode = 				//Index 								1
				//Code =						//Property_Count4U			2		"Makat";   - fromCount4U
				//PropertyStrCode = 	//Property_mINV				3		"ItemCode"; - from PDA
				//DomainObject = "Profile";


					string xPath = recordEmpty[0].CutLength(49);
					string index =  recordEmpty[1].CutLength(49);
					string propertyNameCount4U = recordEmpty[2].CutLength(49);
					string propertyNamemINV = recordEmpty[3].CutLength(49);

					propertyStr.Name = xPath; 	 //	 xPath			   "UIDKey";
					propertyStr.TypeCode = index;//	 Index
					propertyStr.Code = propertyNameCount4U;			//2		"Makat";   - fromCount4U
					propertyStr.PropertyStrCode = propertyNamemINV; 	//3		"ItemCode"; - from PDA
					propertyStr.DomainObject = DomainObjectTypeEnum.Profile.ToString();

					int retBit = newPropertyStr.ValidateError(propertyStr);
					if (retBit != 0)  //Error
					{
						//this.IsDictionaryContainsKey(newItur.Code, iturFromDBDictionary, record.JoinRecord(separator));	   //?
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
						//if (locationFromDBDictionary.IsDictionaryContainsKey(newLocation.Code, record.JoinRecord(separator), Log) == false)
						if (propertyStrFromDBDictionary.ContainsKey(newPropertyStr.PropertyStrCode) == false)
						{
							//this._locationDictionary.AddToDictionary(newLocation.Code, newLocation, record.JoinRecord(separator), Log);
							this._propertyStrDictionary[newPropertyStr.PropertyStrCode] = newPropertyStr;
							propertyStrFromDBDictionary[newPropertyStr.PropertyStrCode] = null;
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
			//if (propertyStrFromDBDictionary.ContainsKey(DomainUnknownCode.UnknownPropertyStr) == false
			//	&& this._propertyStrDictionary.ContainsKey(DomainUnknownCode.UnknownPropertyStr) == false)
			//{
			//	PropertyStr newPropertyStr = new PropertyStr();
			//	newPropertyStr.PropertyStrCode = DomainUnknownCode.UnknownPropertyStr;
			//	newPropertyStr.DomainObject = DomainObjectTypeEnum.BuildingConfig.ToString();
			//	this._propertyStrDictionary[newPropertyStr.PropertyStrCode] = newPropertyStr;
			//}
			return this._propertyStrDictionary;
		}

	}
}
