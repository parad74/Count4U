using System;
using System.ServiceModel;
using System.Collections.Generic;
using NLog;
//using Sip2.TCPClient.Emulator.Interface;
//using Sip2.TCPSocketClient.Emulator;
using IdSip2.ServiceContract;
using IdSip2.MessageContract;

// СЕРВИС.

namespace IdSip2.ServiceImplementation
{
	
	public class SimpleMessageService : ISimpleMessage
    {
		//private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		//ISip2 _sip2SocketClient = new Sip2SocketClient(); ???

		public Sip2Message Next(Sip2Message message)
		{
			Sip2Message  newSip2Message = new Sip2Message();
			newSip2Message.Result = message.Message + " + !SimpleMessageService! +";
			return newSip2Message;
		}

		public Sip2Message GetSip2(Sip2Message message)
		{
			//Sip2Message newSip2Message = new Sip2Message(message);

			TestResponse test = new TestResponse();
			Sip2Message newSip2Message = test.Response(message);

			//newSip2Message.Response = @"98YYYYYN15001020140407    0009332.00AOALEPH|AMBeit Ariela Public Library|BXYYYYYYYYYYYNNYYY|AN217.194.202.116|AFACS status.|AGACS status.|";
			//newSip2Message.Result = message.Message + " + =SimpleMessageService= +"; //this._sip2SocketClient.Start(); ???
			//this._sip2SocketClient.Stop();
			return newSip2Message;
		}

	}
}


//public Sip2Message Calculate(Sip2Message request)
//		{
//			Sip2Message response = new Sip2Message(request);

//			switch (request.Operation)
//			{
//				case "+":
//					response.Result = request.N1 + request.N2;
//					break;
//				case "-":
//					response.Result = request.N1 - request.N2;
//					break;
//				case "*":
//					response.Result = request.N1 * request.N2;
//					break;
//				case "/":
//					response.Result = request.N1 / request.N2;
//					break;
//				default:
//					response.Result = 0.0D;
//					break;
//			}
//			return response;
//		}