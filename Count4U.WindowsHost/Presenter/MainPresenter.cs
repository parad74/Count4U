using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using Common.Main.Interface;
using Common.Presenter.Interface;
using Common.WCF;
using Microsoft.Practices.ServiceLocation;


namespace Common.Main.Presenter
{
	public class MainPresenter : BasePresenter<IMainView>, IMainPresenter
	{
		public readonly IMainService _service;
		public readonly IServiceLocator _serviceLocator;

		//private Sip2TCPSocketServer _sip2TCPSocketServer = null;
		//private System.Timers.Timer _timer;

		//// Указание, где ожидать входящие сообщения.
		//Uri _addressRequest2Object = new Uri(@"net.tcp://localhost:10274/Request2ObjectService");

		//// Указание, как обмениваться сообщениями.
		//NetTcpBinding _bindingRequest2Object = new NetTcpBinding();

		//// Ссылка на экзкмпляр ServiceHost.
		//ServiceHost _request2ObjectServiceHost;

		//Uri _addressSimpleMessage = new Uri("net.tcp://localhost:12345/SimpleMessageService");
		//NetTcpBinding _bindingSimpleMessage = new NetTcpBinding();
	
		public MainPresenter(IMainService service,
			IServiceLocator serviceLocator)
		{
			this._serviceLocator = serviceLocator;
			this._service = service;
		}


		////===================== Sip2TCPSocketServer =============================================
		//public void InitSip2TCPSocketServer()
		//{
		//	this._sip2TCPSocketServer = new Sip2TCPSocketServer(12345);

		//	this._timer = new System.Timers.Timer();
		//	this._timer.Enabled = true;
		//	//Интервал 10000мс - 10с.
		//	this._timer.Interval = 2000;
		//	this._timer.Elapsed += new System.Timers.ElapsedEventHandler(StartSip2TCPSocketServer);
		//	this._timer.AutoReset = true;
		//	this._timer.Start();
		//}

		//public void RestartTimerSip2TCPSocketServer()
		//{
		//	this._timer.Start();
		//}

		//private void StartSip2TCPSocketServer(object source, ElapsedEventArgs e)
		//{

		//	if (this._sip2TCPSocketServer != null)
		//	{
		//		try
		//		{
		//			//UiConnection.UpdateText("SocketHostStarting        " + DateTime.Now + Environment.NewLine);

		//			this._timer.Stop();
		//			this._sip2TCPSocketServer.Start();
		//		}
		//		catch (Exception ex)
		//		{
		//			MessageBox.Show(ex.Message);
		//		}
		//	}
		//	else
		//	{
		//		//UiConnection.UpdateText( "socketServer == null" +  Environment.NewLine);
		//	}
		//}

		//public void CloseSip2TCPSocketServer()
		//{
		//	this._timer.Stop();
		//	if (this._sip2TCPSocketServer != null)
		//	{
		//		this._sip2TCPSocketServer.Close();
		//		this.View.SetOutputText("Server closed: Sip2TCPSocketServer,  address 12345 " + DateTime.Now + Environment.NewLine);

		//	}
		//}

		////==================================== Request2ObjectService =======================================
		//public void OpenRequest2ObjectServiceHost()
		//{
		//	try
		//	{
		//		if (this._request2ObjectServiceHost == null)
		//		{
		//			// Создание экзкмпляра ServiceHost.
		//			this._request2ObjectServiceHost = new ServiceHost(typeof(Request2ObjectService));

		//			this._bindingRequest2Object.ReceiveTimeout = TimeSpan.MaxValue;
		//			this._bindingRequest2Object.SendTimeout = TimeSpan.MaxValue;
		//			// Добавление "Конечной Точки".
		//			this._request2ObjectServiceHost.AddServiceEndpoint(typeof(IRequest2Object), _bindingRequest2Object, _addressRequest2Object);

		//			// Начало ожидания прихода сообщений.
		//			this._request2ObjectServiceHost.Open();
		//			//SetOutputText(string text);
		//			this.View.SetOutputText("Server started: Contract IRequest2Object,  binding " + _bindingRequest2Object.ToString() + ", address " + _addressRequest2Object.ToString() + "    " + DateTime.Now + Environment.NewLine);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		MessageBox.Show(ex.Message);
		//	}
		//}

