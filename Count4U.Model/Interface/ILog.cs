using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface
{
	public interface ILog
	{
		//void Add(string message);
		void Add(MessageTypeEnum messageType, string message);
		void AddSimple(MessageTypeEnum messageType, string message);
		void Clear();
		LogMessages Messages();
		//List<string> Message();
		//string PrintLog(List<MessageTypeEnum> includeMessageType  = null, int countMessage = 200);
		string PrintLog(ImportExportLoglevel importExportLogLevel = ImportExportLoglevel.Simple, int countMessage = 500, Encoding encoding = null);
		//string FileLog(List<MessageTypeEnum> includeMessageType = null);
		void SetIncludeMessageType(List<MessageTypeEnum> includeMessageType);
        string FileLog(Encoding encoding = null);
	}
}
