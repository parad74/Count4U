using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class SectionMPLParser : ISectionParser
	{
		private readonly ILog _log;
		private Dictionary<string, Section> _sectionDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public SectionMPLParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._sectionDictionary = new Dictionary<string, Section>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public Dictionary<string, Section> SectionDictionary
		{
			get { return this._sectionDictionary; }
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// Получение списка Product 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, Section> GetSections(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Section> sectionFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			this._sectionDictionary.Clear();
			this._errorBitList.Clear();

			string separator = ",";
			string separatorParms = parms.GetStringValueFromParm(ImportProviderParmEnum.Delimiter);
			if (string.IsNullOrWhiteSpace(separatorParms) == false) separator = separatorParms; 
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
				countExcludeFirstString))
			{
				if (record == null) continue;
				if (record.Length < 6) 
				{
						continue;
				}

				//Field1: Barcode								0
				//Field2: Item Code							1
				//Field3: Name									2
				//Field4: Sell Price (optional)				3
				//Field5: Section Code (optional)		4
				//Field6: Section Name (optional)		5

				Section newSection = new Section();
				SectionString newSectionString = new SectionString();
				newSectionString.SectionCode = record[5].Trim().LeadingZero3(); //record[4];
				newSectionString.Description = "";
				newSectionString.Name = "";
				if (record.Length > 6) //>5
				{
					newSectionString.Name = record[6].ReverseDosHebrew(invertLetter, rt2lf); 
				}

				int retBit = newSection.ValidateError(newSectionString, this._dtfi);
				if (retBit != 0)  {
					this._errorBitList.Add(new BitAndRecord	{Bit = retBit,	Record = record.JoinRecord(separator),	ErrorType = MessageTypeEnum.Error	});
					continue;
				}
				else //	Error  retBit == 0 
				{
					retBit = newSection.ValidateWarning(newSectionString, this._dtfi); //Warning
					if (sectionFromDBDictionary.IsDictionaryContainsKey(newSection.SectionCode) == false)
					{
						this._sectionDictionary[newSection.SectionCode] = newSection;
						sectionFromDBDictionary[newSection.SectionCode] = null;
						//this._iturDictionary.AddToDictionary(newItur.Code, newItur, record.JoinRecord(separator), Log);
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
			return this._sectionDictionary;
		}

	}


}
