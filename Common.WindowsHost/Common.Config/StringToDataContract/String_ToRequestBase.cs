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
	public static class String_ToRequestBase
	{
		public static Sip2BaseData StringToRequestBase(string entity, Sip2BaseData requestBase, CodeOperationEnum codeOperationEnum)
		{
			entity = entity.Substring(2);
			UtilsSip2Protocol sip2Protocol = new UtilsSip2Protocol();
			string codeOperation = ConvertData.GetCodeOperationString(codeOperationEnum);

			string errorXDoc = "";
			XElement xElement = sip2Protocol.GetXDocumentOperation(codeOperation, out errorXDoc);
			if (xElement == null) 
			{
				requestBase.Error = "StringToRequestBase for CodeOperation " + codeOperation + " not find XElement on XmlProtocolFile";
				return requestBase;
			}
			Operation operation = new Operation(xElement);
			Dictionary<string, Field> fieldDictionary = operation.fieldDictionary;
			//надо перебрать весь словарь  и по рефлексии найти проперти с таким же имени
			PropertyInfo[] props = requestBase.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			//этому проперти присвоить данные 
			//которые распарсить по правилам в объекте 
			foreach (KeyValuePair<string, Field> keyValuePair in fieldDictionary)
			{
				for (int i = 0; i < props.Length; i++)
				{
					if (props[i].Name.ToLower() == keyValuePair.Key.ToLower())
					{
						if (keyValuePair.Value.datacontract == "1")
						{
							string parceData = sip2Protocol.GetValue_FromString_ByFormat(entity, keyValuePair.Value.xElementFormat);
							//props[i].SetValue(request99, keyValuePair.Value.datasample);
							props[i].SetValue(requestBase, parceData);
						}
					}
				}
			}
			return requestBase;
		}
	}
}
