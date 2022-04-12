using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4Mobile
{
	//НЕ Используется - то же в 	BuildingConfigMerkavaXslxParser перенесено
	public class BuildingConfigMerkavaXslx2SqliteParser : IBuildingConfigParser
	{
		private readonly ILog _log;
		private Dictionary<string, BuildingConfig> _buildingConfigDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public BuildingConfigMerkavaXslx2SqliteParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._buildingConfigDictionary = new Dictionary<string, BuildingConfig>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, BuildingConfig> BuildingConfigDictionary
		{
			get { return this._buildingConfigDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, BuildingConfig> GetBuildingConfigs(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> buildingConfigFromDBDictionary,
			DomainObjectTypeEnum domainObjectType,
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

			this._buildingConfigDictionary.Clear();
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

				if (record.Length < 4)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}


				String[] recordEmpty = { "", "", "", "", "", "" };

				BuildingConfig newBuildingConfig = new BuildingConfig();
				int count = 6;
					if (record.Count() < 6)
					{
						count = record.Count();
					}
				 
					for (int i = 0; i < count; i++)
					{
						recordEmpty[i] = record[i].Trim();
					}
					//Name				0
					//Description		1
					//Ord					2
					//Code					3
					// NameEn			4
					// NameHe			5
					string name = recordEmpty[0].ReverseDosHebrew(invertLetter, rt2lf);
					string nameEn = recordEmpty[4].ReverseDosHebrew(invertLetter, rt2lf);
					string nameHe = recordEmpty[5].ReverseDosHebrew(invertLetter, rt2lf);
					newBuildingConfig.Name = name;
					int ord =  0;
					Int32.TryParse(recordEmpty[2], out ord);
					newBuildingConfig.Ord = ord;
					newBuildingConfig.Uid = Guid.NewGuid().ToString();
					newBuildingConfig.NameEn = nameEn;
					newBuildingConfig.NameHe = nameHe;

					if (buildingConfigFromDBDictionary.ContainsKey(name) == false)
					{
						//this._locationDictionary.AddToDictionary(newLocation.Code, newLocation, record.JoinRecord(separator), Log);
						this._buildingConfigDictionary[name] = newBuildingConfig;
						buildingConfigFromDBDictionary[name] = null;
					}				
			}

			return this._buildingConfigDictionary;
		}

	}
}
