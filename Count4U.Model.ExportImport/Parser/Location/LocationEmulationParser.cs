using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class LocationEmulationParser : ILocationParser
	{
		private readonly ILog _log;
		private Dictionary<string, Location> _locationDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public LocationEmulationParser(//IFileParser fileParser,
			ILog log)
		{
			this._log = log;
			this._locationDictionary = new Dictionary<string, Location>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, Location> LocationDictionary
		{
			get { return this._locationDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, Location> GetLocations(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Location> locationFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			this._locationDictionary.Clear();
			this._errorBitList.Clear();

			Random rnd = new Random();
			//string separator = " ";
			//if (separators != null && separators.Count() > 0)
			//{
			//	separator = separators[0];
			//}

			//bool invertLetter = false;
			//bool rt2lf = false;
			//if (parms != null)
			//{
			//	if (encoding == Encoding.GetEncoding(862) || encoding == Encoding.GetEncoding(1255))
			//	{
			//		invertLetter = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLetters);
			//		rt2lf = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWords);
			//	}
			//}

			int count = parms.GetIntValueFromParm(ImportProviderParmEnum.MaxLen);
			if (count <= 0) return this._locationDictionary;
			long code = 0;
			for (int i = 1; i <= count; )
			{
				code++;
				Location newLocation = new Location();
				LocationString newLocationString = new LocationString();
				newLocationString.Code = code.ToString().LeadingZero4();
				newLocationString.Name = newLocationString.Code;
				newLocationString.Description = "emulation";
				//newLocationString.BackgroundColor = ;


				int retBit = newLocation.ValidateError(newLocationString, rnd);
				if (retBit != 0)  
				{
					i++;
					continue;
				}
				else  
				{
					retBit = newLocation.ValidateWarning(newLocationString, rnd); //Warning
					if (locationFromDBDictionary.ContainsKey(newLocation.Code) == false)
					{
						//this._locationDictionary.AddToDictionary(newLocation.Code, newLocation, record.JoinRecord(separator), Log);
						this._locationDictionary[newLocation.Code] = newLocation;
						i++;
						locationFromDBDictionary[newLocation.Code] = null;
					}
				}
			} //	for i


			if (locationFromDBDictionary.ContainsKey(DomainUnknownCode.UnknownLocation) == false
				&& this._locationDictionary.ContainsKey(DomainUnknownCode.UnknownLocation) == false)
			{
				Location newLocation = new Location();
				newLocation.Code = DomainUnknownCode.UnknownLocation;
				newLocation.Name = DomainUnknownName.UnknownLocation;		//TODO Localization
				newLocation.Description = DomainUnknownCode.UnknownLocation;
				newLocation.BackgroundColor = "255, 12, 255";
				this._locationDictionary[newLocation.Code] = newLocation;
			}
			return this._locationDictionary;
		}

	}
}
