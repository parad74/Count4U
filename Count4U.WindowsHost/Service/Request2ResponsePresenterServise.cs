using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Common.WCF;
using Idsip2.ServiceClient;
using Idsip2.ServiceClient.Common;
using IdSip2.Common;
using IdSip2.Common.Protocol;
using IdSip2.DataContract;
using IdSip2.ProxyClient;
using IdSip2.ServiceClient;
using IdSip2.ServiceContract;
using IdSip2.ServiceContract.Common;
using Main;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json.Linq;

namespace IdSip2.Service
{
	// Server Request2Response (Host на WinForm) (аналог должен? хостится на WindowsServise) или можно один и тот же захостить?
	// IV_Request2ResponseServer (сервер) - 
	//где заполняются данные - вызовом прокси-клиента(bibliothecaProxyClient V_ClientProxy) 
	// внешнего сервера (конечной точки пользователя) которые могут переключаться
	// вызывается из II_Request2ResponseClient
	// ( II_Server Sip2Socket  вызывает -> II_Request2ResponseClient -> IV_Request2ResponseServer -> V_ClientProxy)
	public class Request2ResponsePresenterService : IRequest2Response
	{
		private string _error;
		private CodeOperationEnum _codeOperationEnum;
		private string _codeBackOperation;
		private string _codeOperation;
		//private bool _showRequestMessage;
		//IRequest2ResponseClientPresenter _bibliothecaProxyClient;

			//читаем данные из файла
		public Request2ResponsePresenterService()
			: base()
		{
			//this._serviceLocator = serviceLocator;
			this._error = "";
			this._codeOperationEnum = CodeOperationEnum.CodeOperationUnknown;
			this._codeBackOperation = "";
			this._codeOperation = "";
		//	this._showRequestMessage = false;


			//var containerClientPresenters = this._serviceLocator.GetAllInstances<IClientPresenterInfo>().ToList();
			//IClientPresenterInfo info = containerClientPresenters.FirstOrDefault(r => r.Title == "Ale Kotarim");
			//this._bibliothecaProxyClient = _serviceLocator.GetInstance<IRequest2ResponseClientPresenter>(info.Name);
			
		}

	
		public Response64 Request63(Request63 request)
		{
			//IRequest2ResponseProxy bibliothecaProxyClient = UiConnection.GetSelectedClientProxy(); //V_ClientProxy
			//UiConnection.ClearOutputText();
			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy
			if (bibliothecaProxyClient == null) return new Response64();
			try
			{
				if (request == null) return new Response64();
				//UiConnection.UpdateOutputText("Request" + request.Operation + " >> [" + request.Request + "]" + Environment.NewLine);
				//UiConnection.UpdateLogRequestMessageText("63 Request : " + Environment.NewLine + OperationContext.Current.RequestContext.RequestMessage.ToString() + Environment.NewLine);

				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					Response64 response64 = new Response64(request as Sip2BaseData);
					//UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
					//response64.Error = response64.Error + this._error;
					return response64;
				}

				//REQUEST Fill Liber8 RequestData
				//Dictionary<string, string> propertyDictionary = this.FillFromRequestData(request, out this._error);

				//UiConnection.UpdateListViewRequest(this._codeOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); request.Error = request.Error + this._error; }

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				//AleKotarimBibliothecaProxyClient bibliothecaProxyClient = new AleKotarimBibliothecaProxyClient();
				Response64 response = bibliothecaProxyClient.Request63Proxy(request);
				//Dictionary<string, string> propertyDictionary1 = this.FillFromResponseData(response, out this._error);

				//UiConnection.UpdateListViewResponse(this._codeBackOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary1);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); response.Error = response.Error + this._error; }

				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponsePresenterService :: Request63 " + exp.Message);
			}
		}

