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
	public class FamilyNativXslx2SdfParser : IFamilyParser
	{
		private readonly ILog _log;
		private Dictionary<string, Family> _familyDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public FamilyNativXslx2SdfParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._familyDictionary = new Dictionary<string, Family>();
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
			Dictionary<string, Family> FamilyFromDBDictionary, 
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			this._familyDictionary.Clear();
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

			long k = 0;


			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString))
			{
				//Field4  FamilyCode		 3
				//Field5  Family // can use as FamilyName	 4

				if (record == null) continue;
				if (record.Length < 5)
				{
					continue;
				}

				string familyCode = DomainUnknownCode.UnknownFamily;
				string familyName = DomainUnknownName.UnknownFamily;
				//Field4  FamilyCode		 3
				//Field5  Family // can use as FamilyName	 4
				if (string.IsNullOrWhiteSpace(record[3].Trim()) == false)
				{
					familyCode = record[3].Trim(' ');
				}

				if (string.IsNullOrWhiteSpace(record[4].Trim()) == false)
				{
					familyName = record[4].Trim(' ');
				}

				if (this._familyDictionary.ContainsKey(familyCode) == false)
				{
					//string familyName = record[1].ReverseDosHebrew(invertLetter, rt2lf);
					//string type = record[2].ReverseDosHebrew(invertLetter, rt2lf);
					//string size = record[3].ReverseDosHebrew(invertLetter, rt2lf);
			
					//string extra1 =	"";
					//string extra2 =	"";
					//Field5: Extra1	4
					//Field6: Extra2	5
					//if (record.Length > 4)		extra1 = record[4].Trim();
					//if (record.Length > 5)       extra2 = record[5].Trim();

					Family family = new Family { FamilyCode = familyCode, Name = familyName, Description = "" };
					family.ValidateLenght50();
					this._familyDictionary[family.FamilyCode] = family;
				}
				 
			} //foreach
			//this._supplierDictionary = this._supplierRepository.GetSupplierDictionary(fromPathFile, true);
			return this._familyDictionary;
		}



	}
}
