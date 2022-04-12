using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using System.IO;

namespace Count4U.Model.Interface
{
	public interface IExportIturStreamWriter
	{
		void AddRow(StreamWriter sw, Itur itur, string separator = ",", bool iturNameOrERPIturCode = true, string Param1 = "", bool invertLetter = true, bool rt2lf = true, int maxLen = 49);
		void AddRowSimple(StreamWriter sw, Itur itur, string separator = ",", bool iturNameOrERPIturCode = true, string Param1 = "", bool invertLetter = true, bool rt2lf = true, int maxLen = 49);
	
		void AddHeader(StreamWriter sw,
			string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null);
		void AddHeaderSum(StreamWriter sw, 
			string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null);

	}
}
