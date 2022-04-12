using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.ServiceLocation;
using System.Data.Entity.Core.Objects;
using System.Data.SqlServerCe;
using System.Data;
using ErikEJ.SqlCe;
using System.Globalization;
using System.Data.SqlClient;
using System.IO;
using NLog;
using System.ComponentModel;
using Count4U.Model.Extensions;

namespace Count4U.Model
{
	public abstract class BaseCopyBulkRepository : BaseADORepository
	{
		
		public static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public BaseCopyBulkRepository(
			IConnectionADO connectionADO,
			IDBSettings dbSettings,
			ILog log, 
			IServiceLocator serviceLocator
			) :
			base(connectionADO, dbSettings, log, serviceLocator)
		{
		
		}

		/// <summary>
		/// Копировать из обной БД в другую - таблица одна и та же
		/// </summary>
		/// <param name="tableName">Имя таблицы</param>
		/// <param name="fromPathDB">путь БД из которой</param>
		/// <param name="toPathDB">путь БД куда</param>
		protected void CopyTable(string tableName, string fromPathDB, string toPathDB)
		{
			//Localization.Resources.Log_TraceRepositoryResult1008%"Start Process: Copy Table [{0}] Via BulkCopy"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1008, tableName));
			//Localization.Resources.Log_TraceRepositoryResult1009%"From DB [{0}] To DB [{1}] "
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1009, fromPathDB, toPathDB));

			string queryString = "SELECT " + tableName + ".*  FROM  " + tableName + ";";
			string connectionString = this.BuildADOConnectionStringBySubFolder(fromPathDB);

