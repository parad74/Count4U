using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{					 
	public class IturNativPlusLadpcParser9999 : IIturParser
	{
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public IturNativPlusLadpcParser9999(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._iturDictionary = new Dictionary<string, Itur>();
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
			string iturNameEmulationType = parms.GetStringValueFromParm(ImportProviderParmEnum.IturNameSuffix);		   //can be 9999 or 1
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			this._iturDictionary.Clear();
			this._errorBitList.Clear();

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
				countExcludeFirstString))
			{
				if (record == null) continue;
				if (record.Length < 2)
				{
					continue;
				}

				//0 Location.Level1.Code
				//1 Location.Level1.Name
	
				String[] recordEmpty = { "", ""};
				int count = 2;
				//if (record.Count() < 3)
				//{
				//	count = record.Count();
				//}

				for (int i = 0; i < count; i++)
				{
					recordEmpty[i] = record[i].Trim();
				}

				// эмуляция 9999
				Itur newItur = new Itur();
				IturString newIturString = new IturString();
				newIturString.LocationCode = recordEmpty[0].CutLength(49);
				string prfix = recordEmpty[0].LeadingZero4();
				newIturString.IturCode = prfix + "0002";
				newIturString.ERPIturCode = newIturString.LocationCode + "-9999";
				newItur.Level1 = newIturString.LocationCode;
				newIturString.StatusIturBit = "0";

				//0 Location.Level1.Code
				//1 Location.Level1.Name

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
				//0 Location.Level1.Code
				//1 Location.Level1.Name
				else //	Error  retBit == 0 
				{
					retBit = newItur.ValidateWarning(newIturString, this._dtfi); //Warning
					if (iturFromDBDictionary.IsDictionaryContainsKey(newItur.IturCode) == false)
					{
						newItur.InvStatus = 0;
						newItur.LevelNum = 2;
						//.Name = recordEmpty[2].CutLength(49);
						newItur.Name1 = recordEmpty[1].CutLength(49);//Level1Name
						newItur.Level2 = "9999";
						newItur.ParentIturCode = newItur.LocationCode;
			
						//0 = Terminal Node & Container (for InventProduct)
						//1 = Not Terminal Node & Not Container (for InventProduct)
						//2 = Not Terminal node & Container (for InventProduct)
						newItur.Disabled = false;//Contaner изначально предполагаем что все  контейнеры
						newItur.NodeType = 0;  //0 = Terminal Node  - изначально предполагаем что все терминальный
	  
						this._iturDictionary[newItur.IturCode] = newItur;
						iturFromDBDictionary[newItur.IturCode] = null;
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
			return this._iturDictionary;
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
