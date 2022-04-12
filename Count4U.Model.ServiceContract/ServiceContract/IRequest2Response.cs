using System.ServiceModel;
using System.Runtime.Serialization;
using IdSip2.DataContract;
using System.ServiceModel.Web;

namespace IdSip2.ServiceContract
{
	[ServiceContract(Namespace = "http://IdSip2.ServiceContract")]
	public interface IRequest2Response
	{
		[OperationContract]
		string RequestSip2(string request);

		[OperationContract]
		Sip2BaseData RequestSip2BaseData(string request);

		[OperationContract]
		//[WebGet(UriTemplate = "weather/{state}/{city}"
		Response98 Request99(Request99 request);
		
		[OperationContract]
		Response64 Request63(Request63 request);

		[OperationContract]
		Response12 Request11(Request11 request);

		[OperationContract]
		Response94 Request93(Request93 request);

		[OperationContract]
		Response18 Request17(Request17 request);

		[OperationContract]
		Response10 Request09(Request09 request);

		[OperationContract]
		Response30 Request29(Request29 request);

		[OperationContract]
		Response38 Request37(Request37 request);

		[OperationContract]
		Response36 Request35(Request35 request);

	}
}









