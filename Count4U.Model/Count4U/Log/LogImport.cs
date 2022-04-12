using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using System.IO;

namespace Count4U.Model.Count4U
{
	public class LogImport : ILog
	{
		private List<string> _message;
		private LogMessages _logMessage;
		private List<MessageTypeEnum> _includeMessageType;

		public LogImport()
		{
			this._message = new List<string>();
			this._logMessage = new LogMessages();
			this._includeMessageType = new List<MessageTypeEnum>();
			this._includeMessageType.Add(MessageTypeEnum.Error);
			this._includeMessageType.Add(MessageTypeEnum.ErrorDB);
			this._includeMessageType.Add(MessageTypeEnum.Fatal);
			this._includeMessageType.Add(MessageTypeEnum.Trace);
			this._includeMessageType.Add(MessageTypeEnum.TraceParser);
			this._includeMessageType.Add(MessageTypeEnum.TraceParserResult);
			this._includeMessageType.Add(MessageTypeEnum.TraceProvider);
			this._includeMessageType.Add(MessageTypeEnum.TraceProviderResult);
			this._includeMessageType.Add(MessageTypeEnum.TraceRepository);
			this._includeMessageType.Add(MessageTypeEnum.TraceRepositoryResult);
			this._includeMessageType.Add(MessageTypeEnum.Warning);
			this._includeMessageType.Add(MessageTypeEnum.WarningParser);
			this._includeMessageType.Add(MessageTypeEnum.WarningRepository);
			this._includeMessageType.Add(MessageTypeEnum.SimpleTrace);
		}	

		private List<MessageTypeEnum> IncludeMessageType
		{
			get { return _includeMessageType; }
		}

		public void SetIncludeMessageType(List<MessageTypeEnum> includeMessageType)
		{
			this._includeMessageType = includeMessageType;
		}


		//public void Add(string message)
		//{
		//    this._message.Add("< " + DateTime.Now.ToString() + " >" + " - " + message);
		//}

		public void Clear()
		{
			//this._message.Clear();
			//this._includeMessageType.Clear();
			this._logMessage.Clear();
		}

		public LogMessages Messages()
		{
			return this._logMessage;
		}

		//public List<string> Message()
		//{
		//    return this._message;
		//}

		//public string PrintLog(int countMessage = 200)
		//{
			//using (MemoryStream ms = new MemoryStream())
			//{
			//    StreamWriter writer = new StreamWriter(ms);
			//    string addMessage = "";
			//    if (this._message.Count < countMessage)
			//    {
			//        countMessage = this._message.Count;
			//    }
			//    else
			//    {
			//        addMessage = "Too many Errors and Warnings in Log: [ " + this._message.Count.ToString()
			//            + "]" + Environment.NewLine;
			//    }
			//    for(int i=0; i < countMessage ; i++)
			//    {
			//        writer.Write(this._message[i] + Environment.NewLine);
			//    }
			//    writer.Write(addMessage);
			//    writer.Flush();
			//    return System.Text.Encoding.Default.GetString(ms.ToArray());
			//}
		//    return "";
		//}

		//public string FileLog()
		//{
			//using (MemoryStream ms = new MemoryStream())
			//{
			//    StreamWriter writer = new StreamWriter(ms);
			//    foreach (var message in this._message)
			//    {
			//        writer.Write(message + Environment.NewLine);
			//    }
			//    writer.Flush();
			//    return System.Text.Encoding.Default.GetString(ms.ToArray());
			//}
		//    return "";
		//}


		public void Add(MessageTypeEnum messageType, string message)
		{
			//this._message.Add("< " + DateTime.Now.ToString() + " >" + " - " + message);
			if (messageType != MessageTypeEnum.SimpleTrace)
			{
				message = "<" + DateTime.Now.ToString("HH:mm:ss") + "> " + messageType + " : " + message;
			}
			else
			{
				message = "<" + DateTime.Now.ToString("HH:mm:ss") + "> : " + message;
			}
			this._logMessage.Add(new LogMessage { MessageType = messageType, Message = message });
		}

		public void AddSimple(MessageTypeEnum messageType, string message)
		{
			//this._message.Add("< " + DateTime.Now.ToString() + " >" + " - " + message);
			message = "<" + DateTime.Now.ToString("HH:mm:ss") + ">  : " + message;
			this._logMessage.Add(new LogMessage { MessageType = messageType, Message = message });
		}

		public string PrintLog(ImportExportLoglevel importExportLogLevel = ImportExportLoglevel.Simple, int countMessage = 500, Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = System.Text.Encoding.Default;
			}
			List<MessageTypeEnum> includeMessageType = new List<MessageTypeEnum>();
			
			foreach (MessageTypeEnum messageType in this._includeMessageType )
			{
				includeMessageType.Add(messageType); 
			}
			includeMessageType.Add(MessageTypeEnum.SimpleTrace);
			includeMessageType.Add(MessageTypeEnum.Error);
			includeMessageType.Add(MessageTypeEnum.ErrorDB);
			includeMessageType.Add(MessageTypeEnum.Fatal);

			List<MessageTypeEnum> finishMessageType = new List<MessageTypeEnum>();
			finishMessageType.Add(MessageTypeEnum.EndTrace);
			
			List<MessageTypeEnum> includeMessageTypeDistinct = includeMessageType.Select(x => x).Distinct().ToList();
			List<MessageTypeEnum> finishMessageTypeDistinct = finishMessageType.Select(x => x).Distinct().ToList(); 
			//LogMessages logMessage = from m in this._logMessage where (m.MessageType.In(this._includeMessageType)) select m;
			var logMessage = this._logMessage.Where(x => x.MessageType.In(includeMessageTypeDistinct) == true).Select(x => x).ToList();
			var logMessageFinish = this._logMessage.Where(x => x.MessageType.In(finishMessageTypeDistinct) == true).Select(x => x).ToList();

			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter writer = new StreamWriter(ms, encoding);
				string addMessage = "";
				if (logMessage.Count < countMessage)
				{
					countMessage = logMessage.Count;
				}
					else
				{
					addMessage = "Too many Errors and Warnings in Log: [ " + logMessage.Count.ToString()
						+ "]" + Environment.NewLine;
				}
				for (int i = 0; i < countMessage; i++)
				{
					writer.Write(logMessage[i].Message + Environment.NewLine);
				}

				writer.Write(addMessage);

				foreach (var msg in logMessageFinish)
				{
					writer.Write(msg.Message + Environment.NewLine);
				}
				
				writer.Flush();
				return encoding.GetString(ms.ToArray());
			}
		}


	     public string FileLog(Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = System.Text.Encoding.Default;
			}
			var logMessage = this._logMessage; //.Where(x => x.MessageType.In(this._includeMessageType) == true).Select(x => x).ToList();

			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter writer = new StreamWriter(ms, encoding);
				foreach (var message in logMessage)
				{
					writer.Write(message.Message + Environment.NewLine);
				}
				writer.Flush();
				return encoding.GetString(ms.ToArray());
			}
		}
	
	}
}
