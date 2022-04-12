using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Common.WCF;
using System.ServiceModel.Channels;
using Count4U.Model.ServiceContract;

namespace Count4U.Model.ProxyClient
{
	// V_ClientProxy "Test Data"  - просто класс - не сервис клиент
	public class TestDataBibliothecaProxyClient : IRequest2ResponseProxy
	{
		private string _name;
		//public TestDataBibliothecaProxyClient(string[] args = null)
		public TestDataBibliothecaProxyClient()
		{
				
		}

	
		//public Response98 Request99Proxy(Request99 request)
		//{
		//	string error = "";
		//	Response98 response = new Response98(request as Sip2BaseData);
		//	response.Result = "TestDataBibliothecaProxyClient 1+ " + response.Result;
		//	Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
		//	if (string.IsNullOrWhiteSpace(error) == false)
		//	{
		//		response.Result = response.Result + "--" + error;
		//		response.Error = response.Error + "--" + error;
		//	}
		//	return response;
		//}

		//public Response64 Request63Proxy(Request63 request)
		//{
		//	string error = "";
		//	Response64 response = new Response64(request as Sip2BaseData);
		//	response.Result = "TestDataBibliothecaProxyClient 1+ " + response.Result;
		//	Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
		//	if (string.IsNullOrWhiteSpace(error) == false)
		//	{
		//		response.Result = response.Result + "--" + error;
		//		response.Error = response.Error + "--" + error;
		//	}
		//	return response;
		//}

		//public Response12 Request11Proxy(Request11 request)
		//{
		//	string error = "";
		//	Response12 response = new Response12(request as Sip2BaseData);
		//	Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
		//	response.item_identifier = request.item_identifier;
		//	response.title_identifier = response.title_identifier + request.item_identifier;
		//	propertyDictionary1["item_identifier"] = response.item_identifier;
		//	propertyDictionary1["title_identifier"] = response.title_identifier;
		//	if (string.IsNullOrWhiteSpace(error) == false)
		//	{
		//		response.Result = response.Result + "--" + error;
		//		response.Error = response.Error + "--" + error;
		//	} 
		//	return response;
		//}

		//public Response94 Request93Proxy(Request93 request)
		//{
		//	string error = "";
		//	Response94 response = new Response94(request as Sip2BaseData);
		//	Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
		//	if (string.IsNullOrWhiteSpace(error) == false)
		//	{
		//		response.Result = response.Result + "--" + error;
		//		response.Error = response.Error + "--" + error;
		//	}
		//	return response;
		//}

		//public Response18 Request17Proxy(Request17 request)
		//{
		//	string error = "";
		//	Response18 response = new Response18(request as Sip2BaseData);
		//	Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
		//	response.item_identifier = request.item_identifier;
		//	response.title_identifier = response.title_identifier + request.item_identifier;
		//	propertyDictionary1["item_identifier"] = response.item_identifier;
		//	propertyDictionary1["title_identifier"] = response.title_identifier;

		//	if (string.IsNullOrWhiteSpace(error) == false)
		//	{
		//		response.Result = response.Result + "--" + error;
		//		response.Error = response.Error + "--" + error;
		//	}
		//	return response;

		//}

		////var response10 = client1.SIP2_CheckIn(new ServiceReference1.SIP2_CheckInRequest("", "", "", ""));  //Request09
		//public Response10 Request09Proxy(Request09 request)
		//{

		//	string error = "";
		//	Response10 response = new Response10(request as Sip2BaseData);
		//	Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
		//	if (string.IsNullOrWhiteSpace(error) == false)
		//	{
		//		response.Result = response.Result + "--" + error;
		//		response.Error = response.Error + "--" + error;
		//	}
		//	return response;
		//}


		//public Response30 Request29Proxy(Request29 request)
		//{

		//	string error = "";
		//	Response30 response = new Response30(request as Sip2BaseData);
		//	Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
		//	if (string.IsNullOrWhiteSpace(error) == false)
		//	{
		//		response.Result = response.Result + "--" + error;
		//		response.Error = response.Error + "--" + error;
		//	}
		//	return response;
		//}


		//public Response38 Request37Proxy(Request37 request)
		//{
		//	string error = "";
		//	Response38 response = new Response38(request as Sip2BaseData);
		//	Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
		//	if (string.IsNullOrWhiteSpace(error) == false)
		//	{
		//		response.Result = response.Result + "--" + error;
		//		response.Error = response.Error + "--" + error;
		//	}
		//	return response;
		//}

		//public Response36 Request35Proxy(Request35 request)
		//{
		//	string error = "";
		//	Response36 response = new Response36(request as Sip2BaseData);
		//	Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
		//	if (string.IsNullOrWhiteSpace(error) == false)
		//	{
		//		response.Result = response.Result + "--" + error;
		//		response.Error = response.Error + "--" + error;
		//	}
		//	return response;
		//}



		//public string Name
		//{
		//	get
		//	{
		//		return _name;
		//	}
		//	set
		//	{
		//		_name = value;
		//	}
		//}
	}
}

