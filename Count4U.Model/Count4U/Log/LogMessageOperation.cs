using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U
{
	public static class LogMessageOperation
	{
		//private string _message;
		//private MessageTypeEnum _messageType;

		public static bool In(this MessageTypeEnum messageType, List<MessageTypeEnum> includeMessageType)
		{
			if (includeMessageType.Contains(messageType) == true) return true; 
			return false;
		}
	}
}
