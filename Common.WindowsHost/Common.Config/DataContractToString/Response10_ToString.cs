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
	public static class Response10_ToString
	{

		public static string Response10ToString(this Response10 response)
		{
			string codeOperation = response.Operation;
			CodeOperationEnum codeOperationEnum = ConvertData.GetCodeOperationEnum(codeOperation);
			string ret = "";

			if (codeOperationEnum == CodeOperationEnum.CodeOperation10)
			{
				ret = Response_ToStringBase.ResponseToStringBase(response as Sip2BaseData, codeOperationEnum);
				return ret;
			}
			else
			{
				return "Code operation is expected " + (int)CodeOperationEnum.CodeOperation10;
			}
		}

		//public static string Response98ToString(this Response98 response)
		//{
		//	string codeOperation = response.Operation;
		//	//string codeOperation = ConvertData.GetCodeOperationString(CodeOperationEnum.CodeOperation98);
		//	string ret = codeOperation;
		//	// обойти словарь из конфига и все данные по правилам сконкотенировать
		//	UtilsSip2Protocol sip2Protocol = new UtilsSip2Protocol();
		//	string errorXDoc = "";
		//	try
		//	{
		//		XElement xElement = sip2Protocol.GetXDocumentOperation(codeOperation, out errorXDoc);
		//		Operation operation = new Operation(xElement);
		//		//Dictionary<string, Field> fieldDictionary = operation.fieldDictionary;
		//		Fields fields = operation.fields;
		//		//надо перебрать весь словарь  и по рефлексии найти проперти с таким же имени
		//		PropertyInfo[] props = response.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
		//		//этому проперти присвоить данные 
		//		//которые распарсить по правилам в объекте 

		//		//foreach (KeyValuePair<string, Field> keyValuePair in fieldDictionary)
		//		foreach (Field field in fields) // sip2Protocol
		//		{
		//			for (int i = 0; i < props.Length; i++) //responseData
		//			{
		//				if (props[i].Name.ToLower() == field.name.ToLower())
		//				{
		//					if (field.datacontract == "1")
		//					{
		//						//string parceData = sip2Protocol.GetValueFromStringByFormat(entity, keyValuePair.Value.xElementFormat);
		//						string newData = sip2Protocol.GetStringFromPropertyValueByFormat((string)props[i].GetValue(response, null), field.xElementFormat);
		//						ret = ret + newData;
		//					}
		//					else
		//					{
		//						string newData = sip2Protocol.GetStringFromPropertyValueByFormat(field.datadefault, field.xElementFormat);
		//						ret = ret + newData;
		//					}
		//				}
		//			}

		//		}

		//		ret = ret.TrimEnd("|".ToCharArray());
		//		return ret;
		//	}
		//	catch(Exception exp)
		//	{
		//		errorXDoc = errorXDoc + exp.Message;
		//		return errorXDoc;
		//	}
		//}
	}
}

	