using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Common.WCF;
using IdSip2.DataContract;
using IdSip2.ServiceContract;

namespace IdSip2.ServiceClient
{
	public class Request2ObjectClient
	{
		//private string _abcName = "tcpIRequest2Object";

		Uri _addressRequest2Object = new Uri(@"net.tcp://localhost:10274/Request2ObjectService");

		// Указание, как обмениваться сообщениями.
		NetTcpBinding _bindingRequest2Object = new NetTcpBinding();

		// Ссылка на экзкмпляр ServiceHost.
		ServiceHost _request2ObjectServiceHost;
	
	
		public Request2ObjectClient(string abcName = "")
		{
			//if (string.IsNullOrWhiteSpace(abcName) == false)
			//	this._abcName = abcName;

			this._bindingRequest2Object.ReceiveTimeout = TimeSpan.MaxValue;
			this._bindingRequest2Object.SendTimeout = TimeSpan.MaxValue;
			this._bindingRequest2Object.OpenTimeout = TimeSpan.FromMinutes(2f);
			this._bindingRequest2Object.CloseTimeout = TimeSpan.MaxValue;
		}


		// from Sip2TCPSocketServer.SendToSimpleDataClient
		// to 
		//тестирование клиента sip2IIS с возвратом сообщения
		public Sip2BaseData RequestSip2(string request)
		{
			Sip2BaseData response = new Sip2BaseData();
			response.Request = request;
			try
			{
				var factoryWrapper = new FactoryWrapper<IRequest2Object>(this._bindingRequest2Object, new EndpointAddress(this._addressRequest2Object));
				//var factoryWrapper = new FactoryWrapper<IRequest2Object>(_abcName);
				response = factoryWrapper.Execute(proxy => proxy.RequestSip2(request));
				return response;
			}
			catch (Exception ex)
			{
				response.Result = ex.Message;
				return response;
			}
		}

	
	
	}
}

