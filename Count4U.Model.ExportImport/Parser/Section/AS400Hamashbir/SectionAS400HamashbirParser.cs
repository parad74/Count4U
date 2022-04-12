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
	public class SectionAS400HamashbirParser : ISectionParser
	{

		private readonly ILog _log;
		private Dictionary<string, Section> _sectionDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public SectionAS400HamashbirParser(IServiceLocator serviceLocator,
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
			CatalogParserPoints catalogParserPoints = parms.GetCatalogParserPointsFromParm();

			//SPARITEWSN
			//Files type : Fixed ANSI 
			//CatalogMinLengthIncomingRow = 10,
			//	CatalogItemCodeStart = 3,
			//	CatalogItemCodeEnd = 10, 
			//	CatalogItemNameStart = 11,
			//	CatalogItemNameEnd = 40,
			//	SectionCodeStart=41, 
			//	SectionCodeEnd=44,
			//	SectionNameStart = 45,
			//	SectionNameEnd = 56,
			//	SupplierCodeStart = 73, 
			//	SupplierCodeEnd = 77,
			//	CatalogPriceSaleStart = 82,
			//	CatalogPriceSaleEnd = 90 


			List<Point> startEndSubstring = new List<Point>();

			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.SectionCodeStart,
				End = catalogParserPoints.SectionCodeEnd,
				Length = catalogParserPoints.SectionCodeEnd - catalogParserPoints.SectionCodeStart + 1
			});
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.SectionNameStart,
				End = catalogParserPoints.SectionNameEnd,
				Length = catalogParserPoints.SectionNameEnd - catalogParserPoints.SectionNameStart + 1
			});



			int count = startEndSubstring.Count;
			long k = 0;

			
			foreach (String rec in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				if (string.IsNullOrWhiteSpace(rec) == true) { continue; }
				int lenRow = rec.Length;


				if (lenRow < catalogParserPoints.CatalogMinLengthIncomingRow)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedLengthString, rec));
					continue;
				}


				//	SectionCodeStart=41, 					   0
				//	SectionCodeEnd=44,

				//	SectionNameStart = 45,				   1
				//	SectionNameEnd = 56,

				String[] record = { "", "" };

				for (int i = 0; i < count; i++)
				{
					if (startEndSubstring[i].End < lenRow)
					{
						record[i] = rec.Substring(startEndSubstring[i].Start, startEndSubstring[i].Length);
					}
					else //startEndSubstring[i].End >= lenRow
					{
						if (startEndSubstring[i].Start < lenRow)
						{
							record[i] = rec.Substring(startEndSubstring[i].Start, lenRow - startEndSubstring[i].Start);
						}
					}
				}

				//	newProductSimpleString.SectionCode = record[2].Trim().LeadingZero3(); 				  //test!


				Section newSection = new Section();
				SectionString newSectionString = new SectionString();
				string sectionCode = record[0].Trim().LeadingZero4(); //record[4];
				//sectionCode = sectionCode.Trim('"');
				//sectionCode = sectionCode.Trim();
				newSectionString.SectionCode = sectionCode;

				newSectionString.Description = "";

				string name = record[1].Trim();
				//name = name.Trim('"');
				//name = name.Trim();
				newSectionString.Name = name.ReverseDosHebrew(invertLetter, rt2lf);

				newSectionString.TypeCode = TypeSectionEnum.S.ToString();

				int retBit = newSection.ValidateError(newSectionString, this._dtfi);
				if (retBit != 0)
				{
					this._errorBitList.Add(new BitAndRecord { Bit = retBit, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.Error });
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
		} //foreach
	}




}

	

