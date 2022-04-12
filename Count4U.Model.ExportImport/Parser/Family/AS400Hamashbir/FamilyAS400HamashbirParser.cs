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
	public class FamilyAS400HamashbirParser : IFamilyParser
	{
		private readonly ILog _log;
		private Dictionary<string, Family> _familyDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public FamilyAS400HamashbirParser(IServiceLocator serviceLocator,
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

			CatalogParserPoints catalogParserPoints = parms.GetCatalogParserPointsFromParm();

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

			//	Field1: CompanyCode (col 1-2)
			//Field2: Item Code (col 3-10)
			//Field3: Name (col 11-40)

			//Field4: FamilyID (col 41-44)
			//Field5: FamilyName (col 45-56)
			//Field6: SectionID (col 57-60)
			//Field7: SectionName (col 61-72)

			//Field8: SupplierID (col 73-77)
			//Field9: PriceSell (col 82-90)

			List<Point> startEndSubstring = new List<Point>();
			//5	//Field4: FamilyID (col 41-44)
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.FamilyCodeStart,
				End = catalogParserPoints.FamilyCodeEnd,
				Length = catalogParserPoints.FamilyCodeEnd - catalogParserPoints.FamilyCodeStart + 1
			});

			//Field7: SectionName (col 61-72)
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.FamilyNameStart,
				End = catalogParserPoints.FamilyNameEnd,
				Length = catalogParserPoints.FamilyNameEnd - catalogParserPoints.FamilyNameStart + 1
			});

			int count = startEndSubstring.Count;
			long k = 0;


			foreach (String rec in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{

				//Field1: FamilyCode  0
				//Field2: FamilyName	1

				if (string.IsNullOrWhiteSpace(rec) == true) { continue; }
				int lenRow = rec.Length;

				if (lenRow < catalogParserPoints.FamilyNameStart)
				{
					continue;
				}
				//5	//Field4: FamilyID (col 41-44)			5
				//FamilyCodeStart = 41
				//FamilyCodeEnd	 = 44

				String[] record = { "", ""};
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

				string familyCode = record[0].Trim().LeadingZero4();
				if (this._familyDictionary.ContainsKey(familyCode) == false)
				{
					//string supplierName = record[4].Trim();
					string familyName = record[1].ReverseDosHebrew(invertLetter, rt2lf);
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
