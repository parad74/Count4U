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
	public class UnitPlanParser : IUnitPlanParser
	{
		private readonly ILog _log;
		private Dictionary<string, UnitPlan> _unitPlanDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public UnitPlanParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._unitPlanDictionary = new Dictionary<string, UnitPlan>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, UnitPlan> UnitPlanDictionary
		{
			get { return this._unitPlanDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, UnitPlan> GetUnitPlans(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<string, UnitPlan> unitPlanFromDBDictionary,
			 Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");

			this._unitPlanDictionary.Clear();
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
				encoding, separators,
				countExcludeFirstString))
			{
				if (record == null) continue;

				if (record.Length < 2)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				//[UnitPlanCode] nvarchar		0
				//[LayerCode] nvarchar			1
				//[ObjectCode] nvarchar		2
				//[Description]						3
				//[StartX] float					4
				//[StartY] float 					5
				//[Height] float 					6
				//[Width] float 						7
				//[Rotate] int						8
				String[] recordEmpty = { "", "", "", "", "", "", "", "", "" };

				UnitPlan newUnitPlan = new UnitPlan();
				UnitPlanString newUnitPlanString = new UnitPlanString();
				int count = record.Count();
				for (int i = 0; i < count; i++)
				{
					recordEmpty[i] = record[i];
				}
				newUnitPlanString.UnitPlanCode = recordEmpty[0];
				newUnitPlanString.LayerCode = recordEmpty[1];
				newUnitPlanString.ObjectCode = recordEmpty[2];
				newUnitPlanString.Description = recordEmpty[3];
				newUnitPlanString.StartX = recordEmpty[4];
				newUnitPlanString.StartY = recordEmpty[5];
				newUnitPlanString.Height = recordEmpty[6];
				newUnitPlanString.Width = recordEmpty[7];
				newUnitPlanString.Rotate = recordEmpty[8];

				int retBit = newUnitPlan.ValidateError(newUnitPlanString);
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
					retBit = newUnitPlan.ValidateWarning(newUnitPlanString); //Warning
					if (unitPlanFromDBDictionary.ContainsKey(newUnitPlan.UnitPlanCode) == false)
					{
						this._unitPlanDictionary[newUnitPlan.UnitPlanCode] = newUnitPlan;
						unitPlanFromDBDictionary[newUnitPlan.UnitPlanCode] = null;
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
			if (unitPlanFromDBDictionary.ContainsKey(DomainUnknownCode.UnknownUnitPlan) == false
				&& this._unitPlanDictionary.ContainsKey(DomainUnknownCode.UnknownUnitPlan) == false)
			{
				UnitPlan newUnitPlan = new UnitPlan();
				newUnitPlan.UnitPlanCode = DomainUnknownCode.UnknownLocation;
				this._unitPlanDictionary[newUnitPlan.UnitPlanCode] = newUnitPlan;
			}
			return this._unitPlanDictionary;
		}


	}
}