			SqlCeDataReader reader = null;
			using (SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString))
			{
				try
				{
					sqlCeConnection.Open();
					SqlCeCommand command = new SqlCeCommand(queryString, sqlCeConnection);

					reader = command.ExecuteReader();
					this.DoBulkCopy(tableName, reader, true, toPathDB);
					reader.Close();
				}
				catch (Exception error)
				{
					_logger.ErrorException("CopyTable", error);
					this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);
				}
				finally
				{
					if (reader != null) reader.Close();
					sqlCeConnection.Close();
				}
			}
			//Localization.Resources.Log_TraceRepositoryResult1055%"Copy Table [{0}] Via BulkCopy"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1055, tableName));
		}


		protected void DoBulkCopy(string tableName, IDataReader reader, bool keepNulls, string toPathDB)
		{
			string connectionString = this.BuildADOConnectionStringBySubFolder(toPathDB);
			SqlCeBulkCopyOptions options = new SqlCeBulkCopyOptions();

			if (keepNulls == true)
			{
				options = options |= SqlCeBulkCopyOptions.KeepNulls;
			}
			using (SqlCeBulkCopy bc = new SqlCeBulkCopy(connectionString, options))
			{
				try
				{
					bc.DestinationTableName = tableName;
					bc.WriteToServer(reader);
				}
				catch (Exception error)
				{
					_logger.ErrorException("DoBulkCopy", error);
					this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);
				}

			}
		}


		/// <summary>
		///  Копирование из IEnumerable список данных в таблицу 
		/// </summary>
		/// <typeparam name="T">тип данных Count4U.T</typeparam>
		/// <param name="tableName">имя таблицы</param>
		/// <param name="data">IEnumerable список T</param>
		/// <param name="keepNulls">true</param>
		/// <param name="toPathDB">путь до БД куда писать</param>
		/// <param name="columnMappings">список мапингов между типом Count4U.T и полем в таблице</param>
		protected void DoBulkCopyMapping<T>(string tableName,
			IEnumerable<T> data, bool keepNulls, string toPathDB, List<string[]> columnMappings = null, string fileDB = "Count4UDB.sdf")
		{
			string connectionString = this.BuildADOConnectionStringBySubFolder(toPathDB, fileDB);
			SqlCeBulkCopyOptions options = new SqlCeBulkCopyOptions();

			if (keepNulls == true) { options = options |= SqlCeBulkCopyOptions.KeepNulls;	}

			using (SqlCeBulkCopy bulkCopy = new SqlCeBulkCopy(connectionString, options))
			{
				try
				{
					bulkCopy.DestinationTableName = tableName;
					if (columnMappings != null)
					{
						foreach (var columnMapping in columnMappings)
						{  
							bulkCopy.ColumnMappings.Add(columnMapping[0], columnMapping[1]);
						}
					}
					else
					{
						SetColumnMappings<T>(bulkCopy);
					}
					bulkCopy.WriteToServer(data.ToList());
				}
				catch (Exception error)
				{
					_logger.ErrorException("DoBulkCopyMapping<T> - table name:" + tableName , error);
					this.Log.Add(MessageTypeEnum.ErrorDB,"DoBulkCopyMapping<T> - table name:" + tableName + "error : "+  error.Message + ":" + error.StackTrace);
				}

			}
		}

		public long CountRow(string tableName, string toPathDB, string fileDB = "Count4UDB.sdf")
		{
			string connectionString = this.BuildADOConnectionStringBySubFolder(toPathDB, fileDB);

			using (SqlCeConnection sourceConnection =
					   new SqlCeConnection(connectionString))
			{
				sourceConnection.Open();

				// Perform an initial count on the destination table.
				SqlCeCommand commandRowCount = new SqlCeCommand(
					"SELECT COUNT(*) FROM " + tableName + ";",
					sourceConnection);
				long count = System.Convert.ToInt32(commandRowCount.ExecuteScalar());
				sourceConnection.Close();
				return count;
			}

		}

		protected void DeleteTable(string tableName, string pathDB, string fileDB = "Count4UDB.sdf")
		{

			string sql1 = "DELETE FROM  [" + tableName + "]";
			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB, fileDB);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);

				//Localization.Resources.Log_TraceRepositoryResult1003%" [{0}] in DB {1}"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, tableName, pathDB));
				//Localization.Resources.Log_TraceRepository1001%"Start Process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, tableName));
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, tableName));
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("DeleteTable", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
			
		}

		protected static void SetColumnMappings<T>(SqlCeBulkCopy bulkCopy)
		{
			var props = TypeDescriptor.GetProperties(typeof(T))
			.Cast<PropertyDescriptor>()
			.ToArray();

			List<PropertyDescriptor> notBulkProperty = new List<PropertyDescriptor>();
			bulkCopy.ColumnMappings.Clear();
			foreach (PropertyDescriptor propertyInfo in props)
			{
				bool notBulk = propertyInfo.Attributes.OfType<PropertyNotBulkAttribute>().Any();
				if (propertyInfo.Name != "ID")// && propertyInfo.Name != "BarcodeByteNotDB") // && propertyInfo.Name != "Uid")
				{
					if (notBulk == false)
					{
						bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
					}
					else
					{
						notBulkProperty.Add(propertyInfo);
					}
				}
			}
		}
		

		protected static void CopyBulkIEnumerable<T>(SqlCeBulkCopy bulkCopy, IEnumerable<T> data)
		{
			SetColumnMappings<T>(bulkCopy);
			bulkCopy.WriteToServer(data.ToList());
		}

		// пример
		public static void BulkInsert<T>(string connection, string tableName, IList<T> list)
		{
			using (var bulkCopy = new SqlBulkCopy(connection))
			{
				bulkCopy.BatchSize = list.Count;
				bulkCopy.DestinationTableName = tableName;
				var table = new DataTable();
				var props = TypeDescriptor.GetProperties(typeof(T))
					//Dirty hack to make sure we only have system data types            
					//i.e. filter out the relationships/collections                              
				.Cast<PropertyDescriptor>()
				.Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
				.ToArray();
				foreach (var propertyInfo in props)
				{
					bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
					table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
				}
				var values = new object[props.Length];
				foreach (var item in list)
				{
					for (var i = 0; i < values.Length; i++)
					{
						values[i] = props[i].GetValue(item);
					}
					table.Rows.Add(values);
				}
				bulkCopy.WriteToServer(table);
			}
		}
		//вызов этой функции
		//var imports = new List<Product>();//Load up the imports 
		////Pass in cnx, tablename, and list of  imports
		//BulkInsert(context.Database.Connection.ConnectionString, "Products", imports);

	}
   /// <summary>
   /// пример
   /// </summary>
	class Program
	{
		static void Main1()
		{             // Создаем ридер для источника, с которым ходим работать.             
			var reader = GetReader();               // Строка подключения.   
			var connectionString = @"Server={сервер};initial catalog={база данных};Integrated Security=true";
			// Создаем объект загрузчика SqlBulkCopy, указываем таблицу назначения и загружаем.          
			using (var loader = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.Default))
			{
				loader.ColumnMappings.Add(0, 2);
				loader.ColumnMappings.Add(1, 1);
				loader.ColumnMappings.Add(2, 3);
				loader.ColumnMappings.Add(3, 4);
				loader.DestinationTableName = "Customers";
				loader.WriteToServer(reader); Console.WriteLine("Загрузили!");
			}
			Console.ReadLine();
		}
		static IDataReader GetReader()
		{
			var sourceFilepath = AppDomain.CurrentDomain.BaseDirectory + "sqlbulktest.csv";
			var convertTable = GetConvertTable();
			var constraintsTable = GetConstraintsTable();
			var reader = new CSVReader(sourceFilepath, constraintsTable, convertTable);
			return reader;
		}
		static Func<string, bool>[] GetConstraintsTable()
		{
			var constraintsTable = new Func<string, bool>[4];
			constraintsTable[0] = x => !string.IsNullOrEmpty(x);
			constraintsTable[1] = constraintsTable[0];
			constraintsTable[2] = x => true;
			constraintsTable[3] = x => true; 
			return constraintsTable;
		}
		static Func<string, object>[] GetConvertTable()
		{
			var convertTable = new Func<object, object>[4];            
			// Функция преобразования первого столбца csv файла (фамилия)     
			convertTable[0] = x => x;               // Второго (имя)        
			convertTable[1] = x => x;               // Третьего (дата)         
			// Разбираем строковое представление даты по определенному формату.    
			convertTable[2] = x =>
			{
				DateTime datetime; if (DateTime.TryParseExact(x.ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out datetime))
				{
					return datetime;
				}
				return null;
			};               // Четвертого (промо код)     
			convertTable[3] = x => Convert.ToInt32(x); 
			return convertTable;
		}
	}

	public class CSVReader : IDataReader
	{
		readonly StreamReader _streamReader;
		readonly Func<string, object>[] _convertTable;
		readonly Func<string, bool>[] _constraintsTable;
		string[] _currentLineValues;
		string _currentLine;
		// Конструктор ридера CSV-файла.      
		// Передаем полный абсолютный путь к файлу, таблицы функций ограничений и преобразований. 
		public CSVReader(string filepath, Func<string, bool>[] constraintsTable, Func<string, object>[] convertTable)
		{
			_constraintsTable = constraintsTable;
			_convertTable = convertTable;
			_streamReader = new StreamReader(filepath);
			_currentLine = null;
			_currentLineValues = null;
		}
		// Возвращаем значение, используя одну из функций преобразования и обработку исключения.        
		// Это обезопасит нас от прерывания загрузки данных.      
		public object GetValue(int i)
		{
			try
			{
				return _convertTable[i](_currentLineValues[i]);
			}
			catch (Exception)
			{
				return null;
			}
		}
		// Чтение очередной строки.    
		// Используем функции ограничения для того, чтобы еще на этапе чтения понять, что строка      
		// вызовет исключения при передаче ее в SqlBulkCopy, поэтому мы пропускаем корректные строки.     
		public bool Read()
		{
			if (_streamReader.EndOfStream) return false;
			_currentLine = _streamReader.ReadLine();
			// В случае, если значения будут содержать символ ";" это работать не будет,        
			// и придется использовать более сложный алгоритм разбора. 
			_currentLineValues = _currentLine.Split(';');
			var invalidRow = false;
			for (int i = 0; i < _currentLineValues.Length; i++)
			{
				if (!_constraintsTable[i](_currentLineValues[i]))
				{
					invalidRow = true; break;
				}
			} return !invalidRow || Read();
		}           // Возвращем число столбцов в csv файле.       
		// Нам заранее известно, что 4, поэтому не будем усложнять код.       
		public int FieldCount { get { return 4; } }
		// Освобождаем ресурсы. Закрываем поток.      
		public void Dispose() { _streamReader.Close(); }
		// ... множестве нереализованных методов IDataReader, которые здесь не нужны.        
		#region Implementation of IDataRecord
		public string GetName(int i) { throw new NotImplementedException(); }
		public string GetDataTypeName(int i) { throw new NotImplementedException(); }
		public System.Type GetFieldType(int i) { throw new NotImplementedException(); }
		public int GetValues(object[] values) { throw new NotImplementedException(); }
		public int GetOrdinal(string name) { throw new NotImplementedException(); }
		public bool GetBoolean(int i) { throw new NotImplementedException(); }
		public byte GetByte(int i) { throw new NotImplementedException(); }
		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{ throw new NotImplementedException(); }
		public char GetChar(int i)
		{
			throw new NotImplementedException();
		}
		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{ throw new NotImplementedException(); }
		public Guid GetGuid(int i)
		{ throw new NotImplementedException(); }
		public short GetInt16(int i)
		{
			throw new NotImplementedException();
		}
		public int GetInt32(int i)
		{
			throw new NotImplementedException();
		}
		public long GetInt64(int i)
		{
			throw new NotImplementedException();
		}
		public float GetFloat(int i)
		{
			throw new NotImplementedException();
		}
		public double GetDouble(int i)
		{
			throw new NotImplementedException();
		}
		public string GetString(int i)
		{
			throw new NotImplementedException();
		}
		public decimal GetDecimal(int i)
		{
			throw new NotImplementedException();
		}
		public DateTime GetDateTime(int i)
		{
			throw new NotImplementedException();
		}
		public IDataReader GetData(int i)
		{
			throw new NotImplementedException();
		}
		public bool IsDBNull(int i)
		{
			throw new NotImplementedException();
		}
		object IDataRecord.this[int i]
		{
			get { throw new NotImplementedException(); }
		}
		object IDataRecord.this[string name]
		{
			get { throw new NotImplementedException(); }
		}
		#endregion
		#region Implementation of IDataReader
		public void Close() { throw new NotImplementedException(); }
		public DataTable GetSchemaTable() { throw new NotImplementedException(); }
		public bool NextResult()
		{
			throw new NotImplementedException();
		}
		public int Depth
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public bool IsClosed
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public int RecordsAffected
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		#endregion
	}


}

