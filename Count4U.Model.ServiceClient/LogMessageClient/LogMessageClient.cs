using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Common.WCF;
using Count4U.Model.ServiceContract;
using Count4U.Model.ServiceContract.DataContract;


namespace Count4U.Model.ServiceClient
{
	//III_LogMessageClient вызывается  из I_Sip2TCPSocketServer
	public class LogMessageClient
	{
		// Указание, где ожидать входящие сообщения.
		//Uri _addressLogMessage = new Uri(@"net.tcp://localhost:10001/LogMessageService");

		// Указание, как обмениваться сообщениями.
		//NetTcpBinding _bindingLogMessage = new NetTcpBinding();

		private string _abcName = "tcpILogMessage";
		public LogMessageClient(string abcName = "")
		{
			if (string.IsNullOrWhiteSpace(abcName) == false)
				this._abcName = abcName;
		}


		public Process4UBaseData LogDataSend(Process4UBaseData request)
		{
			//var response = factoryWrapper.Execute(proxy =>
			//			{
			//				proxy.Next(request);
			//				return proxy.Next1(request);
			//			});
			//Sip2Message request = new Sip2Message();

			try
			{
				//var factoryWrapper = new FactoryWrapper<ILogMessage>(this._bindingLogMessage, new EndpointAddress(this._addressLogMessage));
				var factoryWrapper = new FactoryWrapper<ILogMessage>(_abcName);
				var response = factoryWrapper.Execute(proxy => proxy.Sip2LogInfo(request));
				return response;
			}
			catch (Exception ex)
			{
				//	MessageBox.Show(ex.Message, "CLIENT");
				request.Result = "LogMessageClient Error:" + ex.Message;
				return request;

			}
		}

		public Process4UBaseData UpdateListView(Process4UBaseData request, Process4UBaseData response)
		{
			try
			{
				var factoryWrapper = new FactoryWrapper<ILogMessage>(_abcName);	
				var ret = factoryWrapper.Execute(proxy => proxy.UpdateListView(request, response));
				return ret;
			}
			catch (Exception ex)
			{
				request.Result = "LogMessageClient Error:" + ex.Message;
				return request;

			}
		}

		public string LogMessageTitleSend(string message, string title)
		{
			try
			{
				var factoryWrapper = new FactoryWrapper<ILogMessage>(_abcName);
				var ret = factoryWrapper.Execute(proxy => proxy.Sip2InfoTitle(message, title));
				return ret;
			}
			catch (Exception ex)
			{
				return "LogMessageClient Error:" + ex.Message;
			}
		}


		public string LogMessageTimeSend(string message, string time)
		{
			try
			{
				var factoryWrapper = new FactoryWrapper<ILogMessage>(_abcName);
				var ret = factoryWrapper.Execute(proxy => proxy.Sip2LogInfoTime(message, time));
				return ret;
			}
			catch (Exception ex)
			{
				return "LogMessageClient Error:" + ex.Message;
			}
		}

	
	}
}

