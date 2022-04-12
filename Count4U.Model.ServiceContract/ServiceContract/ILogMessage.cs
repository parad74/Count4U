using System.ServiceModel;
using System.Runtime.Serialization;
using Count4U.Model.ServiceContract.DataContract;


namespace Count4U.Model.ServiceContract
{
	[ServiceContract]
	public interface ILogMessage
	{
		//[OperationContract]
		//Sip2Message NextLogInfo(Sip2Message message);

		[OperationContract]
		Process4UBaseData Sip2LogInfo(Process4UBaseData message);

		[OperationContract]
		string Sip2InfoTitle(string message, string title);

		[OperationContract]
		string Sip2LogInfoTime(string message, string time);
		
		[OperationContract]
		Process4UBaseData UpdateListView(Process4UBaseData request, Process4UBaseData response);
	}
}









