using System.ServiceModel;
using System.Runtime.Serialization;
using IdSip2.DataContract;

namespace IdSip2.ServiceContract
{
	[ServiceContract]
	public interface IRequest2Object
	{
		[OperationContract]
		Sip2BaseData RequestSip2(string _requestString);

	}
}









