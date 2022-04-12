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
	public class BranchGazitVerifoneParser : IBranchParser
	{
		private readonly ILog _log;
		private Dictionary<string, Branch> _branchDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public BranchGazitVerifoneParser(IServiceLocator serviceLocator,
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
			string customerCode = parms.GetStringValueFromParm(ImportProviderParmEnum.CustomerCode);
			if (String.IsNullOrWhiteSpace(customerCode) == true)
			{
				this._errorBitList.Add(new BitAndRecord
				{
					Bit = (int)ConvertDataErrorCodeEnum.CodeIsEmpty,
					Record = "Customer Code Is Empty",
					ErrorType = MessageTypeEnum.Error
				});
				return this._branchDictionary;
			}

			string separator = ",";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			List<Point> startEndSubstring = new List<Point>();
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogItemCodeStart,
				End = catalogParserPoints.CatalogItemCodeEnd,
				Length = catalogParserPoints.CatalogItemCodeEnd - catalogParserPoints.CatalogItemCodeStart + 1
			});
			startEndSubstring.Add(new Point
			{
				Start = catalogParserPoints.CatalogItemNameStart,
				End = catalogParserPoints.CatalogItemNameEnd,
				Length = catalogParserPoints.CatalogItemNameEnd - catalogParserPoints.CatalogItemNameStart + 1
			});
			
			int count = startEndSubstring.Count;

			int k = 0;
			foreach (String rec in fileParser.GetRecords(fromPathFile,
				encoding,
				countExcludeFirstString))
			{
				k++;
				if (rec == null) continue;
				if (string.IsNullOrWhiteSpace(rec) == true) { continue; }
				int lenRow = rec.Length;

				if (lenRow < catalogParserPoints.CatalogMinLengthIncomingRow)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedLengthString, rec));
					continue;
				}

				String[] record = { "", "" };

				//00801             TWISTED  
				//0				1			

				for (int j = 0; j < count; j++)
				{
					if (startEndSubstring[j].End < lenRow)
					{
						record[j] = rec.Substring(startEndSubstring[j].Start, startEndSubstring[j].Length);
					}
					else //startEndSubstring[i].End >= lenRow
					{
						if (startEndSubstring[j].Start < lenRow)
						{
							record[j] = rec.Substring(startEndSubstring[j].Start, lenRow - startEndSubstring[j].Start);
						}
					}
				}

				Branch newBranch = new Branch();
				//BranchString newBranchString = new BranchString();
				string branchCodeERP = record[0];
				newBranch.BranchCodeERP = branchCodeERP;
				string branchCodeLocal = k.ToString();
				if (branchCodeERP.Length > 2)
				{
					branchCodeLocal = branchCodeERP.Substring(2);
				}

				if (branchCodeLocal.Length < 3)
				{
					branchCodeLocal = branchCodeLocal.PadLeft(3, '0');
				}
				if (branchCodeLocal.Length > 3)
				{
					branchCodeLocal = branchCodeLocal.Substring(0, 3);
				}
				newBranch.BranchCodeLocal = branchCodeLocal;

				//CodeERP
				string ret = BranchValidate.CodeErpValidate(newBranch.BranchCodeERP);
				if (ret != String.Empty)  //Error
				{
					this._errorBitList.Add(new BitAndRecord
					{
						Bit = (int)ConvertDataErrorCodeEnum.InvalidValue,
						Record = ret + " : " + record.JoinRecord(separator),
						ErrorType = MessageTypeEnum.Error
					});
					continue;
				}
				else //	Error  retBit == 0 
				{
					//Name
					if (record.Length > 1)
					{
						string ret1 = BranchValidate.NameValidate(record[1]);
						if (ret1 == String.Empty)
						{
							string name = record[1].ReverseDosHebrew(invertLetter, rt2lf);
							newBranch.Name = name;
						}
						else
						{
							this._errorBitList.Add(new BitAndRecord
							{
								Bit = (int)ConvertDataErrorCodeEnum.InvalidValue,
								Record = record.JoinRecord(separator),
								ErrorType = MessageTypeEnum.Error
							});
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
