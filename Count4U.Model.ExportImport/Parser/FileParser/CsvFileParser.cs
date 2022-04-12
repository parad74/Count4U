using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U
{
	/// <summary>
	/// Абстрактный класс - парсинг Csv документа
	/// </summary>
	public class CsvFileParser : IFileParser
	{
		private readonly ILog _log;

		public CsvFileParser(ILog log)
		{
			this._log = log;
		}

		public ILog Log
		{
			get { return this._log; }
		} 
		/// <summary>
		/// Получить список строк
		/// </summary>
		/// <param name="excludeFirstString">Исключить 1 строку из списка</param>
		/// <returns></returns>
		public IEnumerable<String[]> GetRecords(string filePath, Encoding encoding,
			String[] separators, int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		{
			string extension = System.IO.Path.GetExtension(filePath).ToLower();
			if (extension != ".xlsx" && extension != ".docx" && extension != ".doc" && extension != ".xls")
			{
				using (StreamReader reader = this.GetStreamReader(filePath, encoding))
				{
					if (reader == null) yield return null;

					if (reader.EndOfStream == true)
					{
						Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsEmpty, filePath));
						yield return null;
					}

					int count = 1;
					if (countExcludeFirstString > 0)
					{
						while (count <= countExcludeFirstString)
						{
							if (reader.EndOfStream == false)
							{
								reader.ReadLine();
								count++;
							}
							else
							{
								count++; //на случай если файл пустой надо выйти из бесконечного  while
								yield return null;
							}
						}
					}

					String[] aRecord;
					while (reader.EndOfStream == false)
					{
						string strRecord = reader.ReadLine();
						if (string.IsNullOrWhiteSpace(strRecord) == false)
						{
							aRecord = strRecord.Split(separators, StringSplitOptions.None);
							yield return aRecord;
						}
					}
				}
			}
			else
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileTxtExpected, filePath));
				yield return null;
			}
		}

		public IEnumerable<String> GetRecords(string filePath, Encoding encoding,
			int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		{
			string extension = System.IO.Path.GetExtension(filePath).ToLower();
			if (extension != ".xlsx" && extension != ".docx" && extension != ".doc" && extension != ".xls")
			{
				using (StreamReader reader = this.GetStreamReader(filePath, encoding))
				{
					if (reader == null) yield return "";

					if (reader.EndOfStream == true)
					{
						Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsEmpty, filePath));
						yield return "";
						//throw new Exception("File " + filePath + " is Empty");
					}

					int count = 1;
					//if (countExcludeFirstString > 0)
					//{
					//	if (reader.EndOfStream == false)
					//	{
					//		if (count <= countExcludeFirstString)
					//		{
					//			reader.ReadLine();
					//			count++;
					//		}
					//	}
					//}

					if (countExcludeFirstString > 0)
					{
						while (count <= countExcludeFirstString)
						{
							if (reader.EndOfStream == false)
							{
								reader.ReadLine();
								count++;
							}
							else
							{
								count++; //на случай если файл пустой надо выйти из бесконечного  while
								yield return null;
							}
						}
					}

					while (reader.EndOfStream == false)
					{
						string strRecord = reader.ReadLine();
						if (string.IsNullOrWhiteSpace(strRecord) == false)
						{
							yield return strRecord;
						}
					}
				}
			}
			else
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileTxtExpected, filePath));
				yield return null;
			}
		}
	
		private StreamReader GetStreamReader(string filePath, Encoding encoding)
		{
			//Log.Add("File Path is " + filePath);
			if (File.Exists(filePath) == false)
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsNotExist, filePath));
				return null;
   				//			Log.Add("File " + filePath + "does not exist.");
				//throw new Exception("File " + filePath + "does not exist.");
			}
			FileStream fs = File.OpenRead(filePath);
			StreamReader sr = new StreamReader(fs, encoding);//Encoding.GetEncoding("windows-1255"));
			return sr;
		}

		#region IFileParser Members


		IEnumerable<object[]> IFileParser.GetRow(string filePath, Encoding encoding, string[] separators, int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IFileParser Members


		public void FinallyMethod()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}


