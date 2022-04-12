using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IdSip2.Common;
using IdSip2.Common.Protocol;
using IdSip2.DataContract;
using IdSip2.ServiceContract;
using IdSip2.ServiceContract.Common;
using Microsoft.Practices.ServiceLocation;

namespace IdSip2.ServiceImplementation
{
	// VI - IRequest2ResponseProxy Server
	public class TopsysBibliothecaProxyService : IRequest2ResponseProxy
	{

		private string _error;
		private CodeOperationEnum _codeOperationEnum;
		private string _codeBackOperation;
		private string _codeOperation;

		NetTcpBinding _binding;
		EndpointAddress _endpointAddress;

		public TopsysBibliothecaProxyService()
			: base()
		{
			this._error = "";
			this._codeOperationEnum = CodeOperationEnum.CodeOperationUnknown;
			this._codeBackOperation = "";
			this._codeOperation = "";

			this._binding = new NetTcpBinding();
			this._endpointAddress = new EndpointAddress(@"net.tcp://localhost:10002/TopsysBibliothecaProxyService");
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
			return "";
		}

		public Sip2BaseData RequestSip2BaseData(string request)
		{
			return new Sip2BaseData();
		}

		public Response98 Request99Proxy(Request99 request)
		{
			string error = "";
			Response98 response = new Response98(request as Sip2BaseData);
			response.Result = "TestDataBibliothecaProxyClient 99 + " + response.Result;
			Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
			if (string.IsNullOrWhiteSpace(error) == false)
			{
				response.Result = response.Result + "--" + error;
				response.Error = response.Error + "--" + error;
			}
			return response;
		}

		public Response64 Request63Proxy(Request63 request)
		{
			string error = "";
			Response64 response = new Response64(request as Sip2BaseData);
			response.Result = "TestDataBibliothecaProxyClient 63 + " + response.Result;
			Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
			if (string.IsNullOrWhiteSpace(error) == false)
			{
				response.Result = response.Result + "--" + error;
				response.Error = response.Error + "--" + error;
			}
			return response;
		}

		public Response12 Request11Proxy(Request11 request)
		{
			string error = "";
			Response12 response = new Response12(request as Sip2BaseData);
			Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
			response.item_identifier = request.item_identifier;
			response.title_identifier = response.title_identifier + "Request12";
			propertyDictionary1["item_identifier"] = response.item_identifier;
			propertyDictionary1["title_identifier"] = response.title_identifier;
			if (string.IsNullOrWhiteSpace(error) == false)
			{
				response.Result = response.Result + "--" + error;
				response.Error = response.Error + "--" + error;
			}
			return response;
		}

		public Response94 Request93Proxy(Request93 request)
		{
			string error = "";
			Response94 response = new Response94(request as Sip2BaseData);
			Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
			if (string.IsNullOrWhiteSpace(error) == false)
			{
				response.Result = response.Result + "--" + error;
				response.Error = response.Error + "--" + error;
			}
			return response;
		}

		public Response18 Request17Proxy(Request17 request)
		{
			string error = "";
			Response18 response = new Response18(request as Sip2BaseData);
			Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
			response.item_identifier = request.item_identifier;
			response.title_identifier = response.title_identifier + "Request17";
			propertyDictionary1["item_identifier"] = response.item_identifier;
			propertyDictionary1["title_identifier"] = response.title_identifier;

			if (string.IsNullOrWhiteSpace(error) == false)
			{
				response.Result = response.Result + "--" + error;
				response.Error = response.Error + "--" + error;
			}
			return response;
		}

		public Response10 Request09Proxy(Request09 request)
		{
			string error = "";
			Response10 response = new Response10(request as Sip2BaseData);
			Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
			if (string.IsNullOrWhiteSpace(error) == false)
			{
				response.Result = response.Result + "--" + error;
				response.Error = response.Error + "--" + error;
			}
			return response;
		}

		public Response30 Request29Proxy(Request29 request)
		{
			string error = "";
			Response30 response = new Response30(request as Sip2BaseData);
			Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
			if (string.IsNullOrWhiteSpace(error) == false)
			{
				response.Result = response.Result + "--" + error;
				response.Error = response.Error + "--" + error;
			}
			return response;
		}

		public Response38 Request37Proxy(Request37 request)
		{
			string error = "";
			Response38 response = new Response38(request as Sip2BaseData);
			Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
			if (string.IsNullOrWhiteSpace(error) == false)
			{
				response.Result = response.Result + "--" + error;
				response.Error = response.Error + "--" + error;
			}
			return response;
		}

		public Response36 Request35Proxy(Request35 request)
		{
			string error = "";
			Response36 response = new Response36(request as Sip2BaseData);
			Dictionary<string, string> propertyDictionary1 = UtilsSip2ProtocolOperation.FillResponseSampleData(response, out error);
			if (string.IsNullOrWhiteSpace(error) == false)
			{
				response.Result = response.Result + "--" + error;
				response.Error = response.Error + "--" + error;
			}
			return response;
		}
	}
}
