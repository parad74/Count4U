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

namespace IdSip2.Service
{
	// VII - ITopsysBibliotheca Server
	public class TopsysBibliothecaService : ITopsysBibliotheca//IRequest2ResponseProxy
	{

		private string _error;
		private CodeOperationEnum _codeOperationEnum;
		private string _codeBackOperation;
		private string _codeOperation;

		NetTcpBinding _binding;
		EndpointAddress _endpointAddress;

		public TopsysBibliothecaService()
			: base()
		{
			this._error = "";
			this._codeOperationEnum = CodeOperationEnum.CodeOperationUnknown;
			this._codeBackOperation = "";
			this._codeOperation = "";

			this._binding = new NetTcpBinding();
			//this._endpointAddress = new EndpointAddress(@"net.tcp://localhost:10055/TopsysBibliothecaService");
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


		public Response98 Request99Topsys(Request99 request)
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

		public Response64 Request63Topsys(Request63 request)
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

		public Response12 Request11Topsys(Request11 request)
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

		public Response94 Request93Topsys(Request93 request)
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

		public Response18 Request17Topsys(Request17 request)
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

		public Response10 Request09Topsys(Request09 request)
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

		public Response30 Request29Topsys(Request29 request)
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

		public Response38 Request37Topsys(Request37 request)
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

		public Response36 Request35Topsys(Request35 request)
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
