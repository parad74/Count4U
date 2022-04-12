using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U
{
	public class LogMessage
	{
		private string _message;
		private MessageTypeEnum _messageType;

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public MessageTypeEnum MessageType
		{
			get { return _messageType; }
			set { _messageType = value; }
		}
	}
}
