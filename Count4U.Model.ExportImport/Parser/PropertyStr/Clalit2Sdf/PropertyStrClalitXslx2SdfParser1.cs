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
	//Field4: configuration_stat_id
	//Field5: descr
	//PropertyStr6
	//CountExcludeFirstRow = 8	 / col =  4
	public class PropertyStrClalitXslx2SdfParser1 : IPropertyStrParser
	{
		private readonly ILog _log;
		private Dictionary<string, PropertyStr> _propertyStrDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PropertyStrClalitXslx2SdfParser1(IServiceLocator serviceLocator,
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
			bool fileXlsm = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsm);
			IFileParser fileParser;
			if (fileXlsm == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelMacrosFileParser.ToString()); }
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
				//Field4: configuration_stat_id		3		newPropertyStr.PropertyStrCode = recordEmpty[3];
				//Field5: descr								4		newPropertyStr.Name = recordEmpty[4];
				//PropertyStr6
				//CountExcludeFirstRow = 8	 / col =  4

				if (record.Length < 5)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				String[] recordEmpty = { "", "", "", "", ""};

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
						recordEmpty[i] = recordEmpty[i].CutLength(49); 
					}
					//Field4: configuration_stat_id		3		newPropertyStr.PropertyStrCode = recordEmpty[3];
					//Field5: descr								4		newPropertyStr.Name = recordEmpty[4];
					//PropertyStr6

					propertyStr.PropertyStrCode = recordEmpty[3];	 //	 configuration_stat_id
					propertyStr.Name = recordEmpty[4].ReverseDosHebrew(invertLetter, rt2lf); ;						//	descr

					propertyStr.DomainObject = DomainObjectTypeEnum.PropertyStr6.ToString();

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
		
			return this._propertyStrDictionary;
		}

	}
}
