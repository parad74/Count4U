using System;
using System.ServiceModel;
using System.Collections.Generic;
//using NLog;
using IdSip2.ServiceContract;
using IdSip2.DataContract;
using IdSip2.ServiceContract.Common;
using IdSip2.Common;
using System.Xml.Linq;
using IdSip2.Common.Protocol;
using System.Reflection;
using IdSip2.ServiceClient;



// СЕРВИС.

namespace IdSip2.Service
{

	public class Request2ObjectService : IRequest2Object
    {
		//private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
	
		public Sip2BaseData RequestSip2(string _requestString)
		{
			//UiConnection.ClearOutputText();
			Sip2BaseData _request = new Sip2BaseData();
			_request.Request = _requestString;
			CodeOperationEnum codeOperationEnum = ConvertData.GetCodeOperationEnum(_requestString);
			

			if (codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
			{
				//UiConnection.UpdateOutputText("Not expected operation code : " + _request.Request);
				_request.Error = _request.Error + " Not expected operation code ";
				return _request;

			}

			string codeOperation = ConvertData.GetCodeOperationString(codeOperationEnum);
			string codeBackOperation = ConvertData.GetCodeBackOperationString(codeOperationEnum);

			//нужен клиент еще один
			//Request2ResponseClient dataOnlineClient = new Request2ResponseClient();
			IRequest2Response dataOnline = new Request2ResponsePresenterServise();

			switch (codeOperationEnum)
			{
				//Request99 - Response98
				case CodeOperationEnum.CodeOperation99:
					{
						Request99 request = _request.Request.StringToRequest99();
						//UiConnection.UpdateOutputText("Request" + codeOperation + ">> [" + _request.Request + "]" + Environment.NewLine);

						Response98 response = dataOnline.Request99(request);
						//Response98 response = dataOnlineClient.Request99(request); //здесь мы заполняем на хосте поля из request99 в response98
	
						string responseString = response.Response98ToString();
						response.Response = responseString + Environment.NewLine;

						//UiConnection.UpdateOutputText("Response" + codeBackOperation + " << [" + response.Response + "]" + Environment.NewLine);
						Sip2BaseData retData = new Sip2BaseData(response);
						return retData;
					}
				//Request63 - Response64
				case CodeOperationEnum.CodeOperation63:
					{
						Request63 request = _request.Request.StringToRequest63();

						//UiConnection.UpdateOutputText("Request" + codeOperation + " >> [" + _request.Request + "]" + Environment.NewLine);

						Response64 response = dataOnline.Request63(request); 
						//Response64 response = dataOnlineClient.Request63(request); //здесь мы заполняем на хосте поля из request99 в response98
				
						string responseString = response.Response64ToString();
						response.Response = responseString + Environment.NewLine;

						//UiConnection.UpdateOutputText("Response" + codeBackOperation + " << [" + response.Response + "]" + Environment.NewLine);
						Sip2BaseData retData = new Sip2BaseData(response);
						return retData;
					}
				//Request11 - Response12
				case CodeOperationEnum.CodeOperation11:
					{
						//UiConnection.UpdateOutputText("Request" + codeOperation + " >> [" + _request.Request + "]" + Environment.NewLine);

						Request11 request = _request.Request.StringToRequest11();

						Response12 response = dataOnline.Request11(request); 
						//Response12 response = dataOnlineClient.Request11(request); //здесь мы заполняем на хосте поля из Request11 в Response12

						string responseString = response.Response12ToString();
						response.Response = responseString + Environment.NewLine;

						//UiConnection.UpdateOutputText("Response" + codeBackOperation + " << [" + response.Response + "]" + Environment.NewLine);
						Sip2BaseData retData = new Sip2BaseData(response);
						return retData;
					}
				default:
					return _request;
			}
		}

	}
}

