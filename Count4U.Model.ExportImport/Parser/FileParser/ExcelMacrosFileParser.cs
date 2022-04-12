using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using System.Globalization;
using System.Runtime.InteropServices;


namespace Count4U.Model.Count4U
{
	public class ExcelMacrosFileParser : IFileParser
	{
		private Application _xlApp = null;
		private Workbook _xlWorkBook;
		private Workbooks tmpworkbooks;
		private List<Worksheet> _xlWorkSheets;
		private Range _headerRange;
		private object[,] headersLine;
		private CultureInfo oldci;
		private Worksheet tableWorksheet;
		private Sheets tmpSheets;

		private const int RowMaxRead = 10000;

		private readonly ILog _log;

		public ExcelMacrosFileParser(ILog log)
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
		/// <param name="excludeFirstString">Исключить countExcludeFirstString строк из списка</param>
		/// <returns></returns>
		public IEnumerable<String[]> GetRecords(string filePath, Encoding encoding,
			String[] separators, int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		{
			string extension = System.IO.Path.GetExtension(filePath);
			if (extension == ".xlsm")
			{
				int worksheetN = 1;
				if (sheetNumberXlsx > 1)
				{
					worksheetN = sheetNumberXlsx;
				}
				Workbook wb = OpenExcelApp(filePath); if (wb == null) yield return null;
				Worksheet ws = wb.Sheets.get_Item(worksheetN); if (ws == null) 
				{
					FinallyMethod();
					yield return null;
				}

				if (string.IsNullOrWhiteSpace(param) == false)
				{
					if (ws.Name.Trim().ToUpper() != param.Trim().ToUpper())
					{
						Log.Add(MessageTypeEnum.WarningParser,
							String.Format("In .xlsx file {0} the Expected Name of Sheet [ {1} ] is Not Equal to the Incoming Name of Sheet [ {2} ] ",
							 filePath, param, ws.Name));
					}
				}

				Range excelRange = ws.UsedRange;
				int maxColNum;
				int lastRow;

				try
				{
					maxColNum = excelRange.SpecialCells(XlCellType.xlCellTypeLastCell).Column;
					lastRow = excelRange.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
				}
				catch
				{
					maxColNum = excelRange.Columns.Count;
					lastRow = excelRange.Rows.Count;
				}
				if (lastRow < countExcludeFirstString) 	
				{
					FinallyMethod();
					yield return null;
				}

				int count = countExcludeFirstString;
			
				while (count <= lastRow)			 // по строкам
				{
					List<string> records = new List<string>();
					Range range = ws.Range[(object)ws.Cells[count, 1], (object)ws.Cells[count, maxColNum]];
					System.Array values = (System.Array)range.Cells.Value2;
					foreach (object val in values)		   //по колонкам
					{
						if (val != null)
						{
							records.Add(val.ToString());
						}
						else
						{
							records.Add("");
						}
					}
					count++;
					String[] aRecord = records.ToArray();
					yield return aRecord;

				}
				FinallyMethod();


				//string[] strArray = ConvertToStringArray(myvalues);

				//Range RealExcelRangeLoc = ws.Range[(object)ws.Cells[i, 1], (object)ws.Cells[i, maxColNum]];
				//object[,] valarr = null;
				//try
				//{
				//	var valarrCheck = RealExcelRangeLoc.Value[XlRangeValueDataType.xlRangeValueDefault];
				//	if (valarrCheck is object[,] || valarrCheck == null)
				//		valarr = (object[,])RealExcelRangeLoc.Value[XlRangeValueDataType.xlRangeValueDefault];
				//}
				//catch
				//{
				//	valarr = loadCellByCell(i, maxColNum, ws);
				//}


				//Excel.Sheets sheets = m_Excel.Worksheets;
				//Excel.Worksheet worksheet = (Excel.Worksheet)sheets.get_Item(1);
				//System.Array myvalues;
				//Excel.Range range = worksheet.get_Range("A1", "E1".ToString());
				//myvalues = (System.Array)range.Cells.Value;
 
				//Excel.Range range = worksheet.get_Range("A" + i.ToString(), "J" + i.ToString());
				//System.Array myvalues = (System.Array)range.Cells.Value;
				//string[] strArray = ConvertToStringArray(myvalues);

				//Range myRange = (Range) sheets.Cells[30,10];
				//currentRate = (float)Convert.ToDouble(myRange.Cells.Value2.ToString());
				//if (values.GetValue(i, j) == null)
				//theArray[position++] = "";
				// else
				//{
				//	theArray[position++] = (string)values.GetValue(i, j).ToString();
				//}
			}
			else
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileXlsxExpected, filePath));
				yield return null;
			}

		}



		public IEnumerable<object[]> GetRow(string filePath, Encoding encoding,
			String[] separators, int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		{
			  	string extension = System.IO.Path.GetExtension(filePath);
			if (extension == ".xlsm")
			{
				int worksheetN = 1;
				if (sheetNumberXlsx > 1)
				{
					worksheetN = sheetNumberXlsx;
				}
				Workbook wb = OpenExcelApp(filePath); if (wb == null) yield return null;
				Worksheet ws = wb.Sheets.get_Item(worksheetN); if (ws == null) 
				{
					FinallyMethod();
					yield return null;
				}

				if (string.IsNullOrWhiteSpace(param) == false)
				{
					if (ws.Name.Trim().ToUpper() != param.Trim().ToUpper())
					{
						Log.Add(MessageTypeEnum.WarningParser,
							String.Format("In .xlsx file {0} the Expected Name of Sheet [ {1} ] is Not Equal to the Incoming Name of Sheet [ {2} ] ",
							 filePath, param, ws.Name));
					}
				}

				Range excelRange = ws.UsedRange;
				int maxColNum;
				int lastRow;

				try
				{
					maxColNum = excelRange.SpecialCells(XlCellType.xlCellTypeLastCell).Column;
					lastRow = excelRange.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
				}
				catch
				{
					maxColNum = excelRange.Columns.Count;
					lastRow = excelRange.Rows.Count;
				}
				if (lastRow < countExcludeFirstString) 	
				{
					FinallyMethod();
					yield return null;
				}

				int count = countExcludeFirstString;
			
				while (count <= lastRow)			 // по строкам
				{
					List<object> records = new List<object>();
					Range range = ws.Range[(object)ws.Cells[count, 1], (object)ws.Cells[count, maxColNum]];
					System.Array values = (System.Array)range.Cells.Value2;
					foreach (object val in values)		   //по колонкам
					{
						if (val != null)
						{
							records.Add(val);
						}
						else
						{
							records.Add(null);
						}
					}
					count++;
					object[] aRecord = records.ToArray();
					yield return aRecord;

				}
				FinallyMethod();
			}
			else
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileXlsxExpected, filePath));
				yield return null;
			}
		}


		private static object[,] loadCellByCell(int row, int maxColNum, _Worksheet osheet)
		{
			var list = new object[2, maxColNum + 1];
			for (int i = 1; i <= maxColNum; i++)
			{
				var RealExcelRangeLoc = osheet.Range[(object)osheet.Cells[row, i], (object)osheet.Cells[row, i]];
				object valarrCheck;
				try
				{
					valarrCheck = RealExcelRangeLoc.Value[XlRangeValueDataType.xlRangeValueDefault];
				}
				catch
				{
					valarrCheck = (object)RealExcelRangeLoc.Value2;
				}
				list[1, i] = valarrCheck;
			}
			return list;
		}

		private Workbook OpenExcelApp(string fileName)
		{
			
			// Opening Exel Application
			_xlApp = new Application { Visible = false };

			// Setting en-us culture info for applying the excel
			oldci = System.Threading.Thread.CurrentThread.CurrentCulture;
			System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");

			// Getting the workbook and initilaze sheets
			tmpworkbooks = _xlApp.Workbooks;
			_xlWorkBook = tmpworkbooks.Open(fileName, 0, true, 5, "", "", true,
											Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
											Missing.Value, Missing.Value, Missing.Value);
			return _xlWorkBook;
		}

		public void FinallyMethod()
		{
			try
			{
				// Closing workbook and Exel App
				if (_xlWorkBook != null)
				{
					_xlWorkBook.Close(false, null, null);
					Marshal.ReleaseComObject(_xlWorkBook);
				}
				if (tmpworkbooks != null)
				{
					tmpworkbooks.Close();
					Marshal.ReleaseComObject(tmpworkbooks);
				}
				if (_xlApp != null)
				{
					_xlApp.Quit();
					Marshal.ReleaseComObject(_xlApp);
				}

			}
			catch (Exception e)
			{
				Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileXlsxExpected, "Faild to finaly release the excel file"));
	
			}
			finally
			{
				// setting the previous CultureInfo to program
				if (oldci != null) System.Threading.Thread.CurrentThread.CurrentCulture = oldci;
			}
		}

		//IEnumerable<object[]> IFileParser.GetRow(string filePath, Encoding encoding, string[] separators, int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		//{
		//	int worksheetN = 1;
		//	if (sheetNumberXlsx > 1)
		//	{
		//		worksheetN = sheetNumberXlsx;
		//	}

		//	if (System.IO.Path.GetExtension(filePath) == ".xlsx")
		//	{
		//		var wb = new XLWorkbook(filePath); if (wb == null) yield return null;
		//		var ws = wb.Worksheet(worksheetN); if (ws == null) yield return null;

		//		if (string.IsNullOrWhiteSpace(param) == false)
		//		{
		//			if (ws.Name.Trim().ToUpper() != param.Trim().ToUpper())
		//			{
		//				Log.Add(MessageTypeEnum.WarningParser,
		//					String.Format("In .xlsx file {0} the Expected Name of Sheet [ {1} ] is Not Equal to the Incoming Name of Sheet [ {2} ] ",
		//					 filePath, param, ws.Name));
		//			}
		//		}
		//		//ws.Cell(1, 1);

		//		int count = 1;
		//		IXLRow firstRow;
		//		IXLRangeRow dataRow;
		//		int lastRow = 0;
		//		try
		//		{
		//			firstRow = ws.FirstRow();
		//			dataRow = firstRow.RowUsed();
		//			lastRow = ws.LastCellUsed().Address.RowNumber;
		//		}
		//		catch
		//		{
		//			Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsEmpty, filePath));
		//			dataRow = null;
		//		}

		//		if (dataRow == null) yield return null;

		//		//if (countExcludeFirstString > 0)
		//		//{
		//		//	if (dataRow != null)
		//		//	{
		//		//		if (count <= countExcludeFirstString)
		//		//		{

		//		//			dataRow = dataRow.RowBelow();
		//		//			count++;
		//		//		}
		//		//	}
		//		//}

		//		if (countExcludeFirstString > 0)
		//		{
		//			while (count <= countExcludeFirstString)
		//			{
		//				if (dataRow != null)
		//				{
		//					dataRow = dataRow.RowBelow();
		//					count++;
		//				}
		//			}
		//		}

		//		object[] aRecord;
		//		if (dataRow != null)
		//		{
		//			while (count <= lastRow) //7597
		//			{
		//				aRecord = dataRow.Cells().Select(x => x.CastTo<string>()).ToArray();
		//				dataRow = dataRow.RowBelow();
		//				count++;
		//				yield return aRecord;
		//			}
		//		}
		//	}
		//	else
		//	{
		//		Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileXlsxExpected, filePath));
		//		yield return null;
		//	}
		//}



		#region IFileParser Members

		//public IEnumerable<string[]> GetRecords(string filePath, Encoding encoding, string[] separators, int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		//{
		//	throw new NotImplementedException();
		//}

		public IEnumerable<String> GetRecords(string filePath, Encoding encoding,
				int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1)
		{
			yield return "";


		}

		#endregion
	}

}

