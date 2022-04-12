using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportIturFileWriter : IExportIturStreamWriter
	{

		#region IExportIturStreamWriter Members


		public void AddRow(StreamWriter sw, Itur itur, string separator = ",", bool iturNameOrERPIturCode = true, string param1 = "", bool invertLetter = true, bool rt2lf = true, int maxLen = 49)
		{
			string iturName = param1 + itur.NumberPrefix + "-" + itur.NumberSufix;
			string newRow = itur.IturCode + "," + iturName;
			sw.WriteLine(newRow);
		}

		public void AddRowSimple(StreamWriter sw, Itur itur, string separator = ",", bool iturNameOrERPIturCode = true, string param1 = "", bool invertLetter = true, bool rt2lf = true, int maxLen = 49)
		{
			string iturName = param1 + itur.NumberPrefix + "-" + itur.NumberSufix;
			string newRow = itur.IturCode + "," + iturName;
			sw.WriteLine(newRow);
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{

		}

		public void AddHeaderSum(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{

		}

		#endregion
	}
}
