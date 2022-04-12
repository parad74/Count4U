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
	public static class String_ToRequest35
	{
		// пришла строка с liber8 - надо распарсить  в объект (dataContract) и передать дальше на Client_Host
		public static Request35 StringToRequest35(this string entity)
		{
			Request35 request = new Request35();
			request.Request = entity;

			CodeOperationEnum codeOperationEnum = ConvertData.GetCodeOperationEnum(entity);
			if (codeOperationEnum == CodeOperationEnum.CodeOperation35)
			{
				Sip2BaseData response = String_ToRequestBase.StringToRequestBase(entity, request, codeOperationEnum);
				return response as Request35;
			}
			else
			{
				request.Error = "Code operation is expected " + (int)CodeOperationEnum.CodeOperation35;
				return request;
			}
		}
	}
}