//DataSet dataSet = new DataSet();

//dataSet.ReadXml(xmlFilename);

//foreach (DataTable table in dataSet.Tables)
//{
//    sqlBulkCopy.DestinationTableName = table.TableName;
//    sqlBulkCopy.ColumnMappings.Clear();
//    foreach (DataColumn column in table.Columns)
//    {
//        sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
//    }
//    Console.WriteLine("[{0:G}] Write table: {1}...", DateTime.Now, table.TableName);
//    sqlBulkCopy.WriteToServer(table);
//    Console.WriteLine("[{0:G}] Write table: {1} complete", DateTime.Now, table.TableName);
//}

//---
//var sbCopy= new SqlBulkCopy(connectionString);
//sbCopy.DestinationTableName = "TableName";
//sbCopy.WriteToServer(entitiesList.AsDataReader()); 
 //======
	//public IDataReader DoBulkCopy<T>(List<T> DataToInsert, string DestinationTableName)
	//{
	//    var BulkCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString, SqlBulkCopyOptions.KeepNulls);
	//    BulkCopy.BulkCopyTimeout = 0;
	//    BulkCopy.BatchSize = 50;
	//    BulkCopy.DestinationTableName = DestinationTableName;

	//    IDataReader reader = DataToInsert.AsDataReader();
	//    BulkCopy.WriteToServer(reader);
	//    BulkCopy.Close();

	//    return reader;
	//}
