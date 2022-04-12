using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Common.WCF;
using IdSip2.DataContract;
using IdSip2.ServiceContract;
using Idsip2.ServiceClient.AleKotarimBibliothecaClient;
using IdSip2.ServiceContract.Common;
using IdSip2.Common;
using System.ServiceModel.Channels;


namespace IdSip2.ClientPresenter
{
	// V_Client  - IISTest не рассматривать пока не появится IIS у заказчика
	public class IISTestDataBibliothecaProxyClient : IRequest2ResponseProxy
	{

		private string _abcTcpName = "tcpIRequest2Response";
		private string _abcHttpName = "httpIRequest2Object";
		private string _abcName = "tcpIRequest2Response";
		BasicHttpBinding _binding ;
		EndpointAddress _endpointAddress ;


		public IISTestDataBibliothecaProxyClient(string[] args = null)
		{
			//this._binding = new BasicHttpBinding();
			//this._binding.ReceiveTimeout = TimeSpan.MaxValue;
			//this._binding.SendTimeout = TimeSpan.MaxValue;
			//this._binding.OpenTimeout = TimeSpan.FromMinutes(2f);
			//this._binding.CloseTimeout = TimeSpan.MaxValue;
			//HttpTransportBindingElement be = new HttpTransportBindingElement();
			//be.KeepAliveEnabled = false;
			//request.KeepAlive = false;
			//request.ConnectionGroupName = DateTime.Now.Ticks.ToString(); 
			//this._endpointAddress = new EndpointAddress(@"http://demo2010.libraries.co.il/BuildaGate5library/general/bibliotheca.php");
			
		}

			//	using (ServiceReference1.selfServiceBorrowReturnPortTypeClient client1 = new ServiceReference1.selfServiceBorrowReturnPortTypeClient())
			//{
			//	client1.Open();
			//	//var response10 = client1.SIP2_CheckIn(new ServiceReference1.SIP2_CheckInRequest("","","",""));  //Request09
			//	//var response12 = client1.SIP2_Checkout(new ServiceReference1.SIP2_CheckoutRequest("", "", "", "",""));  //Request10
			//	var response18 = client1.SIP2_ItemInfo(new ServiceReference1.SIP2_ItemInfoRequest("","",""));//Request17
			//	var response64 = client1.SIP2_Patron_Info(new ServiceReference1.SIP2_Patron_InfoRequest("", "", "","","")); //Request63
			//	var response98 = client1.SIP2_Status(new ServiceReference1.SIP2_StatusRequest("", "")); //Request99
			//}


		public Response98 Request99Proxy(Request99 request)
		{
			var response = new Response98(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcHttpName, new ClientEndpointBehavior());
				//inject my endpoint behavior
		

				response = factoryWrapper.Execute(proxy => proxy.Request99(request));
				return response;

			}
			catch (Exception ex)
			{
				response.Result = ex.GetType().ToString() + " ---  " + ex.Message;
				response.Error = ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}

		public Response64 Request63Proxy(Request63 request)
		{
			var response = new Response64(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcHttpName, new ClientEndpointBehavior());
				response = factoryWrapper.Execute(proxy => proxy.Request63(request));
				return response;

			}
			catch (Exception ex)
			{
				response.Result = ex.GetType().ToString() + " ---  " + ex.Message;
				response.Error = ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}


		public Response12 Request11Proxy(Request11 request)
		{
			var response = new Response12(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcHttpName, new ClientEndpointBehavior());
				
					
				response = factoryWrapper.Execute(proxy => proxy.Request11(request));
				return response;

			}
			catch (Exception ex)
			{
				response.Result = ex.GetType().ToString() + " ---  " + ex.Message;
				response.Error = ex.GetType().ToString() + " ---  " + ex.Message;
				return response;
			}
		}

		public Response94 Request93Proxy(Request93 request)
		{
			var response = new Response94(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcHttpName, new ClientEndpointBehavior());
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

		public Response18 Request17Proxy(Request17 request)
		{
			var response = new Response18(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcHttpName, new ClientEndpointBehavior());
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

	
		public Response10 Request09Proxy(Request09 request)
		{
			var response = new Response10(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcHttpName, new ClientEndpointBehavior());
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


		public Response30 Request29Proxy(Request29 request)
		{
			var response = new Response30(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcHttpName, new ClientEndpointBehavior());
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


		public Response38 Request37Proxy(Request37 request)
		{
			var response = new Response38(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcHttpName, new ClientEndpointBehavior());
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

		public Response36 Request35Proxy(Request35 request)
		{
			var response = new Response36(request as Sip2BaseData);
			try
			{
				//var factoryWrapper = new FactoryWrapper<IRequest2Response>(this._bindingRequest2Response, new EndpointAddress(this._address2ResponseObject));
				var factoryWrapper = new FactoryWrapper<IRequest2Response>(_abcHttpName, new ClientEndpointBehavior());
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

