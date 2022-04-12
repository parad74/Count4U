using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportIturPdaMISFileWriter : IExportIturStreamWriter
	{

		public void AddRow(StreamWriter sw, Itur itur, string separator = "|", bool iturNameOrERPIturCode = true, string param1 = "", bool invertLetter = true, bool rt2lf = true, int maxLen = 49)
		{
			//TODO
			//string iturName = param1 + itur.NumberPrefix + "-" + itur.NumberSufix;
			if (string.IsNullOrWhiteSpace(param1) == false) param1 = param1 + " "; 

			string iturName = param1 + itur.NumberSufix;

			if (iturNameOrERPIturCode == false)
			{
				iturName = param1 + itur.ERPIturCode;
			}
			else
			{
				if (string.IsNullOrWhiteSpace(itur.Name) == false)
				{
					iturName = CompileProductName(itur.Name, maxLen, invertLetter, rt2lf);
				}
			}
			string newRow = itur.IturCode + "|" + itur.ERPIturCode + "|" + iturName;
			sw.WriteLine(newRow);
	}

		public void AddRowSimple(StreamWriter sw, Itur itur, string separator = ",", bool iturNameOrERPIturCode = true, string param1 = "", bool invertLetter = true, bool rt2lf = true, int maxLen = 49)
		{
			//string iturName = param1 + itur.NumberPrefix + "-" + itur.NumberSufix;
			//string iturName = param1 + " " + itur.NumberSufix;
			//if (iturNameOrERPIturCode == false)
			//{
			//	iturName = param1 + " " + itur.ERPIturCode;
			//}
			//string newRow = itur.IturCode + "," + iturName;
			//sw.WriteLine(newRow);
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		
		}

		public void AddHeaderSum(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			
		}

		private static string CompileProductName(string name, int maxLen, bool invertLetter, bool rt2lf)
		{
			string productName = "";
			//================ ProductName собираем ============
			string nameInvertLetter = String.IsNullOrEmpty(name) ? " " : name.ReverseDosHebrew(invertLetter, false);		
			productName = nameInvertLetter;
			// хоть одна буква на иврите
			bool isHebrew = productName.HaveDosHebrewCharInWord();
			if (isHebrew == true)
			{
				productName = nameInvertLetter.ReverseDosHebrew(false, rt2lf);
			}

			string productNameRevers = productName;
			//string productNameRevers = String.IsNullOrEmpty(product.Name) ? " " : product.Name.ReverseDosHebrew(invertLetter, rt2lf);		 //(true, true);
			//productName = productNameRevers;
			if (productNameRevers.Length > maxLen && maxLen > 0)
			{
				if (isHebrew == true)
				{
					int start = productNameRevers.Length - maxLen;
					productName = productNameRevers.Substring(start, maxLen);
				}
				else
				{
					productName = productNameRevers.Substring(0, maxLen);
				}
			}
			return productName;
		}
	}
}