		//public void CloseRequest2ObjectServiceHost()
		//{
		//	try
		//	{
		//		if (this._request2ObjectServiceHost != null)
		//		{
		//			// Завершение ожидания прихода сообщений.
		//			this._request2ObjectServiceHost.Close();
		//			this._request2ObjectServiceHost = null;
		//			this.View.SetOutputText("Server closed: Contract IRequest2Object,  binding " + _bindingRequest2Object.ToString() + ", address " + _addressRequest2Object.ToString() + "    " + DateTime.Now + Environment.NewLine);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		MessageBox.Show(ex.Message);
		//	}
		//}

		//=============== test
		//public void TestExecute()
		//{
		////	IImportProvider provider = _serviceLocator.GetInstance<IImportProvider>();
		////	provider.ToPathDB = this.View.GetDbPathText();
		////	provider.Parms.Clear();
		////	//provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
		////	provider.ProviderEncoding = Encoding.GetEncoding(1255);
		////	//provider.Clear(this.View.GetDbPathText());

		////	string folder = this.View.GetFolderCSVText();
		////	string[] files = Directory.GetFiles(this.View.GetFolderCSVText());
		////	foreach(var file in files){
		////		string finalPath = System.IO.Path.Combine(folder, file);
		////		provider.FromPathFile = finalPath;
		////		provider.Import();
		////	}
		////	MessageBox.Show("All  Import");	
		////	//--------------------------------------
		////	//this.View.SetOutputText(_service.Test(this.View.GetInputText()));
		////}

		////public void TestGetCoilEFRepository()
		////{
		////	ICoilRepository coilRepository = _serviceLocator.GetInstance<ICoilRepository>();
		////	string toPathDB = this.View.GetDbPathText();
		////	Coils coils = coilRepository.GetCoils(toPathDB);
		////	this.View.DataGridView.DataSource = coils;
		////}

		////public void TestClear()
		////{
		////	IImportProvider importCoilProvider = _serviceLocator.GetInstance<IImportProvider>();
		////	ICoilRepository coilRepository = _serviceLocator.GetInstance<ICoilRepository>();

		////	//-----------------------------------
		////	//Coils coils = coilRepository.GetCoils();

		////	//------------------------------------
		////	importCoilProvider.Clear(this.View.GetDbPathText());
		////	MessageBox.Show("All  Clear");
			

		////	//--------------------------------------
		////	//this.View.SetOutputText(_service.Test(this.View.GetInputText()));
		//}

		//public void TestClear()
		//{
		//}

		//// тестирование клиента Sip2IIS
		//public Sip2Message Test1Execute(string inputString)
		//{
		//	//var response = factoryWrapper.Execute(proxy =>
		//	//			{
		//	//				proxy.Next(request);
		//	//				return proxy.Next1(request);
		//	//			});
		//	Sip2Message request = new Sip2Message();
		//	try
		//	{
		//		var factoryWrapper = new FactoryWrapper<ISimpleMessage>(this._bindingSimpleMessage, new EndpointAddress(this._addressSimpleMessage));

		//		request.Message = inputString;
		//		var response = factoryWrapper.Execute(proxy => proxy.Next(request));

		//		return response;

		//		//работает
		//		//if (factory == null)
		//		//{
		//		//	factory = new ChannelFactory<ISimpleMessage>(binding, new EndpointAddress(address));
		//		//	channel = factory.CreateChannel();
		//		//}

		//		//if (factory != null && channel != null)
		//		//{
		//		//	Sip2Message request = new Sip2Message();
		//		//	request.Message ="test";
		//		//	Sip2Message response = channel.Next(request);
		//		//	textBox1.Text += string.Format("Sip2Message on client - {0}",   response.Result) + Environment.NewLine;

		//		//}
		//	}
		//	catch (Exception ex)
		//	{
		//		MessageBox.Show(ex.Message, "CLIENT");
		//		return request;
		//	}
		//}

		////тестирование клиента sip2IIS с возвратом сообщения
		//public Sip2Message Test2Execute()
		//{
		//	Sip2Message request = new Sip2Message();
		//	try
		//	{
		//		var factoryWrapper = new FactoryWrapper<ISimpleMessage>(this._bindingSimpleMessage, new EndpointAddress(this._addressSimpleMessage));

		//		request.Message = "test";
		//		var response = factoryWrapper.Execute(proxy => proxy.GetSip2(request));

		//		return response;

		//	}
		//	catch (Exception ex)
		//	{
		//		MessageBox.Show(ex.Message, "CLIENT");
		//		return request;
		//	}
		//}
	
	}
}

