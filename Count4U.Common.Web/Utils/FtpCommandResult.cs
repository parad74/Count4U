using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Count4U.Common.Web
{
	[Serializable]
	public class FtpCommandResult
	{
		public long ID { get; set; }
		public string Error { get; set; }
		public string Message { get; set; }
		public string MessageResponse { get; set; }
		public string MessageResponse1 { get; set; }
		public string MessageCreateFolder { get; set; }
		public string ExecutionTimeString { get; set; }
		public DateTime ExecutionDateTime { get; set; }
		//public CommandErrorCodeEnum ErrorCode { get; set; }
		//public CommandErrorCodeEnum ValidateDataErrorCode { get; set; }
		public SuccessfulEnum Successful { get; set; }                   //использую для синхронизации UI
		public SuccessfulEnum Successful1 { get; set; }                   //использую для синхронизации UI
		//public CommandResultCodeEnum ResultCode { get; set; }
		public override string ToString()
		{
			string ret = "";
			if (string.IsNullOrWhiteSpace(MessageCreateFolder) == false)
				ret = ret + MessageCreateFolder + Environment.NewLine;
			if (string.IsNullOrWhiteSpace(MessageResponse) == false)
				ret = ret + MessageResponse + Environment.NewLine;
			if (string.IsNullOrWhiteSpace(MessageResponse1) == false)
				ret = ret + MessageResponse1 + Environment.NewLine;
			if (string.IsNullOrWhiteSpace(Error) == false)
				ret = ret + Error + Environment.NewLine;
			if (string.IsNullOrWhiteSpace(Message) == false)
				ret = ret + Message + Environment.NewLine;
			return ret;
		}
	}


}
