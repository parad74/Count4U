using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	//парсит 6 , добавляем в 7
	public class PropertyStr6MerkavaXslx2SdfParser1 : IPropertyStrParser
	{
		private readonly ILog _log;
		private Dictionary<string, PropertyStr> _propertyStrDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PropertyStr6MerkavaXslx2SdfParser1(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._propertyStrDictionary = new Dictionary<string, PropertyStr>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, PropertyStr> PropertyStrDictionary
		{
			get { return this._propertyStrDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, PropertyStr> GetPropertyStrs(string fromPathFile,
			DomainObjectTypeEnum domainObjectType,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, PropertyStr> propertyStrFromDBDictionary,
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

			this._propertyStrDictionary.Clear();
			this._errorBitList.Clear();

			Random rnd = new Random();
			string separator = " ";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			IPropertyStrRepository propertyStrRepository = this._serviceLocator.GetInstance<IPropertyStrRepository>();

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

			//парсит 6 , добавляем в 7 - это второй шаг
			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString,
				sheetNameXlsx,
				sheetNumberXlsx))
			{
				if (record == null)	continue;
				//Property6Str
				//EmployeeCode			  0					newPropertyStr.PropertyStrCode = recordEmpty[0];
				//EmployeeID				 1 						 newPropertyStr.Code = recordEmpty[1];

				//Property6Str
				// => на первом шаге было 
				//EmployeeCode			  0
				//EmployeeName			  1 

				// нужен результат в БД	 property
				//EmployeeCode			  0			newPropertyStr.PropertyStrCode
				//EmployeeName			  1 		    newPropertyStr.Name
				// EmployeeID							    newPropertyStr.Code
				//propertyStr.DomainObject		Property6Str
				if (record.Length < 2)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				String[] recordEmpty = { "", ""};

				PropertyStr newPropertyStr = new PropertyStr();
				PropertyStr propertyStr = new PropertyStr();
				int count = 2;
					if (record.Count() < 2)
					{
						count = record.Count();
					}
				 
					for (int i = 0; i < count; i++)
					{
						recordEmpty[i] = record[i].Trim();
						recordEmpty[i] = recordEmpty[i].CutLength(49); 
					}

					//EmployeeCode			  0					newPropertyStr.PropertyStrCode = recordEmpty[0];
					//EmployeeID				 1 						 newPropertyStr.Code = recordEmpty[1];
					propertyStr.PropertyStrCode = recordEmpty[0];																		//	 EmployeeCode

					int retBit = newPropertyStr.ValidateError(propertyStr);
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

					newPropertyStr.DomainObject = DomainObjectTypeEnum.PropertyStr6.ToString();
					if (record.Length > 1)
					{
						newPropertyStr.Code = recordEmpty[1];		  						//	EmployeeID
					}

					this._propertyStrDictionary[newPropertyStr.PropertyStrCode] = newPropertyStr;

			}

			IEnumerable<PropertyStr> propertyStrFromDBSimples = propertyStrRepository.GetPropertyStrs(DomainObjectTypeEnum.PropertyStr7.ToString(), dbPath);
			IImportPropertyStrRepository provider = this._serviceLocator.GetInstance<IImportPropertyStrRepository>();
			provider.ClearPropertyStrs(DomainObjectTypeEnum.PropertyStr7, dbPath);
			//	Dictionary<string, PropertyStr> _returnPropertyStrDictionary = new Dictionary<string,PropertyStr>();

			foreach (PropertyStr propertyStrSimple in propertyStrFromDBSimples)	 //code,Name
			{
				PropertyStr ps = new PropertyStr();		  //get from updateDictionary propertyStrSimple
				bool exists = this._propertyStrDictionary.TryGetValue(propertyStrSimple.PropertyStrCode, out ps);	//code, Name

				if (exists == true)	   //если в файле апдейта есть Макат текущего  ProductSimple, то мы его обновляем
				{
					propertyStrSimple.Code = ps.Code;
					this._propertyStrDictionary[propertyStrSimple.PropertyStrCode] = propertyStrSimple;
				}
			}
			return this._propertyStrDictionary;
		}
	}
}
