using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using ClosedXML.Excel;

namespace Count4U.Model.Count4U
{
	public class ExcelFileParser : IFileParser
	{
		private readonly ILog _log;

		public ExcelFileParser(ILog log)
		{
			this._log = log;
		}

		public ILog Log
		{
			get { return this._log; }
		} 
//		range.FirstCell()
//range.FirstCellUsed()
//range.FirstColumn()
//range.FirstColumnUsed()
//range.FirstRow()
//range.FirstRowUsed()

//range.LastCell()
//range.LastCellUsed()
//range.LastColumn()
//range.LastColumnUsed()
//range.LastRow()
//range.LastRowUsed()

		//private static void Main()
		//{
		//	List<String> categories;
		//	List<String> companies;
		//	ExtractCategoriesCompanies("NorthwindData.xlsx", out categories, out companies);

		//	// Do something with the categories and companies
		//}

		//private static void ExtractCategoriesCompanies(string northwinddataXlsx, out List<string> categories, out List<string> companies)
		//{
		//	categories = new List<string>();
		//	const int coCategoryId = 1;
		//	const int coCategoryName = 2;

		//	var wb = new XLWorkbook(northwinddataXlsx);
		//	var ws = wb.Worksheet("Data");

		//	// Look for the first row used
		//	var firstRowUsed = ws.FirstRowUsed();

		//	// Narrow down the row so that it only includes the used part
		//	var categoryRow = firstRowUsed.RowUsed();

		//	// Move to the next row (it now has the titles)
		//	categoryRow = categoryRow.RowBelow();

		//	// Get all categories
		//	while (!categoryRow.Cell(coCategoryId).IsEmpty())
		//	{
		//		String categoryName = categoryRow.Cell(coCategoryName).GetString();
		//		categories.Add(categoryName);

		//		categoryRow = categoryRow.RowBelow();
		//	}

		//	// There are many ways to get the company table.
		//	// Here we're using a straightforward method.
		//	// Another way would be to find the first row in the company table
		//	// by looping while row.IsEmpty()

		//	// First possible address of the company table:
		//	var firstPossibleAddress = ws.Row(categoryRow.RowNumber()).FirstCell().Address;
		//	// Last possible address of the company table:
		//	var lastPossibleAddress = ws.LastCellUsed().Address;

		//	// Get a range with the remainder of the worksheet data (the range used)
		//	var companyRange = ws.Range(firstPossibleAddress, lastPossibleAddress).RangeUsed();

		//	// Treat the range as a table (to be able to use the column names)
		//	var companyTable = companyRange.AsTable();

		//	// Get the list of company names
		//	companies = companyTable.DataRange.Rows()
		//		.Select(companyRow => companyRow.Field("Company Name").GetString())
		//		.ToList();
		//}


		//private static void ExtractCategoriesCompanies(string northwinddataXlsx, out List<string> categories, out List<string> companies)
		//{
		//	categories = new List<string>();
		//	const int coCategoryId = 1;
		//	const int coCategoryName = 2;

		//	var wb = new XLWorkbook(northwinddataXlsx);
		//	var ws = wb.Worksheet(0);

		//	// Look for the first row used
		//	var firstRowUsed = ws.FirstRowUsed();

		//	// Narrow down the row so that it only includes the used part
		//	var categoryRow = firstRowUsed.RowUsed();

		//	// Move to the next row (it now has the titles)
		//	categoryRow = categoryRow.RowBelow();

		//	// Get all categories
		//	while (!categoryRow.Cell(coCategoryId).IsEmpty())
		//	{
		//		String categoryName = categoryRow.Cell(coCategoryName).GetString();
		//		categories.Add(categoryName);

		//		categoryRow = categoryRow.RowBelow();
		//	}

		//	// There are many ways to get the company table.
		//	// Here we're using a straightforward method.
		//	// Another way would be to find the first row in the company table
		//	// by looping while row.IsEmpty()

		//	// First possible address of the company table:
		//	var firstPossibleAddress = ws.Row(categoryRow.RowNumber()).FirstCell().Address;

		//	string[] row = ws.Row(categoryRow.RowNumber()).Cells().CastTo<string[]>();//z

		//	// Last possible address of the company table:
		//	var lastPossibleAddress = ws.LastCellUsed().Address;

		//	// Get a range with the remainder of the worksheet data (the range used)
		//	var companyRange = ws.Range(firstPossibleAddress, lastPossibleAddress).RangeUsed();

		//	// Treat the range as a table (to be able to use the column names)
		//	var companyTable = companyRange.AsTable();

		//	// Get the list of company names
		//	companies = companyTable.DataRange.Rows()
		//		.Select(companyRow => companyRow.Field("Company Name").GetString())
		//		.ToList();
		//}


