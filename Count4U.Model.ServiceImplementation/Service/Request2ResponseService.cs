using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Practices.ServiceLocation;

namespace IdSip2.ServiceImplementation
{
	// Server Request2Response (Host на WinForm) (аналог должен? хостится на WindowsServise) или можно один и тот же захостить?
	// IV_Request2ResponseServer (сервер) - 
	//где заполняются данные - вызовом прокси-клиента(bibliothecaProxyClient V_ClientProxy(не сервис клиент, просто класс)) 
	// внешнего сервера (конечной точки пользователя) которые могут переключаться
	// вызывается из II_Request2ResponseClient
	// ( II_Server Sip2Socket  вызывает -> II_Request2ResponseClient -> IV_Request2ResponseServer -> V_ClientProxy(не сервис клиент, просто класс))
	public class Request2ResponseService : IRequest2Response
	{

		private string _error;
		private CodeOperationEnum _codeOperationEnum;
		private string _codeBackOperation;
		private string _codeOperation;

		public Request2ResponseService()	: base()
		{
			this._error = "";
			this._codeOperationEnum = CodeOperationEnum.CodeOperationUnknown;
			this._codeBackOperation = "";
			this._codeOperation = "";
		}

		public Response64 Request63(Request63 request)
		{
			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy не сервис клиент, просто класс
			if (bibliothecaProxyClient == null) return new Response64();
	
			try
			{
				if (request == null) return new Response64();
				
				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					return new Response64(request as Sip2BaseData);
				}

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response64 response = bibliothecaProxyClient.Request63Proxy(request);
				response._summary = request.summary;
				response.Result = "Request2ResponseService" + response.Result;
				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponseService :: Request63 " + exp.Message);
			}
		}

		public Response98 Request99(Request99 request)
		{
			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy не сервис клиент, просто класс
			  
			// Idea - получает SessionID
			//if (bibliothecaProxyClient.GetType() == IdeaBibliothecaProxyClient)
			//{
				
			//}

			if (bibliothecaProxyClient == null) return new Response98();

			try
			{
				if (request == null) return new Response98();

				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					return new Response98(request as Sip2BaseData);
				}

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response98 response = bibliothecaProxyClient.Request99Proxy(request);
				response.Result = "Request2ResponseService" + response.Result;
				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponseService :: Request99 " + exp.Message);
			}
		}

		public Response12 Request11(Request11 request)
		{
			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy не сервис клиент, просто класс
			if (bibliothecaProxyClient == null) return new Response12();

			try
			{
				if (request == null) return new Response12();
	
				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					return new Response12(request as Sip2BaseData);
				}

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response12 response = bibliothecaProxyClient.Request11Proxy(request);
				return response;
			}
			catch(Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponseService :: Request11 " + exp.Message);
			}

		}


		// Idea - получает SessionID
		public Response94 Request93(Request93 request)
		{
			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy не сервис клиент, просто класс
			if (bibliothecaProxyClient == null) return new Response94();
			try
			{
				if (request == null) return new Response94();
	
				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					return new Response94(request as Sip2BaseData);
				}

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response94 response = bibliothecaProxyClient.Request93Proxy(request);
				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponseService :: Request93 " + exp.Message);
			}
		}

		public Response18 Request17(Request17 request)
		{
			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy не сервис клиент, просто класс
			if (bibliothecaProxyClient == null) return new Response18();

			try
			{
				if (request == null) return new Response18();
	
				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					return new Response18(request as Sip2BaseData);
				}

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response18 response = bibliothecaProxyClient.Request17Proxy(request);
				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponseService :: Request17 " + exp.Message);
			}
		}

		public Response10 Request09(Request09 request)
		{
			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy не сервис клиент, просто класс
			if (bibliothecaProxyClient == null) return new Response10();

			try
			{
				if (request == null) return new Response10();
		
				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					return new Response10(request as Sip2BaseData);
				}

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response10 response = bibliothecaProxyClient.Request09Proxy(request);
				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponseService :: Request09 " + exp.Message);
			}
		}

		public Response30 Request29(Request29 request)
		{
			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy не сервис клиент, просто класс
			if (bibliothecaProxyClient == null) return new Response30();
			try
			{
				if (request == null) return new Response30();
	
				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					return new Response30(request as Sip2BaseData);
				}

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response30 response = bibliothecaProxyClient.Request29Proxy(request);
				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponseService :: Request29 " + exp.Message);
			}
		}

		public Response38 Request37(Request37 request)
		{
			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy
			if (bibliothecaProxyClient == null) return new Response38();
			try
			{
				if (request == null) return new Response38();

				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					return new Response38(request as Sip2BaseData);
				}

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response38 response = bibliothecaProxyClient.Request37Proxy(request);
				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponseService :: Request37 " + exp.Message);
			}
		}

		public Response36 Request35(Request35 request)
		{
			IRequest2ResponseProxy bibliothecaProxyClient = ServiceClientModuleInit.BibliothecaProxyClientCurrent; //V_ClientProxy не сервис клиент, просто класс
			if (bibliothecaProxyClient == null) return new Response36();

			try
			{
				if (request == null) return new Response36();
	
				this.InitCode(request as Sip2BaseData);
				if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown)
				{
					return new Response36(request as Sip2BaseData);
				}

				//RESPONSE- Fill bibliothecaProxyClient  ResponseData
				Response36 response = bibliothecaProxyClient.Request35Proxy(request);
				return response;
			}
			catch (Exception exp)
			{
				throw new FaultException<Exception>(exp, "Request2ResponseService :: Request35 " + exp.Message);
			}
		}



		//=========================
		private void InitCode(Sip2BaseData request)
		{
			this._error = "";
			this._codeOperationEnum = CodeOperationEnum.CodeOperationUnknown;
			this._codeBackOperation = "";
			this._codeOperation = "";

			this._codeOperationEnum = ConvertData.GetCodeOperationEnum(request.Request);
			this._codeOperation = ConvertData.GetCodeOperationString(this._codeOperationEnum);
			this._codeBackOperation = ConvertData.GetCodeBackOperationString(this._codeOperationEnum);
			if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown) this._error = "Code Operation Unknown";
		}



		public string RequestSip2(string request)
		{
			throw new NotImplementedException();
		}

		public Sip2BaseData RequestSip2BaseData(string request)
		{
			throw new NotImplementedException();
		}
	}
}