		//public Response64 Request63(Request63 request)
		//{
		//	UiConnection.ClearOutputText();
		//	try
		//	{
		//		if (request == null) return new Response64();
		//		UiConnection.UpdateOutputText("Request" + request.Operation + " >> [" + request.Request + "]" + Environment.NewLine);
		//		UiConnection.UpdateLogText("63 Request : " + Environment.NewLine + OperationContext.Current.RequestContext.RequestMessage.ToString() + Environment.NewLine);
		//		this.InitCode(request as Sip2BaseData);
		//		if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
		//		{
		//			Response64 response64 = new Response64(request as Sip2BaseData);
		//			UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
		//			response64.Error = response64.Error + this._error;
		//			return response64;
		//		}

		//		//REQUEST
		//		Dictionary<string, string> propertyDictionary = this.FillFromRequestData(request, out this._error);
		//		UiConnection.UpdateListViewRequest(this._codeOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary);
		//		if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); request.Error = request.Error + this._error; }

		//		//RESPONSE- Fill data
		//		Response64 response = new Response64(request as Sip2BaseData);
		//		Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out this._error);
		//		UiConnection.UpdateListViewResponse(this._codeBackOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary1);
		//		if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); response.Error = response.Error + this._error; }

		//		return response;
		//	}
		//	catch (Exception exp)
		//	{
		//		throw new FaultException<Exception>(exp, "Request2ResponsePresenterService :: Request63 " + exp.Message);
		//	}
		//}