		/// <summary>
		/// Получить список строк
		/// </summary>
		/// <param name="excludeFirstString">Исключить countExcludeFirstString строк из списка</param>
		/// <returns></returns>
		public IEnumerable<String[]> GetRecords(string filePath, Encoding encoding,
			String[] separators, int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		{
			int worksheetN = 1;
			if (sheetNumberXlsx > 1)
			{
				worksheetN = sheetNumberXlsx;
			}

			string extension = System.IO.Path.GetExtension(filePath).ToLower();
			if (extension == ".xlsx")
			{
				var wb = new XLWorkbook(filePath); if (wb == null) yield return null;
				if (wb.Worksheets.Count < worksheetN)
				{
					Log.Add(MessageTypeEnum.Error,
						String.Format("In .xlsx file {0} the Expected Number of Sheet [ {1} ] is more than there are Sheets  in Workbook [ {2 }]  ",
						 filePath, worksheetN, wb.Worksheets.Count));
					yield return null;
				}
				else
				{
					var ws = wb.Worksheet(worksheetN); if (ws == null) yield return null;


					if (string.IsNullOrWhiteSpace(param) == false)
					{
						if (ws.Name.Trim().ToUpper() != param.Trim().ToUpper())
						{
							Log.Add(MessageTypeEnum.WarningParser,
								String.Format("In .xlsx file {0} the Expected Name of Sheet [ {1} ] is Not Equal to the Incoming Name of Sheet [ {2} ] ",
								 filePath, param, ws.Name));
						}
					}
					//ws.Cell(1, 1);

					int count = 1;
					IXLRow firstRow;
					IXLRangeRow dataRow;
					int lastRow = 0;
					try
					{
						firstRow = ws.FirstRow();
						dataRow = firstRow.RowUsed();
						lastRow = ws.LastCellUsed().Address.RowNumber;
					}
					catch
					{
						Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsEmpty, filePath));
						dataRow = null;
					}

					if (dataRow == null) yield return null;

					//if (countExcludeFirstString > 0)
					//{
					//	if (dataRow != null)
					//	{
					//		if (count <= countExcludeFirstString)
					//		{

					//			dataRow = dataRow.RowBelow();
					//			count++;
					//		}
					//	}
					//}

					if (countExcludeFirstString > 0)
					{
						while (count <= countExcludeFirstString)
						{
							if (dataRow != null)
							{
								dataRow = dataRow.RowBelow();
								count++;
							}
							else
							{
								if( count == countExcludeFirstString)
								{
									count++;
								}
							}
						}
					}

					String[] aRecord;
					if (dataRow != null)
					{
						while (count <= lastRow) //7597
						{
							try
							{
								aRecord = dataRow.Cells().Select(x => x.GetString()).ToArray<string>();
							}
							catch (Exception exx)
							{
								aRecord = new String[] { "error", "count = " + (count + 1).ToString() };
								string errMessage =  " :  in row # " + (count + 1).ToString() + ":" + exx.Message;
								Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedMarker, errMessage));
							}
							dataRow = dataRow.RowBelow();
							count++;
							yield return aRecord;
						}
					}
				}
			}
			else
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileXlsxExpected, filePath));
				yield return null;
			}
	
		}



		public IEnumerable<String> GetRecords(string filePath, Encoding encoding,
			int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		{
			string extension = System.IO.Path.GetExtension(filePath).ToLower();
			if (extension == ".xlsx")
			{
				int worksheetN = 1;
				if (sheetNumberXlsx > 1)
				{
					worksheetN = sheetNumberXlsx;
				}


				var wb = new XLWorkbook(filePath); if (wb == null) yield return null;
				if (wb.Worksheets.Count < worksheetN)
				{
					Log.Add(MessageTypeEnum.Error,
						String.Format("In .xlsx file {0} the Expected Number of Sheet [ {1} ] is more than there are Sheets  in Workbook [ {2 }]  ",
						 filePath, worksheetN, wb.Worksheets.Count));
					yield return null;
				}
				else
				{
					var ws = wb.Worksheet(worksheetN); if (ws == null) yield return null;

					if (string.IsNullOrWhiteSpace(param) == false)
					{
						if (ws.Name.Trim().ToUpper() != param.Trim().ToUpper())
						{
							Log.Add(MessageTypeEnum.WarningParser,
								String.Format("In .xlsx file {0} the Expected Name of Sheet [ {1} ] is Not Equal to the Incoming Name of Sheet [ {2} ] ",
								 filePath, param, ws.Name));
						}
					}

					int count = 1;
					IXLRow firstRow;
					IXLRangeRow dataRow;
					int lastRow = 0;
					try
					{
						firstRow = ws.FirstRow();
						dataRow = firstRow.RowUsed();
						lastRow = ws.LastCellUsed().Address.RowNumber;
					}
					catch
					{
						Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsEmpty, filePath));
						dataRow = null;
					}

					if (dataRow == null) yield return null;

					//if (countExcludeFirstString > 0)
					//{
					//	if (dataRow != null)
					//	{
					//		if (count <= countExcludeFirstString)
					//		{

					//			dataRow = dataRow.RowBelow();
					//			count++;
					//		}
					//	}
					//}

					if (countExcludeFirstString > 0)
					{
						while (count <= countExcludeFirstString)
						{
							if (dataRow != null)
							{
								dataRow = dataRow.RowBelow();
								count++;
							}
							else
							{
								if (count == countExcludeFirstString)
								{
									count++;
								}
							}
						}
					}

					String[] aRecord;
					string retString = "";
					if (dataRow != null)
					{
						while (count <= lastRow) //7597
						{
							try
							{
								aRecord = dataRow.Cells().Select(x => x.GetString()).ToArray<string>();
							}
							catch (Exception exx)
							{
								aRecord = new String[] { "error", "count = " + (count + 1).ToString() };
								string errMessage = " :  in row # " + (count + 1).ToString() + ":" + exx.Message;
								Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedMarker, errMessage));
							}
							retString = aRecord.JoinRecord(",");
							dataRow = dataRow.RowBelow();
							count++;
							yield return retString;
						}
					}
				}
			}
			else
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileXlsxExpected, filePath));
				yield return null;
			}

		}
				

		IEnumerable<object[]> IFileParser.GetRow(string filePath, Encoding encoding, string[] separators, int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		//IEnumerable<object[]> GetRow(string filePath, Encoding encoding, string[] separators, int countExcludeFirstString,
		//	string param = "", int sheetNumberXlsx = 1)
		{
			int worksheetN = 1;
			if (sheetNumberXlsx > 1)
			{
				worksheetN = sheetNumberXlsx;
			}

			string extension = System.IO.Path.GetExtension(filePath).ToLower();
			if (extension == ".xlsx")
			{
				var wb = new XLWorkbook(filePath); if (wb == null) yield return null;
				if (wb.Worksheets.Count < worksheetN)
				{
					Log.Add(MessageTypeEnum.Error,
						String.Format("In .xlsx file {0} the Expected Number of Sheet [ {1} ] is more than there are Sheets  in Workbook [ {2 }]  ",
						 filePath, worksheetN, wb.Worksheets.Count));
					yield return null;
				}
				else
				{
					var ws = wb.Worksheet(worksheetN); if (ws == null) yield return null;

					if (string.IsNullOrWhiteSpace(param) == false)
					{
						if (ws.Name.Trim().ToUpper() != param.Trim().ToUpper())
						{
							Log.Add(MessageTypeEnum.WarningParser,
								String.Format("In .xlsx file {0} the Expected Name of Sheet [ {1} ] is Not Equal to the Incoming Name of Sheet [ {2} ] ",
								 filePath, param, ws.Name));
						}
					}
					//ws.Cell(1, 1);

					int count = 1;
					IXLRow firstRow;
					IXLRangeRow dataRow;
					int lastRow = 0;
					try
					{
						firstRow = ws.FirstRow();
						dataRow = firstRow.RowUsed();
						lastRow = ws.LastCellUsed().Address.RowNumber;
					}
					catch
					{
						Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsEmpty, filePath));
						dataRow = null;
					}

					if (dataRow == null) yield return null;

					//if (countExcludeFirstString > 0)
					//{
					//	if (dataRow != null)
					//	{
					//		if (count <= countExcludeFirstString)
					//		{

					//			dataRow = dataRow.RowBelow();
					//			count++;
					//		}
					//	}
					//}

					if (countExcludeFirstString > 0)
					{
						while (count <= countExcludeFirstString)
						{
							if (dataRow != null)
							{
								dataRow = dataRow.RowBelow();
								count++;
							}
							else
							{
								if (count == countExcludeFirstString)
								{
									count++;
								}
							}
						}
					}

					object[] aRecord;
					if (dataRow != null)
					{
						while (count <= lastRow) //7597
						{
							try
							{
								//aRecord = dataRow.Cells().Select(x => x.CastTo<string>()).ToArray();
								aRecord = dataRow.Cells().Select(x => x.GetString()).ToArray();
							}
							catch (Exception exx)
							{
								aRecord = new String[] { "error", "count = " + (count + 1).ToString() };
								string errMessage = " :  in row # " + (count + 1).ToString() + ":" + exx.Message;
								Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedMarker, errMessage));
							}
							dataRow = dataRow.RowBelow();
							count++;
							yield return aRecord;
						}
					}
				}
			}
			else
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileXlsxExpected, filePath));
				yield return null;
			}
		}



		#region IFileParser Members


		public void FinallyMethod()
		{
			throw new NotImplementedException();
		}

		#endregion
	}

}

