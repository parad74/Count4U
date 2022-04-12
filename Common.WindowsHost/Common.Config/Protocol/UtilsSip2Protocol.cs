using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IdSip2.Common.Protocol;
using IdSip2.DataContract;
using IdSip2.ServiceContract.Common;

namespace IdSip2.Common
{
	public class UtilsSip2Protocol
	{
		public UtilsSip2Protocol()
		{
		}

		public XElement GetXDocumentOperation(string codeOperation, out string errorXDoc)
		{
			 XDocument xdoc = UtilsSip2.XDocumentSip2Protocol();
			 errorXDoc = "";
			 if (xdoc == null)
			 {
				 errorXDoc = errorXDoc + " : ??" + UtilsSip2.Error;			//TODO 
				 return null;
			 }

			try
			{
				//var responseTestData = xdoc.Descendants("operation").Where(t => t.Attribute("code").Value == codeOperation).Descendants("response").FirstOrDefault().Value;
				var retXDocument = xdoc.Descendants("operation").Where(t => t.Attribute("code").Value == codeOperation);
				if (retXDocument == null)
				{
					errorXDoc = errorXDoc + " : " + " GetXDocumentOperation: xdoc.Descendants operation code == " + codeOperation + " =>  xdoc == null";
					return null;
				}
				return retXDocument.FirstOrDefault();
			}
			catch (Exception exp)
			{
				errorXDoc = errorXDoc +  exp.Message + "--" + exp.StackTrace;
				return null;
			}
		}

		//<format type="fixed" length="1" startindex="1"  required="0|1|2">
		//<format type="variablelength_startindex" startindex="9"  required="">
		// <format type="variablelength" start="AM" required=""> 
		//String -> ToRequestBase
		public string GetValue_FromString_ByFormat(string entity, XElement xElementFormat)
		{
			string retData = "";
			if (string.IsNullOrWhiteSpace(entity) == true) return retData;
			if (xElementFormat == null) return retData;

			string formatType = (string)xElementFormat.Attribute("type") ?? "";
			if (formatType == "fixed")
			{
				int length = (int?)xElementFormat.Attribute("length") ?? 0;
				int startindex = (int?)xElementFormat.Attribute("startindex") ?? 0;
				string required = (string)xElementFormat.Attribute("required") ?? "";
				if (length > 0 && startindex > 0)
				{
					if ((startindex + length) > entity.Length) length = entity.Length - startindex;
					startindex  = startindex - 1;
					retData = entity.Substring(startindex, length);
				}
			}
			else if (formatType == "variablelength")
			{
				string start = (string)xElementFormat.Attribute("start") ?? "";
				if (start.Length != 2) return retData;
				string required = (string)xElementFormat.Attribute("required") ?? "";
				int startindex = (int?)xElementFormat.Attribute("startindex") ?? 1;
				string[] in_data = entity.Split(new Char[] { '|' });
				foreach (var element in in_data)
				{
					//  Name
					string ind = "";
					if ((startindex - 1 + 2) < element.Length) {
						ind = element.Substring(startindex - 1, 2).ToUpper();
					}
					if (ind == start) retData = element.Substring(startindex - 1 + 2);
				}
			}
			else if (formatType == "variablelength_startindex")
			{
				int startindex = (int?)xElementFormat.Attribute("startindex") ?? 0;
				string required = (string)xElementFormat.Attribute("required") ?? "";
				int length = entity.Length - startindex;
				startindex = startindex - 1;
				retData = entity.Substring(startindex, length);
			}
			return retData;
		}

