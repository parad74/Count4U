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
	public class SectionTafnitMatrixParser : ISectionParser
	{
		private readonly ILog _log;
		private Dictionary<string, Section> _sectionDictionary;
		private Dictionary<string, Section> _sectionTempDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public SectionTafnitMatrixParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._sectionDictionary = new Dictionary<string, Section>();
			this._sectionTempDictionary = new Dictionary<string, Section>();
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
			this._sectionTempDictionary.Clear();
			this._errorBitList.Clear();

			string separator = ";";

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
				if (record.Length < 10)
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

				
				//string sectionCode = record[6].Trim().ReverseDosHebrew(invertLetter, rt2lf) + "#" + record[7].Trim().ReverseDosHebrew(invertLetter, rt2lf)
				//	+ "#" + record[8].Trim().ReverseDosHebrew(invertLetter, rt2lf) + "#" + record[9].Trim().ReverseDosHebrew(invertLetter, rt2lf); 

				string sectionName = record[6].Trim().ReverseDosHebrew(invertLetter, rt2lf); 

				Section newSection = new Section();
				SectionString newSectionString = new SectionString();
				newSectionString.SectionCode = sectionName;
				newSectionString.Description = "";
				newSectionString.Name = sectionName;
	
				int retBit = newSection.ValidateError(newSectionString, this._dtfi);
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
					retBit = newSection.ValidateWarning(newSectionString, this._dtfi); //Warning
					if (sectionFromDBDictionary.IsDictionaryContainsKey(newSection.SectionCode) == false)
					{
						this._sectionTempDictionary[newSection.SectionCode] = newSection;
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

			this._sectionDictionary.Clear();
			
			Section sectionNew = new Section();
			sectionNew.SectionCode = DomainUnknownCode.UnknownSection;
			sectionNew.Name = DomainUnknownName.UnknownSection;
			this._sectionDictionary[DomainUnknownCode.UnknownSection] = sectionNew;
			int i = 0;
			foreach (KeyValuePair<string, Section> keyValuePair in this._sectionTempDictionary)
			{
				i++;
				string code = i.ToString().PadLeft(4, '0');//Guid.NewGuid().ToString(); 
				Section section = keyValuePair.Value;
				section.SectionCode = code;
				this._sectionDictionary[code] = section;
			}

			return this._sectionDictionary;
		}

	}


}
