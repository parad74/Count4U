using System.ServiceModel;
using System.Runtime.Serialization;
using Count4U.Model.ServiceContract.MessageContract;

namespace Count4U.Model.ServiceContract
{
	[ServiceContract]
	public interface ISimpleMessage
	{
		[OperationContract]
		Process4UMessage Next(Process4UMessage message);
		[OperationContract]
		Process4UMessage GetSip2(Process4UMessage message);
	}
}