			//<format type="fixed" length="1" startindex="1"  required="0|1|2">
		//<format type="variablelength_startindex" startindex="9"  required="">
		// <format type="variablelength" start="AM" required=""> 
		//Response -> ToString
		public string GetString_FromPropertyValue_ByFormat(string propertyValue, XElement xElementFormat)
		{
			string retData = "";
			if (string.IsNullOrWhiteSpace(propertyValue) == true) propertyValue = "";

			if (xElementFormat == null) return retData;

			string formatType = (string)xElementFormat.Attribute("type") ?? "";
			if (formatType == "fixed")
			{
				int length = (int?)xElementFormat.Attribute("length") ?? 0;
				int startindex = (int?)xElementFormat.Attribute("startindex") ?? 0;
				string required = (string)xElementFormat.Attribute("required") ?? "";
				if (required == "YYYYMMDDZZZZHHMMSS") retData = ConvertData.GetDateString();
				else if (required == "DD.MM.YYYY") retData = ConvertData.GetDateShotString(propertyValue);
				
				else if (propertyValue.Length > length) retData = propertyValue.Substring(0, length);
				else if (propertyValue.Length < length)
				{
					if (required == "PadLeft0") retData = propertyValue.PadLeft(length, '0');
					else retData = propertyValue.PadRight(length, ' ');
				}
				else retData = propertyValue;
			}
			else if (formatType == "variablelength")
			{
				string required = (string)xElementFormat.Attribute("required") ?? "";
				string start = (string)xElementFormat.Attribute("start") ?? "";
				if (start.Length > 2) start = start.Substring(2); 
				retData = start + propertyValue + "|";
				if (required == "YYYYMMDDZZZZHHMMSS") retData = start + ConvertData.GetDateString() + "|";
				else if (required == "DD.MM.YYYY") retData = start + ConvertData.GetDateShotString(propertyValue) + "|";
				else if (required == "excludeIfEmpty")
				{
					if (string.IsNullOrWhiteSpace(propertyValue) == true)
					{
						retData = "";
					}
				}
			}
			else if (formatType == "variablelength_startindex")
			{
				int startindex = (int?)xElementFormat.Attribute("startindex") ?? 0;
				string required = (string)xElementFormat.Attribute("required") ?? "";
				retData = propertyValue;
			}

			return retData;
		}

	

		//private static string LeadingEmpty(string inString, int n, string code)
		//{
		//	int lenCode = inString.Length;

		//	if (lenCode > n) code = inString.Substring(0, n);

		//	if (lenCode < n)
		//	{
		//		int rest = n - lenCode;
		//		string temp = "";
		//		for (int i = 0; i < rest; i++)
		//		{
		//			temp = temp + " ";
		//		}
		//		code = temp + inString;
		//	}
		//	return code;
		//}

	}

	public static class UtilsSip2ProtocolOperation
	{
		public static Dictionary<string, string> FillResponseSampleData(Sip2BaseData response, out string error)
		{
			Dictionary<string, string> propertyDictionary = new Dictionary<string, string>();
			if (response == null)
			{
				error = "FillResponseSampleData: In parameter response == null";
				return propertyDictionary;
			}

			string codeOperation = response.Operation;
			// обойти словарь из конфига и все данные по правилам сконкотенировать
			UtilsSip2Protocol sip2Protocol = new UtilsSip2Protocol();
			try
			{
				XElement xElement = sip2Protocol.GetXDocumentOperation(codeOperation, out error);
				if (xElement == null)
				{
					error = error + "FillResponseSampleData: In parameter xElement == null (codeOperation == " + codeOperation  + ")";
				}
				Operation operation = new Operation(xElement);
				//Dictionary<string, Field> fieldDictionary = operation.fieldDictionary;
				Fields fields = operation.fields;
				//надо перебрать весь словарь  и по рефлексии найти проперти с таким же имени
				PropertyInfo[] props = response.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
				//этому проперти присвоить данные 
				//которые распарсить по правилам в объекте 

				//foreach (KeyValuePair<string, Field> keyValuePair in fieldDictionary)
				foreach (Field field in fields) // sip2Protocol
				{
					for (int i = 0; i < props.Length; i++) //responseData
					{
						if (props[i] == null) continue;
						if (field == null) continue;
						if (props[i].Name.ToLower() == field.name.ToLower())
						{
							if (field.datacontract == "1")
							{
								props[i].SetValue(response, field.datasample);
								propertyDictionary[props[i].Name.ToLower()] = field.datasample;
							}
						}
					}
				}
			}
			catch (Exception exp)
			{
				error = exp.Message + exp.StackTrace;
			}
			return propertyDictionary;
		}
	}

}