		public Response98 Request99(Request99 request)
		{
			//IRequest2ResponseProxy bibliothecaProxyClient = UiConnection.GetSelectedClientProxy();  // V_ClientProxy
		//	UiConnection.ClearOutputText();
			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy
			if (bibliothecaProxyClient == null) return new Response98();
			try
			{
				if (request == null) return new Response98();
				//UiConnection.UpdateOutputText("Request" + request.Operation + " >> [" + request.Request + "]" + Environment.NewLine);
				//UiConnection.UpdateLogRequestMessageText("99 Request : " + Environment.NewLine + OperationContext.Current.RequestContext.RequestMessage.ToString() + Environment.NewLine);

				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					Response98 response98 = new Response98(request as Sip2BaseData);
					//UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
					response98.Error = response98.Error + this._error;
					return response98;
				}

				//REQUEST Fill Liber8 RequestData
				//Dictionary<string, string> propertyDictionary = this.FillFromRequestData(request, out this._error);
				//UiConnection.UpdateListViewRequest(this._codeOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); 
				//	request.Error = request.Error + this._error; }

				
				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				//AleKotarimBibliothecaProxyClient bibliothecaProxyClient = new AleKotarimBibliothecaProxyClient();
				Response98 response = bibliothecaProxyClient.Request99Proxy(request);
				//Dictionary<string, string> propertyDictionary1 = this.FillFromResponseData(response, out this._error);
				
				//UiConnection.UpdateListViewResponse(this._codeBackOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary1);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); response.Error = response.Error + this._error; }

				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponsePresenterService :: Request99 " + exp.Message);
			}
		}


		public Response12 Request11(Request11 request)
		{
			//test FaultException
			//int divisor = 0;
			//if (divisor == 0)
			//{
			//	DivideByZeroException exception = new DivideByZeroException();
			//	throw new FaultException<DivideByZeroException>(exception, "Попытка деления на ноль!");
			//}


			//IRequest2ResponseProxy bibliothecaProxyClient = UiConnection.GetSelectedClientProxy();  // V_ClientProxy
			//UiConnection.ClearOutputText();

			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy
			if (bibliothecaProxyClient == null) return new Response12();
			try
			{
				//int ret = 10 / divisor;
				if (request == null) return new Response12();
				//UiConnection.UpdateOutputText("Request" + request.Operation + " >> [" + request.Request + "]" + Environment.NewLine);
				//UiConnection.UpdateLogRequestMessageText("11 Request : " + Environment.NewLine + OperationContext.Current.RequestContext.RequestMessage.ToString() + Environment.NewLine);

				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					Response12 response12 = new Response12(request as Sip2BaseData);
					//UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
					response12.Error = response12.Error + this._error;
					return response12;
				}

				//REQUEST 
				//Dictionary<string, string> propertyDictionary = this.FillFromRequestData(request, out this._error);
				//UiConnection.UpdateListViewRequest(this._codeOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); request.Error = request.Error + this._error; }

					
				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response12 response = bibliothecaProxyClient.Request11Proxy(request);
				//Dictionary<string, string> propertyDictionary1 = this.FillFromResponseData(response, out this._error);
				
				//UiConnection.UpdateListViewResponse(this._codeBackOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary1);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); response.Error = response.Error + this._error; }

				return response;
			}
			catch(Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponsePresenterService :: Request11 " + exp.Message);
			}

		}


		public Response94 Request93(Request93 request)
		{
			//IRequest2ResponseProxy bibliothecaProxyClient = UiConnection.GetSelectedClientProxy();  // V_ClientProxy
			//UiConnection.ClearOutputText();

			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy
			if (bibliothecaProxyClient == null) return new Response94();
			try
			{
				if (request == null) return new Response94();
				//UiConnection.UpdateOutputText("Request" + request.Operation + " >> [" + request.Request + "]" + Environment.NewLine);
				//UiConnection.UpdateLogRequestMessageText("93 Request : " + Environment.NewLine + OperationContext.Current.RequestContext.RequestMessage.ToString() + Environment.NewLine);

				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					Response94 response94 = new Response94(request as Sip2BaseData);
					//UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
					response94.Error = response94.Error + this._error;
					return response94;
				}

				//REQUEST 
				//Dictionary<string, string> propertyDictionary = this.FillFromRequestData(request, out this._error);
				//UiConnection.UpdateListViewRequest(this._codeOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); request.Error = request.Error + this._error; }

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response94 response = bibliothecaProxyClient.Request93Proxy(request);
				//Dictionary<string, string> propertyDictionary1 = this.FillFromResponseData(response, out this._error);

				//UiConnection.UpdateListViewResponse(this._codeBackOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary1);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); response.Error = response.Error + this._error; }

				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponsePresenterService :: Request93 " + exp.Message);
			}
		}

		public Response18 Request17(Request17 request)
		{
			//IRequest2ResponseProxy bibliothecaProxyClient = UiConnection.GetSelectedClientProxy();  // V_ClientProxy
			//UiConnection.ClearOutputText();

			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy
			if (bibliothecaProxyClient == null) return new Response18();

			try
			{
				if (request == null) return new Response18();
				//UiConnection.UpdateOutputText("Request" + request.Operation + " >> [" + request.Request + "]" + Environment.NewLine);
				//UiConnection.UpdateLogRequestMessageText("17 Request : " + Environment.NewLine + OperationContext.Current.RequestContext.RequestMessage.ToString() + Environment.NewLine);

				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					Response18 response18 = new Response18(request as Sip2BaseData);
					//UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
					response18.Error = response18.Error + this._error;
					return response18;
				}

				//REQUEST 
				//Dictionary<string, string> propertyDictionary = this.FillFromRequestData(request, out this._error);
				//UiConnection.UpdateListViewRequest(this._codeOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); request.Error = request.Error + this._error; }

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response18 response = bibliothecaProxyClient.Request17Proxy(request);
				//Dictionary<string, string> propertyDictionary1 = this.FillFromResponseData(response, out this._error);

				//UiConnection.UpdateListViewResponse(this._codeBackOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary1);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); response.Error = response.Error + this._error; }
				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponsePresenterService :: Request17 " + exp.Message);
			}
		}

		public Response10 Request09(Request09 request)
		{
			//IRequest2ResponseProxy bibliothecaProxyClient = UiConnection.GetSelectedClientProxy();  // V_ClientProxy
		//	UiConnection.ClearOutputText();

			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy
			if (bibliothecaProxyClient == null) return new Response10();

			try
			{
				if (request == null) return new Response10();
				//UiConnection.UpdateOutputText("Request" + request.Operation + " >> [" + request.Request + "]" + Environment.NewLine);
				//UiConnection.UpdateLogRequestMessageText("09 Request : " + Environment.NewLine + OperationContext.Current.RequestContext.RequestMessage.ToString() + Environment.NewLine);

				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					Response10 response10 = new Response10(request as Sip2BaseData);
					//UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
					response10.Error = response10.Error + this._error;
					return response10;
				}

				//REQUEST 
				//Dictionary<string, string> propertyDictionary = this.FillFromRequestData(request, out this._error);
				//UiConnection.UpdateListViewRequest(this._codeOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); request.Error = request.Error + this._error; }

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response10 response = bibliothecaProxyClient.Request09Proxy(request);
				//Dictionary<string, string> propertyDictionary1 = this.FillFromResponseData(response, out this._error);

				//UiConnection.UpdateListViewResponse(this._codeBackOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary1);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); response.Error = response.Error + this._error; }
				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponsePresenterService :: Request09 " + exp.Message);
			}
		}

		public Response30 Request29(Request29 request)
		{
			//IRequest2ResponseProxy bibliothecaProxyClient = UiConnection.GetSelectedClientProxy();  // V_ClientProxy
			//UiConnection.ClearOutputText();

			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy
			if (bibliothecaProxyClient == null) return new Response30();
			try
			{
				if (request == null) return new Response30();
				//UiConnection.UpdateOutputText("Request" + request.Operation + " >> [" + request.Request + "]" + Environment.NewLine);
				//UiConnection.UpdateLogRequestMessageText("29 Request : " + Environment.NewLine + OperationContext.Current.RequestContext.RequestMessage.ToString() + Environment.NewLine);

				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					Response30 response30 = new Response30(request as Sip2BaseData);
					//UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
					response30.Error = response30.Error + this._error;
					return response30;
				}

				//REQUEST 
				//Dictionary<string, string> propertyDictionary = this.FillFromRequestData(request, out this._error);
				//UiConnection.UpdateListViewRequest(this._codeOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); request.Error = request.Error + this._error; }

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response30 response = bibliothecaProxyClient.Request29Proxy(request);
				//Dictionary<string, string> propertyDictionary1 = this.FillFromResponseData(response, out this._error);

				//UiConnection.UpdateListViewResponse(this._codeBackOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary1);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); response.Error = response.Error + this._error; }

				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponsePresenterService :: Request29 " + exp.Message);
			}
		}

		public Response38 Request37(Request37 request)
		{
			//IRequest2ResponseProxy bibliothecaProxyClient = UiConnection.GetSelectedClientProxy();  // V_ClientProxy
			//UiConnection.ClearOutputText();

			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy
			if (bibliothecaProxyClient == null) return new Response38();
			try
			{
				if (request == null) return new Response38();
				//UiConnection.UpdateOutputText("Request" + request.Operation + " >> [" + request.Request + "]" + Environment.NewLine);
				//UiConnection.UpdateLogRequestMessageText("37 Request : " + Environment.NewLine + OperationContext.Current.RequestContext.RequestMessage.ToString() + Environment.NewLine);

				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					Response38 response38 = new Response38(request as Sip2BaseData);
					//UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
					response38.Error = response38.Error + this._error;
					return response38;
				}

				//REQUEST 
				//Dictionary<string, string> propertyDictionary = this.FillFromRequestData(request, out this._error);
				//UiConnection.UpdateListViewRequest(this._codeOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); request.Error = request.Error + this._error; }

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response38 response = bibliothecaProxyClient.Request37Proxy(request);
				//Dictionary<string, string> propertyDictionary1 = this.FillFromResponseData(response, out this._error);

				//UiConnection.UpdateListViewResponse(this._codeBackOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary1);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); response.Error = response.Error + this._error; }

				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponsePresenterService :: Request37 " + exp.Message);
			}
		}

		public Response36 Request35(Request35 request)
		{
			//IRequest2ResponseProxy bibliothecaProxyClient = UiConnection.GetSelectedClientProxy();  // V_ClientProxy
			//UiConnection.ClearOutputText();

			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy
			if (bibliothecaProxyClient == null) return new Response36();

			try
			{
				if (request == null) return new Response36();
				//UiConnection.UpdateOutputText("Request" + request.Operation + " >> [" + request.Request + "]" + Environment.NewLine);
				//UiConnection.UpdateLogRequestMessageText("35 Request : " + Environment.NewLine + OperationContext.Current.RequestContext.RequestMessage.ToString() + Environment.NewLine);

				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					Response36 response36 = new Response36(request as Sip2BaseData);
					//UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
					response36.Error = response36.Error + this._error;
					return response36;
				}

				//REQUEST 
				//Dictionary<string, string> propertyDictionary = this.FillFromRequestData(request, out this._error);
				//UiConnection.UpdateListViewRequest(this._codeOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); request.Error = request.Error + this._error; }

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response36 response = bibliothecaProxyClient.Request35Proxy(request);
				//Dictionary<string, string> propertyDictionary1 = this.FillFromResponseData(response, out this._error);

				//UiConnection.UpdateListViewResponse(this._codeBackOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary1);
				//if (string.IsNullOrWhiteSpace(this._error) != true) { UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine); response.Error = response.Error + this._error; }

				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponsePresenterService :: Request35 " + exp.Message);
			}
		}



		//=========================
		private void InitCode(Sip2BaseData request)
		{
			this._error = "";
			this._codeOperationEnum = CodeOperationEnum.CodeOperationUnknown;
			this._codeBackOperation = "";
			this._codeOperation = "";
			if (request == null) return;

			this._codeOperationEnum = ConvertData.GetCodeOperationEnum(request.Request);
			this._codeOperation = ConvertData.GetCodeOperationString(this._codeOperationEnum);
			this._codeBackOperation = ConvertData.GetCodeBackOperationString(this._codeOperationEnum);
			if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown) this._error = "Code Operation Unknown";
		}

		//private Dictionary<string, string> FillResponseSampleData(Sip2BaseData response, out string error)
		//{
		//	string codeOperation = response.Operation;
		//	Dictionary<string, string> propertyDictionary = new Dictionary<string, string>();
		//	// обойти словарь из конфига и все данные по правилам сконкотенировать
		//	UtilsSip2Protocol sip2Protocol = new UtilsSip2Protocol();
		//	try
		//	{
		//		XElement xElement = sip2Protocol.GetXDocumentOperation(codeOperation, out error);
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
		//						props[i].SetValue(response, field.datasample);
		//						propertyDictionary[props[i].Name.ToLower()] = field.datasample;
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception exp)
		//	{
		//		error = exp.Message;
		//	}
		//	return propertyDictionary;
		//}



		//private Dictionary<string, string> FillFromRequestData(Sip2BaseData request, out string error)
		//{
		//	string codeOperation = request.Operation;
		//	string _error = "";
		//	Dictionary<string, string> propertyDictionary = new Dictionary<string, string>();
		//	// обойти словарь из конфига и все данные по правилам сконкотенировать
		//	UtilsSip2Protocol sip2Protocol = new UtilsSip2Protocol();
		//	try
		//	{
		//		XElement xElement = sip2Protocol.GetXDocumentOperation(codeOperation, out _error);
		//		Operation operation = new Operation(xElement);
		//		Fields fields = operation.fields;
		//		//надо перебрать весь  field из описания протокола и по рефлексии найти проперти (у request) с таким же имени
		//		PropertyInfo[] props = request.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
		//		//этому проперти (в словаре) присвоить данные 
		//		//которые распарсить по правилам в объекте 
		//		foreach (Field field in fields) // sip2Protocol
		//		{
		//			for (int i = 0; i < props.Length; i++) //requestData
		//			{
		//				if (props[i].Name.ToLower() == field.name.ToLower())   
		//				{
		//					if (field.datacontract == "1")
		//					{
		//						propertyDictionary[props[i].Name.ToLower()] = (string)props[i].GetValue(request, null);
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception exp)
		//	{
		//		_error = _error + " : " + exp.Message + "--" + exp.StackTrace;
		//	}
		//	error = _error;
		//	return propertyDictionary;
		//}

		//private Dictionary<string, string> FillFromResponseData(Sip2BaseData response, out string error)
		//{
		//	string codeOperation = response.Operation;
		//	string _error = "";
		//	Dictionary<string, string> propertyDictionary = new Dictionary<string, string>();
		//	// обойти словарь из конфига и все данные по правилам сконкотенировать
		//	UtilsSip2Protocol sip2Protocol = new UtilsSip2Protocol();
		//	try
		//	{
		//		XElement xElement = sip2Protocol.GetXDocumentOperation(codeOperation, out _error);
		//		Operation operation = new Operation(xElement);
		//		Fields fields = operation.fields;
		//		//надо перебрать весь  field из описания протокола и по рефлексии найти проперти (у response) с таким же имени
		//		PropertyInfo[] props = response.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
		//		//этому проперти (в словаре) присвоить данные 
		//		//которые распарсить по правилам в объекте 
		//		foreach (Field field in fields) // sip2Protocol
		//		{
		//			for (int i = 0; i < props.Length; i++) //responseData
		//			{
		//				if (props[i].Name.ToLower() == field.name.ToLower())
		//				{
		//					if (field.datacontract == "1")
		//					{
		//						propertyDictionary[props[i].Name.ToLower()] = (string)props[i].GetValue(response, null);
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception exp)
		//	{
		//		_error = _error + " : " + exp.Message + "--" + exp.StackTrace;
		//	}
		//	error =  _error;
		//	return propertyDictionary;
		//}


		public string RequestSip2(string request)
		{
			try
			{
				string ret = "srart";
				var client = new HttpClient();
				client.BaseAddress = new Uri("http://localhost:12504/");

				// Клиент Web API
				//var client = new HttpClient();
				//client.BaseAddress = new Uri("http://localhost:1234/");
				//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				//// GET
				//var response = client.GetAsync("api/data").Result;
				//if (response.IsSuccessStatusCode)
				//{
				//	// reference System.Net.Http.Formatting.dll, Newtonsoft.Json.dll
				//	var items = response.Content.ReadAsAsync<IEnumerable<int>>().Result;
				//}

				//// POST
				//response = client.PostAsJsonAsync("api/data", new { Id = 123, Title = "Post" }).Result;
				//if (response.IsSuccessStatusCode)
				//{
				//	var location = response.Headers.Location;
				//}
				//==================
				//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
				//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
				//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
//				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
				//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
				//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
				//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
				
				 
			


				// GET
				//var response = client.GetAsync("Home/ClientSideCreation").Result;
				//!!! PAБОТАЕТ
				//HttpResponseMessage response = client.GetAsync("Home/GetField").Result;
				//if (response.IsSuccessStatusCode)
				//{
				//		ret = response.Content.ReadAsStringAsync().Result;
					
				//}

				//// POST
				//!!! Работает
				var response = client.PostAsJsonAsync("Home/GetField", new { value1 = "123", value2 = "Post" }).Result;
				if (response.IsSuccessStatusCode)
				{
					ret = response.Content.ReadAsStringAsync().Result;
				}

				
			
				//HttpResponseMessage response1 = client.GetAsync("Home/Get").Result;
				
				//response1.Content.LoadIntoBufferAsync();

				//if (response1.IsSuccessStatusCode)
				//{
				//	var ret1 = "";
				//	var contentType = response1.Content.Headers.ContentType;

				//	var items = response1.Content.ReadAsFormDataAsync().Result;
					
				//	foreach (var item in items)
				//	{
				//		ret1 = ret1 + item + ",";
				//	}
				//	ret1 = ret1.TrimEnd(',');
				//}
				return ret;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponsePresenterService :: Request99 " + exp.Message);
			}

		}

		//		var client = new HttpClient();
		// client.BaseAddress = new Uri("http://localhost:1234/");
		// client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

		//// GET
		//var response = client.GetAsync("api/data").Result;
		//if (response.IsSuccessStatusCode)
		// {
		//	// reference System.Net.Http.Formatting.dll, Newtonsoft.Json.dll
		//	var items = response.Content.ReadAsAsync<IEnumerable<int>>().Result; 
		//}

		//// POST
		//response = client.PostAsJsonAsync("api/data", new { Id = 123, Title = "Post" }).Result;
		//if (response.IsSuccessStatusCode)
		// {
		//	var location = response.Headers.Location;
		//}
		public Sip2BaseData RequestSip2BaseData(string request)
		{
			throw new NotImplementedException();
		}
	}
}
