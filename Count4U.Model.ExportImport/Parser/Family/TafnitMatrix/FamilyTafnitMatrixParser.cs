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
	public class FamilyTafnitMatrixParser : IFamilyParser
	{
		private readonly ILog _log;
		private Dictionary<string, Family> _familyDictionary;
		private Dictionary<string, Family> _familyTempDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public FamilyTafnitMatrixParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._familyDictionary = new Dictionary<string, Family>();
			this._familyTempDictionary = new Dictionary<string, Family>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public Dictionary<string, Family> FamilyDictionary 
		{
			get { return this._familyDictionary; }
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		public Dictionary<string, Family> GetFamilys(string fromPathFile, 
			Encoding encoding, string[] separators, 
			int countExcludeFirstString, 
			Dictionary<string, Family> familyFromDBDictionary, 
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			this._familyDictionary.Clear();
			this._familyTempDictionary.Clear();
			this._errorBitList.Clear();

			string separator = ",";
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
				encoding,
				separators,
				countExcludeFirstString))
			{

				if (record == null) continue;

				if (record.Length < 6)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}
				//--------------
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
				string familyCode = record[6].Trim().ReverseDosHebrew(invertLetter, rt2lf) + "#" + record[7].Trim().ReverseDosHebrew(invertLetter, rt2lf)
			+ "#" + record[8].Trim().ReverseDosHebrew(invertLetter, rt2lf) + "#" + record[9].Trim().ReverseDosHebrew(invertLetter, rt2lf);

				Family newFamily = new Family();
				FamilyString newFamilyString = new FamilyString();
				newFamilyString.FamilyCode = familyCode;
				newFamilyString.Type = record[7].Trim().ReverseDosHebrew(invertLetter, rt2lf);
				newFamilyString.Extra1 = record[8].Trim().ReverseDosHebrew(invertLetter, rt2lf);
				newFamilyString.Extra2 = record[9].Trim().ReverseDosHebrew(invertLetter, rt2lf);
				newFamilyString.Description = familyCode;
				newFamilyString.Name = record[6].Trim().ReverseDosHebrew(invertLetter, rt2lf);
	
				int retBit = newFamily.ValidateError(newFamilyString, this._dtfi);
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
					retBit = newFamily.ValidateWarning(newFamilyString, this._dtfi); //Warning
					if (familyFromDBDictionary.IsDictionaryContainsKey(newFamily.FamilyCode) == false)
					{
						this._familyTempDictionary[newFamily.FamilyCode] = newFamily;
						familyFromDBDictionary[newFamily.FamilyCode] = null;
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

			this._familyDictionary.Clear();

			Family familyNew = new Family();
			familyNew.FamilyCode = DomainUnknownCode.UnknownFamily;
			familyNew.Name = DomainUnknownName.UnknownFamily;
			this._familyDictionary[DomainUnknownCode.UnknownFamily] = familyNew;
			int i = 0;
			foreach (KeyValuePair<string, Family> keyValuePair in this._familyTempDictionary)
			{
				i++;
				string code = i.ToString().PadLeft(4, '0');//Guid.NewGuid().ToString(); 
				Family family = keyValuePair.Value;
				family.FamilyCode = code;
				//family.ValidateLenght50();
				this._familyDictionary[code] = family;
			}
		
			return this._familyDictionary;
		}



	}
}
