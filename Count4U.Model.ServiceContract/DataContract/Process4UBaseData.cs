using System.ServiceModel;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace  Count4U.Model.ServiceContract.DataContract
{
	[KnownType("GetKnownTypes")]
	public class Process4UBaseData
	{

		[Key]
		public long ID { get; set; }
		public DateTime CreateDateTime { get; set; }
		private string _operation;
		private string _messageName;
		private string _request;
		private string _response;
		private string _result;
		private string _operationType;
		private string _error;
		public string XmlMessage { get; set; }

		public Process4UBaseData()
		{
			//this.ID = ++ ConvertData.ID;
			this.CreateDateTime = DateTime.Now;
			this._operation = "";
			this._messageName = "";
			this._request = "";
			this._response = "";
			this._result = "";
			this._operationType = "";
			this._error = ""   ;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Process4UBaseData)) return false;
			return Equals((Process4UBaseData)obj);
		}

		public bool Equals(Process4UBaseData other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ID, this.ID);
		}

		public override int GetHashCode()
		{
			return (this.ID != null ? this.ID.GetHashCode() : 0);
		}

		public Process4UBaseData(string operation, string messageName, string result,
			string request, string response, string key, string error)
		{
			//this.ID = ++ConvertData.ID;
			this.CreateDateTime = DateTime.Now;
			this._operation = operation;
			this._messageName = messageName;
			this._request = request;
			this._response = response;
			this._result = result;
			this._operationType = key;
			this._error = error;
		}

		public Process4UBaseData(Process4UBaseData message)
		{
			//this.ID = ++ConvertData.ID;
			this.CreateDateTime = DateTime.Now;
			this._operation = message._operation;
			this._messageName = message._messageName;
			this._request = message._request;
			this._response = message._response;
			this._result = message._result;
			this._operationType = message._operationType;
			this._error = message._error;
		}

		static Type[] GetKnownTypes()
		{
			return new Type[] {
					//typeof(Request09) ,
					//typeof(Request11) ,
					//typeof(Request17) ,
					//typeof(Request29) ,
					//typeof(Request35) ,
					//typeof(Request37) ,
					//typeof(Request63) ,
					//typeof(Request93) ,
					//typeof(Request99) ,
					//typeof(Response10) ,
					//typeof(Response12) ,
					//typeof(Response18) ,
					//typeof(Response30) ,
					//typeof(Response36) ,
					//typeof(Response38) ,
					//typeof(Response64) ,
					//typeof(Response94) ,
					//typeof(Response98) ,
					//typeof(Request99)
			};
		}

		[DataMember(Name = "Operation", Order = 1, IsRequired = true)]
		public string Operation
		{
			get { return this._operation; }
			set { this._operation = value; }
		}

		[DataMember(Name = "MessageName", Order = 2, IsRequired = true)]
		public string MessageName
		{
			get { return this._messageName; }
			set { this._messageName = value; }
		}

		[DataMember(Name = "Request", Order = 3, IsRequired = true)]
		public string Request
		{
			get { return this._request; }
			set { this._request = value; }
		}

		[DataMember(Name = "Response", Order = 4, IsRequired = true)]
		public string Response
		{
			get { return this._response; }
			set { this._response = value; }
		}

		[DataMember(Name = "Result", Order = 5, IsRequired = true)]
		public string Result
		{
			get { return this._result; }
			set { this._result = value; }
		}

		[DataMember(Name = "OperationType", Order = 6, IsRequired = true)]
		public string OperationType
		{
			get { return this._operationType; }
			set { this._operationType = value; }
		}

		[DataMember(Name = "Error", Order = 7, IsRequired = true)]
		public string Error
		{
			get { return this._error; }
			set { this._error = value; }
		}

	
	}


}
