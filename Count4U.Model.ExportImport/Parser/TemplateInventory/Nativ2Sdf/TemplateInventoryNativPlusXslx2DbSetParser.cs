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
using Count4U.Model.Common;


namespace Count4U.Model.Count4Mobile
{
	public class TemplateInventoryNativPlusXslx2DbSetParser : ITemplateInventorySQLiteParser
	{
		private readonly ILog _log;
		private Dictionary<string, TemplateInventory> _templateInventoryDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public TemplateInventoryNativPlusXslx2DbSetParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._templateInventoryDictionary = new Dictionary<string, TemplateInventory>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, TemplateInventory> TemplateInventoryDictionary
		{
			get { return this._templateInventoryDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TemplateInventory> GetTemplateInventorys(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> templateInventoryFromDBDictionary,
			//Dictionary<string, string> catalogFromDBDictionary,
			//Dictionary<string, string> locationFromDBDictionary,
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

			this._templateInventoryDictionary.Clear();
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
				if (record == null) continue;

				if (record.Length < 5)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}


				//LocationLevel1							 0
				//LocationLevel2							 1
				//LocationLevel3							2
				//LocationLevel4							3
				//ItemCode									4
				//QuantityExpected					   5

				String[] recordEmpty = { "", "", "", "", "", "" };
				int count = recordEmpty.Length;
				//String[] recordEmpty = new string[count];
				//for (int i = 0; i < count; i++)
				//{
				//	recordEmpty[i] = "";
				//}

				if (record.Count() < count)
				{
					count = record.Count();
				}

				for (int i = 0; i < count; i++)
				{
					if (record[i].Length > 49)
					{
						record[i] = record[i].Substring(0, 49);
					}
					recordEmpty[i] = record[i].Trim();
				}

				//LocationLevel1							 0
				//LocationLevel2							 1
				//LocationLevel3							2
				//LocationLevel4							3
				//ItemCode									4
				//QuantityExpected					   5


				string locationLevel1 = recordEmpty[0];
				string locationLevel2 = recordEmpty[1];
				string locationLevel3 = recordEmpty[2];
				string locationLevel4 = recordEmpty[3];
				string itemCode = recordEmpty[4];
				string quantityExpected = recordEmpty[5];

				TemplateInventory newTemplateInventory = new TemplateInventory();
				newTemplateInventory.Level1Code = locationLevel1;
				newTemplateInventory.Level2Code = locationLevel2;
				newTemplateInventory.Level3Code = locationLevel3;
				newTemplateInventory.Level4Code = locationLevel4;
				newTemplateInventory.ItemCode = itemCode;
				newTemplateInventory.QuantityExpected = quantityExpected;

				yield return newTemplateInventory;

			}
		}

	
	}
}
