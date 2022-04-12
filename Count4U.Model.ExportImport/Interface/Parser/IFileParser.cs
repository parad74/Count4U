using System;
using System.Collections.Generic;
using System.Text;
namespace Count4U.Model.Interface.Count4U
{
	public interface IFileParser
	{
		IEnumerable<string[]> GetRecords(string filePath, Encoding encoding, string[] separators,
			int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1);
		IEnumerable<String> GetRecords(string filePath, Encoding encoding,
			int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1);
		IEnumerable<object[]> GetRow(string filePath, Encoding encoding, string[] separators,
		int countExcludeFirstString, string param = "", int sheetNumberXlsx = 1);
		void FinallyMethod();
	}
}
