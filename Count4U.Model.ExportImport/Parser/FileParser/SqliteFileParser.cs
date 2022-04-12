using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Data.SqlTypes;
using System.Data;
using Devart.Data;
using Devart.Data.SQLite;


namespace Count4U.Model.Count4U
{
	public class SqliteFileParser : IFileParser
	{
		private readonly ILog _log;

		public SqliteFileParser(ILog log)
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
			if (param == "")
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.TableNameIsEmpty, filePath));
				yield return null;
			}
			else
			{
				string extension = System.IO.Path.GetExtension(filePath).ToLower();
				if (extension == ".db3" || extension == ".db")
				{
					using (SQLiteConnection con = new SQLiteConnection())
					{
						con.ConnectionString = @"Data Source=" + filePath + @";Read Uncommitted=true; Pooling=false;";
						con.Open();
						//TODO
						// Проверить есть ли таблица
						DataTable table = con.GetSchema("Tables");
						bool isExistInDB = false;
						foreach (System.Data.DataRow row in table.Rows)
						{
							string name = row["Name"].ToString();
							if (param.Trim().ToUpper() == name.Trim().ToUpper())
							{
								isExistInDB = true;
								break;
							}
						}
						table.Clear();
						table = null;

						if (isExistInDB == false)
						{
							con.Close();
							yield return null;
						}
						else
						{
							string sql1 = String.Format(@"SELECT * FROM {0} ", param); //CurrentInventory";
							using (SQLiteCommand command = new SQLiteCommand(sql1, con))
							{

								Devart.Data.SQLite.SQLiteDataReader dataReader = null;
								try
								{
									dataReader = command.ExecuteReader();
								}
								catch (Devart.Data.SQLite.SQLiteException exception)
								{
									Log.Add(MessageTypeEnum.Error, exception.Message + " in DB file : " + filePath);
									if (dataReader != null)
									{
										dataReader.Close();
										dataReader.Dispose();
										dataReader = null;
									}
								}


								if (dataReader == null)
								{
									con.Close();
									con.Dispose();
									yield return null;
								}

								else
								{
									int countColumn = dataReader.FieldCount;
									List<string> columnNames = new List<string>();
									for (int i = 0; i < countColumn; i++)
									{
										columnNames.Add(dataReader.GetName(i).ToString());
									}
									String[] aRecord = columnNames.ToArray();
									yield return aRecord;
								}


								if (dataReader.EndOfData == true)
								{
									Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsEmpty, filePath));
									if (dataReader != null)
									{
										dataReader.Close();
										dataReader.Dispose();
										dataReader = null;
									}
									con.Close();
									con.Dispose();
									con.Close();
									yield return null;
								}


								else if (dataReader.HasRows == true)
								{
									while (dataReader.Read())
									{
										int countColumn = dataReader.FieldCount;
										List<string> records = new List<string>();

										for (int i = 0; i < countColumn; i++)
										{
											if (dataReader.IsDBNull(i) == false)
											{
												records.Add(dataReader.GetValue(i).ToString());
											}
											else
											{
												records.Add("");
											}
										}
										String[] aRecord = records.ToArray();
										yield return aRecord;
									}
								}
								if (dataReader != null)
								{
									dataReader.Close();
									dataReader.Dispose();
									dataReader = null;
								}
							}
							con.Close();
							con.Dispose();

							GC.Collect();
							GC.WaitForPendingFinalizers();
							GC.Collect();
							GC.Collect();
						}
					}
				}
				else
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileXlsxExpected, filePath));
					yield return null;
				}
			}
		}

		public IEnumerable<object[]> GetRow(string filePath, Encoding encoding,
		String[] separators, int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		{
			if (param == "")
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.TableNameIsEmpty, filePath));
				yield return null;
			}
			else
			{
				string extension = System.IO.Path.GetExtension(filePath).ToLower();
				if (extension == ".db3" || extension == ".db")
				{
					using (SQLiteConnection con = new SQLiteConnection())
					{
						con.ConnectionString = @"Data Source=" + filePath + @";Read Uncommitted=true; Pooling=false;";
						con.Open();
						//TODO
						// Проверить есть ли таблица
						//TableNotExistInDB 
						//try{
						DataTable table = con.GetSchema("Tables");
						bool isExistInDB = false;
						foreach (System.Data.DataRow row in table.Rows)
						{
							string name = row["Name"].ToString();
							if (param.Trim().ToUpper() == name.Trim().ToUpper())
							{
								isExistInDB = true;
								break;
							}
						}

						table.Clear();
						table = null;

						if (isExistInDB == false)
						{
							con.Close();
							yield return null;
						}
						else
						{
							string sql1 = String.Format(@"SELECT * FROM {0} ", param); //CurrentInventory";
							using (SQLiteCommand command = new SQLiteCommand(sql1, con))
							{
								Devart.Data.SQLite.SQLiteDataReader dataReader = null;

								try
								{
									dataReader = command.ExecuteReader();
								}
								catch (Devart.Data.SQLite.SQLiteException exception)
								{
									Log.Add(MessageTypeEnum.Error, exception.Message + " in DB file : " + filePath);
									if (dataReader != null)
									{
										dataReader.Close();
										dataReader.Dispose();
										dataReader = null;
									}
								}


								if (dataReader == null)
								{
									con.Close();
									con.Dispose();
									yield return null;
								}

								else
								{
									int countColumn = dataReader.FieldCount;
									List<string> columnNames = new List<string>();
									for (int i = 0; i < countColumn; i++)
									{
										columnNames.Add(dataReader.GetName(i).ToString());
									}
									object[] aRecord = columnNames.ToArray();
									yield return aRecord;
								}


								if (dataReader.EndOfData == true)
								{
									Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsEmpty, filePath));
									if (dataReader != null)
									{
										dataReader.Close();
										dataReader.Dispose();
										dataReader = null;
									}
									con.Close();
									con.Dispose();
									yield return null;
								}


								else if (dataReader.HasRows == true)
								{
									while (dataReader.Read())
									{
										int countColumn = dataReader.FieldCount;
										object[] row = new object[29];
										int ret = dataReader.GetValues(row);
										yield return row;

									}
								}

								if (dataReader != null)
								{
									dataReader.Close();
									dataReader.Dispose();
									dataReader = null;
								}
							}
							con.Close();
							con.Dispose();
							GC.Collect();
							GC.WaitForPendingFinalizers();
							GC.Collect();
							GC.Collect();

						}
					}
				}
				else
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileXlsxExpected, filePath));
					yield return null;
				}

			}
		}


		public IEnumerable<String> GetRecords(string filePath, Encoding encoding,
			int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		{
		yield return "";


		}
				

		//private StreamReader GetStreamReader(string filePath, Encoding encoding)
		//{
		//	//Log.Add("File Path is " + filePath);
		//	if (File.Exists(filePath) == false)
		//	{
		//		Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsNotExist, filePath));
		//		return null;
		//		//			Log.Add("File " + filePath + "does not exist.");
		//		//throw new Exception("File " + filePath + "does not exist.");
		//	}
		//	FileStream fs = File.OpenRead(filePath);
		//	StreamReader sr = new StreamReader(fs, encoding); //Encoding.GetEncoding("windows-1255"));
		//	return sr;
		//}

		#region IFileParser Members


		public void FinallyMethod()
		{
			throw new NotImplementedException();
		}

		#endregion
	}

}

