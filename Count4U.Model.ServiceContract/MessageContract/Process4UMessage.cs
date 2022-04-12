using System.ServiceModel;
using System.Runtime.Serialization;

// КОНТРАКТЫ.
namespace Count4U.Model.ServiceContract.MessageContract
{
	
	// КОНТРАКТ СООБЩЕНИЯ.
	[MessageContract]
	public class Process4UMessage
	{
		//private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private string _operation;
		private string _message;
		private string _request;
		private string _response;
		private string _result;
		private string _key;
		private string _error;

		public Process4UMessage()
		{
			this._operation = "";
			this._message = "";
			this._request = "";
			this._response = "";
			this._result = "";
			this._key = "";
			this._error = "";
		}

		public Process4UMessage(string operation, string message, string result,
			string request, string response, string key, string error)
		{
			this._operation = operation;
			this._message = message;
			this._request = request;
			this._response = response;
			this._result = result;
			this._key = key;
			this._error = error;
	  }

		public Process4UMessage(Process4UMessage message)
		{
			this._operation = message._operation;
			this._message = message._message;
			this._request = message._request;
			this._response = message._response;
			this._result = message._result;
			this._key = message._key;
			this._error = message._error;
		}

		[MessageHeader]
		public string Operation
		{
			get { return _operation; }
			set { _operation = value; }
		}

		[MessageHeader]
		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		[MessageBodyMember(Order = 1)]
		public string Request
		{
			get { return _request; }
			set { _request = value; }
		}

		[MessageBodyMember(Order = 2)]
		public string Response
		{
			get { return _response; }
			set { _response = value; }
		}

		[MessageBodyMember(Order = 3)]
		public string Result
		{
			get { return _result; }
			set { _result = value; }
		}

		[MessageBodyMember(Order = 4)]
		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		[MessageBodyMember(Order = 5)]
		public string Error
		{
			get { return _error; }
			set { _error = value; }
		}


	}
}











//using System.ServiceModel;

//// КОНТРАКТ.

//namespace Client
//{
//	[ServiceContract]
//	interface IContract
//	{
//		[OperationContract]
//		string Say(string input);
//	}
//}
