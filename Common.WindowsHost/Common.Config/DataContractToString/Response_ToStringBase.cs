using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using IdSip2.Common;
using IdSip2.Common.Protocol;
using IdSip2.DataContract;
using IdSip2.ServiceContract.Common;

namespace IdSip2.ServiceContract
{
	public static class Response_ToStringBase
	{
		public static string ResponseToStringBase(this Sip2BaseData responseBase, CodeOperationEnum codeOperationEnum)
		{
			string codeOperation = ConvertData.GetCodeOperationString(codeOperationEnum);
			string ret = codeOperation;
			// обойти словарь из конфига и все данные по правилам сконкотенировать
			UtilsSip2Protocol sip2Protocol = new UtilsSip2Protocol();
			string errorXDoc = "";
			try
			{
				XElement xElement = sip2Protocol.GetXDocumentOperation(codeOperation, out errorXDoc);
				Operation operation = new Operation(xElement);
				//Dictionary<string, Field> fieldDictionary = operation.fieldDictionary;
				Fields fields = operation.fields;
				//надо перебрать весь словарь  и по рефлексии найти проперти с таким же имени
				PropertyInfo[] props = responseBase.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
				//этому проперти присвоить данные 
				//которые распарсить по правилам в объекте 

				//foreach (KeyValuePair<string, Field> keyValuePair in fieldDictionary)
				foreach (Field field in fields) // sip2Protocol
				{
					for (int i = 0; i < props.Length; i++) //responseData
					{
						if (props[i].Name.ToLower() == field.name.ToLower())
						{
							if (field.datacontract == "1")
							{
								//string parceData = sip2Protocol.GetValueFromStringByFormat(entity, keyValuePair.Value.xElementFormat);
								string newData = sip2Protocol.GetString_FromPropertyValue_ByFormat((string)props[i].GetValue(responseBase, null), field.xElementFormat);
								ret = ret + newData;
							}
							else
							{
								string newData = sip2Protocol.GetString_FromPropertyValue_ByFormat(field.datadefault, field.xElementFormat);
								ret = ret + newData;
							}
						}
					}

				}

				ConfigCommunication configCommunication = new ConfigCommunication();
				string subFolder = configCommunication.GetProtocolFilePath().Trim('\\');
			
				if (codeOperationEnum == CodeOperationEnum.CodeOperation64)
				{
					if (responseBase is Response64)
					{
						Response64 rep64 = responseBase as Response64;
						string summery = rep64._summary;
						string summeryLine = rep64._summaryLine;
						string tag = rep64._tag;
						//if Y on 3rd character on "summary" field on 63 -> send AU (RENEW). example: 142810  Y        
						if (summery.StartsWith("  Y") == true)			//3th
						{
							ret = ret.TrimEnd("|".ToCharArray()) + summeryLine;
						}
						//for 63 2nd Y -> need to include an AT field for each barcode that has the attribute "Total delay days"
						if (summery.StartsWith(" Y") == true)			//2th
						{
							ret = ret.TrimEnd("|".ToCharArray()) + tag + "|";
						}
						//if Y on 1st character on "summary" field on 63 ->
						//send AS for each available hold for that user. (AVAILABLE HOLDS LIST). example: 112847Y 
						else if (summery.StartsWith("Y") == true)	   //1th
						{
							ret = ret.TrimEnd("|".ToCharArray()) + summeryLine;
						}
						//	if Y on 6 character on "summary" field on 63 ->
						//	send CD for each unavailable hold for that user (UNAVILABLE HOLDS LIST). example: 112847     Y
						else if (summery.StartsWith("     Y") == true)
						{
							ret = ret.TrimEnd("|".ToCharArray()) + summeryLine;
						}
					}
				}
				if (subFolder != "3M")
				{
					ret = ret.TrimEnd("|".ToCharArray());
				}
			//	ret = ret.Replace("^", "|");
				return ret;
			}
			catch(Exception exp)
			{
				errorXDoc = errorXDoc + exp.Message;
				return errorXDoc;
			}
		}
	}
}
