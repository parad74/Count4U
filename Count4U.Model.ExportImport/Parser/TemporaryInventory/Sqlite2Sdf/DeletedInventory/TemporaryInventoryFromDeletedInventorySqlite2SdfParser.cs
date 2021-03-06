using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;
using System.IO;

namespace Count4U.Model.Count4U
{
	//парсим *.db3 и записываем AnaliticDB.TemporaryInventory
	public class TemporaryInventoryFromDeletedInventorySqlite2SdfParser : ITemporaryInventorySQLiteParser
	{
		protected IFileParser _fileParser;
		private readonly ILog _log;
		private Dictionary<string, TemporaryInventory> _temporaryInventoryDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;


		public TemporaryInventoryFromDeletedInventorySqlite2SdfParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._temporaryInventoryDictionary = new Dictionary<string, TemporaryInventory>();
			this._errorBitList = new List<BitAndRecord>();

			this._fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.SqliteFileParser.ToString());
		}

		public Dictionary<string, TemporaryInventory> TemporaryInventoryDictionary
		{
			get { return this._temporaryInventoryDictionary; }
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

	
		public IEnumerable<TemporaryInventory> GetTemporaryInventorys(
			string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString, 
			Dictionary<string, string> temporaryInventoryFromDBDictionary, 
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._temporaryInventoryDictionary.Clear();
			this._errorBitList.Clear();
		  //	string toDBPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);	  //Текущая БД

			Dictionary<string, string> columnNameInRecordDictionary = parms.GetIPAdvancedFieldNameDictionaryFromParm();			   //ToDO?

			string separator = "|";

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			string tableName = "DeletedInventory";
			int rowCount = 0;
			int colCount = 5;
			string[] currentInventoryColumnName = new string[colCount];
			currentInventoryColumnName = FillTableTemporaryInventoryColumnNames(currentInventoryColumnName);
			Dictionary<string, int> dictionaryColumnNumbers = FillDictionryColumnNumbers(currentInventoryColumnName);


			int indexOldUid = -1;
			int indexDateModified = -1;
			int indexNewUid = -1;
			int indexTag = -1;
		
			//[0]	"OldUid"	
			//[1]	     "DateModified"	
			//[2]	"NewUid"	
			foreach (object[] objects in this._fileParser.GetRecords(fromPathFile,
			encoding, separators,
			countExcludeFirstString, tableName))
			{
				if (objects == null) continue;

				rowCount++;
				string[] record = new string[] { "rowCount = " + rowCount };

				try
				{
					record = objects as string[];
				}
				catch
				{
					Log.Add(MessageTypeEnum.Error,
					String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
				}

				if (record == null) continue;
				if (rowCount == 1) // header of Table
				{
					colCount = CheckTableColumnNames(currentInventoryColumnName, record, tableName);
					// 	dictionaryColumnNumbers - TODO получить индексы нужных полей, не найденых == -1
					{
						indexOldUid = "OldUid".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexNewUid = "NewUid".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexDateModified = "DateModified".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);
						indexTag = "Tag".GetIndexColumnByName(dictionaryColumnNumbers, tableName, colCount, Log);

					}
				}
				else		   //row from table
				{
					int countRecord = record.Length;
					if (countRecord < colCount)
					{
						Log.Add(MessageTypeEnum.Error,
							String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
						continue;
					}

					//[0]	"ID"	
					//[1]	 "OldUid"	
					//[2]	"NewUid"	
					//[3]	 "DateModified"	
					//[4]	"Tag"	
					TemporaryInventory newTemporaryInventory = new TemporaryInventory();
					newTemporaryInventory.OldUid = record[indexOldUid].CutLength(249);
					newTemporaryInventory.DateModified = record[indexDateModified].CutLength(49);
					newTemporaryInventory.NewUid = record[indexNewUid].CutLength(249);
					newTemporaryInventory.Operation = "DELETE";
					newTemporaryInventory.Domain = "InventProduct";
					newTemporaryInventory.Description = "Delete InventProduct (from DeletedInventory) :" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
					string fileName = Path.GetFileName(fromPathFile);
					newTemporaryInventory.DbFileName = fileName.CutLength(249);
					newTemporaryInventory.Tag = record[indexTag].CutLength(249);
					
					yield return newTemporaryInventory;
				}

			}
		
			
		}

		private string[] FillTableTemporaryInventoryColumnNames(string[] currentInventoryColumnName)
		{																	   
			currentInventoryColumnName[0] = "ID";
			currentInventoryColumnName[1] = "OldUid";
			currentInventoryColumnName[2] = "NewUid";
			currentInventoryColumnName[3] = "DateModified";
			currentInventoryColumnName[4] = "Tag";
	
			return currentInventoryColumnName;
		}

		private Dictionary<string, int> FillDictionryColumnNumbers(string[] columnNames)
		{
			Dictionary<string, int> dictionaryColumnNumbers = new Dictionary<string, int>();
			if (columnNames == null) return dictionaryColumnNumbers;
			int columCount = columnNames.Length;
			for (int i = 0; i < columCount; i++)
			{
				string columnName = columnNames[i];
				dictionaryColumnNumbers[columnName] = i;
			}
			return dictionaryColumnNumbers;
		}

		private int CheckTableColumnNames(string[] columnNames, string[] record, string fromTable)
		{
			if (columnNames == null) return -1;
			if (record == null) return -1;

			int columCount = columnNames.Length;
			int columCount1 = record.Length;
			if (columCount1 != columCount)
			{
				Log.Add(MessageTypeEnum.Error, String.Format("It is Expected  Different Number of Columns in {0} Table {1} : {2}", fromTable, columCount, columCount1));
			}
			int columCountMin = Math.Min(columCount, columCount1);
			for (int i = 0; i < columCountMin; i++)
			{
				string columnName = columnNames[i].ToLower().Trim();
				string columnName1 = record[i].ToLower().Trim();
				if (columnName != columnName1)
				{
					Log.Add(MessageTypeEnum.Error, String.Format("Column {0}  Different from Column {1} in {2} Table ", columnName, columnName1, fromTable));
				}
			}
			return columCountMin;
		}

	}
}
