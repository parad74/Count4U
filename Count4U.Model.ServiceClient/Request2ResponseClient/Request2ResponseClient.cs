using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Common.WCF;

namespace 	Count4U.Model.ServiceClient
{
	//II_Request2ResponseClient  клиет вызывает сервер (IV_Request2ResponseServer : IRequest2Response)
	public class Request2ResponseClient
	{
		private string _abcTcpName = "tcpIRequest2Response";
		//private string _abcHttpName = "httpIRequest2Object";
		private string _abcName = "tcpIRequest2Response";

		
		// Указание, где ожидать входящие сообщения.
		//Uri _address2ResponseObject = new Uri(@"net.tcp://localhost:10074/Request2ResponseService");

		// Указание, как обмениваться сообщениями.
		//NetTcpBinding _bindingRequest2Response = new NetTcpBinding();
		   //<endpoint address="net.tcp://localhost:10074/Request2ResponseService"
		   //binding="netTcpBinding" bindingConfiguration="TCPBinding_IDataService"
		   //contract="IdSip2.ServiceContract.IRequest2Response" name="tcpIRequest2Response" />

		//OLD
		// Имя конечной  точки задается в аргументах , которые определяются по разному  для  WinForm и WinodwsService хостинга
		// Друг от друга не зхависят.
		// WinForm - конфигурится из командной строки - IIS или SERVER
		// WinodwsService в файле C:\idsip2\trunk\IdSip2\IdSip2.Provider\IdSip2.Common\app.config   -  "IIS" = "IIS"
		  //<setting name="IIS" serializeAs="String">
		//	  <value>IIS</value>
		  //  </setting>
		// для IIS хостинга необходимо чтобы среди аргументов была строка "IIS" - иначе хостимся на форму
		//public Request2ResponseClient(string[] args =null)
		public Request2ResponseClient()
		{
	
		//if (args == null) this._abcName = _abcTcpName;
			//if (args.Contains("IIS") == true) this._abcName = _abcHttpName;
			//else
			this._abcName = _abcTcpName;

			//this._bindingRequest2Response.ReceiveTimeout = TimeSpan.MaxValue;
			//this._bindingRequest2Response.SendTimeout = TimeSpan.MaxValue;
			//this._bindingRequest2Response.OpenTimeout = TimeSpan.FromMinutes(2f);
			//this._bindingRequest2Response.CloseTimeout = TimeSpan.MaxValue;

		}

		// from Request2ObjectService
		// to 
		//тестирование клиента sip2IIS с возвратом сообщения
		public string RequestSip2(string request)
		{
			var response = "empty";
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcName);
				response = factoryWrapper.Execute(proxy => proxy.RequestSip2(request));
				return response;

			}
			catch (Exception ex)
			{
				response = ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}

		public Response98 Request99(Request99 request)
		{
			var response = new Response98(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcName);
				response = factoryWrapper.Execute(proxy => proxy.Request99(request));
				response.Result = "Request2ResponseClient2" + response.Result;
				return response;
							
			}
			catch (Exception ex)
			{
				response.Result = "Request2ResponseClient2" + ex.GetType().ToString() + " ---  " + ex.Message;
				response.Error = "Request2ResponseClient2" + ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}

		public Response64 Request63(Request63 request)
		{
			var response = new Response64(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcName);
				response = factoryWrapper.Execute(proxy => proxy.Request63(request));
				response.Result = "Request2ResponseClient2" + response.Result;
				return response;

			}
			catch (Exception ex)
			{
				response.Result = "Request2ResponseClient2 err" + ex.GetType().ToString() + " ---  " + ex.Message;
				response.Error = "Request2ResponseClient2 err" + ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}

		public Response12 Request11(Request11 request)
		{
			var response = new Response12(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcName);
				response = factoryWrapper.Execute(proxy => proxy.Request11(request));
				return response;

			}
			catch (Exception ex)
			{
				response.Result = ex.GetType().ToString() + " ---  "+ ex.Message;
				response.Error = ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}

		public Response94 Request93(Request93 request)
		{
			var response = new Response94(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcName);
				response = factoryWrapper.Execute(proxy => proxy.Request93(request));
				return response;

			}
			catch (Exception ex)
			{
				response.Result = ex.GetType().ToString() + " ---  " + ex.Message;
				response.Error = ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}

		public Response18 Request17(Request17 request)
		{
			var response = new Response18(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcName);
				response = factoryWrapper.Execute(proxy => proxy.Request17(request));
				return response;

			}
			catch (Exception ex)
			{
				response.Result = ex.GetType().ToString() + " ---  " + ex.Message;
				response.Error = ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}


		public Response10 Request09(Request09 request)
		{
			var response = new Response10(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcName);
				response = factoryWrapper.Execute(proxy => proxy.Request09(request));
				return response;

			}
			catch (Exception ex)
			{
				response.Result = ex.GetType().ToString() + " ---  " + ex.Message;
				response.Error = ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}


		public Response30 Request29(Request29 request)
		{
			var response = new Response30(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcName);
				response = factoryWrapper.Execute(proxy => proxy.Request29(request));
				return response;

			}
			catch (Exception ex)
			{
				response.Result = ex.GetType().ToString() + " ---  " + ex.Message;
				response.Error = ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}


		public Response38 Request37(Request37 request)
		{
			var response = new Response38(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcName);
				response = factoryWrapper.Execute(proxy => proxy.Request37(request));
				return response;

			}
			catch (Exception ex)
			{
				response.Result = ex.GetType().ToString() + " ---  " + ex.Message;
				response.Error = ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}

		public Response36 Request35(Request35 request)
		{
			var response = new Response36(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcName);
				response = factoryWrapper.Execute(proxy => proxy.Request35(request));
				return response;

			}
			catch (Exception ex)
			{
				response.Result = ex.GetType().ToString() + " ---  " + ex.Message;
				response.Error = ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}

	
	
	}
}

