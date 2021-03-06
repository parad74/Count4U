using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Count4U.Model.Main;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class BranchMikramSonolParser : IBranchParser
	{
		private readonly ILog _log;
		private Dictionary<string, Branch> _branchDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public BranchMikramSonolParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._branchDictionary = new Dictionary<string, Branch>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public Dictionary<string, Branch> BranchDictionary
		{
			get { return this._branchDictionary; }
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
		public Dictionary<string, Branch> GetBranchs(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Branch> branchFromDBDictionary,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool fileXlsx = parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			IFileParser fileParser;
			if (fileXlsx == true) { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString()); }
			else { fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString()); }
			if (fileParser == null) throw new ArgumentNullException("FileParser is null");


			this._branchDictionary.Clear();
			this._errorBitList.Clear();

			string customerCode = parms.GetStringValueFromParm(ImportProviderParmEnum.CustomerCode);
			if (String.IsNullOrWhiteSpace(customerCode) == true)
			{
				this._errorBitList.Add(new BitAndRecord	  {	Bit = (int)ConvertDataErrorCodeEnum.CodeIsEmpty,   Record = "Customer Code Is Empty",  ErrorType = MessageTypeEnum.Error  	});
				return this._branchDictionary;
			}

			string separator = SeparatorField.Tab;
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

			
			int i = 0;
			foreach (String[] record in fileParser.GetRecords(fromPathFile,
				encoding, separators,
				countExcludeFirstString))
			{
				i++;
				if (record == null) continue;
				if (record.Length < 1)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				//Field1: Branch ERP Code    &  Local Code (Go to both 2 fields)
				//Field2: Branch Name
					

				//string localCode  = record[0];

				Branch newBranch = new Branch();
				//BranchString newBranchString = new BranchString();
				newBranch.BranchCodeERP = record[0];
				newBranch.BranchCodeLocal = record[0];

				//CodeERP
				string ret = BranchValidate.CodeErpValidate(newBranch.BranchCodeERP);
				if (ret != String.Empty)  //Error
				{
					this._errorBitList.Add(new BitAndRecord	 {Bit = (int)ConvertDataErrorCodeEnum.InvalidValue, Record = ret + " : " + record.JoinRecord(separator), ErrorType = MessageTypeEnum.Error });
					continue;
				}
				else //	Error  retBit == 0 
				{
	
					if (record.Length > 1)
					{
						string ret1 = BranchValidate.NameValidate(record[1]);
						if (ret1 == String.Empty)
						{
							newBranch.Name = record[1].ReverseDosHebrew(invertLetter, rt2lf); 
						}
						else
						{
							this._errorBitList.Add(new BitAndRecord	{Bit = (int)ConvertDataErrorCodeEnum.InvalidValue,		Record = record.JoinRecord(separator),	ErrorType = MessageTypeEnum.Error	});
						}
					}

					newBranch.Code = customerCode + "-" + newBranch.BranchCodeLocal;
					newBranch.CustomerCode = customerCode;
					newBranch.DBPath = newBranch.Code;

					if (branchFromDBDictionary.ContainsKey(newBranch.Code) == true)
					{
						this.Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.BranchCodeExistInDB, record.JoinRecord(separator)));
						continue;
					}
					this._branchDictionary[newBranch.Code] = newBranch;
					branchFromDBDictionary[newBranch.Code] = null;
				}
			}
			return this._branchDictionary;
		}
	}


}
