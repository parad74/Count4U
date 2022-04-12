namespace IdSip2.Common
{
	public class Object_ToDataBase
	{
		//public static Sip2BaseData ObjectToRequestBase(object entity, Sip2BaseData requestBase, CodeOperationEnum codeOperationEnum)
		//{
		//	UtilsSip2Protocol sip2Protocol = new UtilsSip2Protocol();
		//	string codeOperation = ConvertData.GetCodeOperationString(codeOperationEnum);

		//	string errorXDoc = "";
		//	XElement xElement = sip2Protocol.GetXDocumentOperation(codeOperation, out errorXDoc);
		//	if (xElement == null)
		//	{
		//		requestBase.Error = "Object_ToDataBase for CodeOperation " + codeOperation + " not find XElement on XmlProtocolFile";
		//		return requestBase;
		//	}
		//	Operation operation = new Operation(xElement);
		//	Dictionary<string, Field> fieldDictionary = operation.fieldDictionary;
		//	//надо перебрать весь словарь  и по рефлексии найти проперти с таким же имени
		//	PropertyInfo[] objectProps = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
		//	PropertyInfo[] props = requestBase.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

		//	//этому проперти присвоить данные 
		//	//которые распарсить по правилам в объекте 
		//	foreach (KeyValuePair<string, Field> keyValuePair in fieldDictionary)
		//	{
		//		for (int i = 0; i < props.Length; i++)
		//		{
		//			if (props[i].Name.ToLower() == keyValuePair.Key.ToLower())
		//			{
		//				if (keyValuePair.Value.datacontract == "1")
		//				{
		//					string parceData = "";
		//					for (int k = 0; k < objectProps.Length; k++)
		//					{
		//						if (objectProps[k].Name.ToLower() == keyValuePair.Key.ToLower())
		//						{
		//							 parceData = (string)objectProps[k].GetValue(entity, null);
		//							 break;
		//						}
		//					}
		//					props[i].SetValue(requestBase, parceData);
		//					break;
		//				}
		//			}
		//		}
		//	}
		//	return requestBase;
		//}
	}
}
