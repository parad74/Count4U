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
	public class IturFacingParser : IIturParser
	{
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;
		private Dictionary<string, Supplier> _supplierDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public IturFacingParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._iturDictionary = new Dictionary<string, Itur>();
			this._supplierDictionary = new Dictionary<string, Supplier>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public Dictionary<string, Itur> IturDictionary
		{
			get { return this._iturDictionary; }
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
		public Dictionary<string, Itur> GetIturs(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Itur> iturFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			this._iturDictionary.Clear();
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
			string newDocumentCode = Guid.NewGuid().ToString(); 

			
			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString))
			{
				if (record == null) continue;
				if (record.Length < 4)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

//Field1: Itur Code				0
//Field2: Width					1
//Field3: Height					2
//Field4: Total Shelves		3
//Field5: Shelf Code			4
//Field6: Supplier Code		5
//Field7: Supplier Width		6
//Field8: Date						7
//Field9: Time						8


				//string objectType = Convert.ToString(record[0]).Trim(" \t".ToCharArray());

				Itur newItur = new Itur();
				IturString newIturString = new IturString();

				string iturCode = record[0].Trim();
				iturCode = iturCode.TrimStart('0');
				if (iturCode.Length == 0) newIturString.IturCode = "20159999";
				else if (iturCode.Length <= 4) newIturString.IturCode = "2015" + iturCode.PadLeft(4, '0');
				else newIturString.IturCode = iturCode;

				newIturString.LocationCode = DomainUnknownCode.UnknownLocation;
				newIturString.StatusIturBit = "0";
				//if (record.Length >= 2)
				//{
				//	newIturString.ERPIturCode = record[1];
				//}

				//if (record.Length == 3)
				//{
				//	newIturString.Name = record[2];
				//}

				int retBit = newItur.ValidateError(newIturString, this._dtfi);
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
					retBit = newItur.ValidateWarning(newIturString, this._dtfi); //Warning
					if (iturFromDBDictionary.IsDictionaryContainsKey(newItur.IturCode) == false)
					{
						//Field2: Width					1
//Field3: Height					2
//Field4: Total Shelves		3
						
						newItur.Width =  GetInt(record[1]);
						newItur.Height = GetInt(record[2]);
						newItur.Area = (double)(newItur.Width * newItur.Height) / 10000.0;
						newItur.ShelfInItur = GetInt(record[3]);
						if (newItur.ShelfInItur == 0) newItur.ShelfInItur = 1;
						newItur.UnitPlaceWidth = (int)((double)(newItur.Width * newItur.Height) / (double)newItur.ShelfInItur);
						newItur.AreaCount = newItur.Area / (double)newItur.ShelfInItur;
						newItur.Description = newItur.ShelfInItur + " ; " + newItur.Width + " ; " + newItur.Height + " ; " + newItur.Area;
						this._iturDictionary[newItur.IturCode] = newItur;
						iturFromDBDictionary[newItur.IturCode] = null;
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
			return this._iturDictionary;
		}

		private int GetInt(string strValue)
		{
			int ret = 0;
			bool yes1 = Int32.TryParse(strValue, out ret);
			if (yes1 == true) return ret;
			else return 0;
		
		}

		#region IIturParser Members


		public IEnumerable<Itur> GetItursEnumerable(string fromPathFile, Encoding encoding, string[] separators, int countExcludeFirstString, Dictionary<string, Itur> IturFromDBDictionary, Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IIturParser Members


		public List<Location> LocationToDBList
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

	
}
