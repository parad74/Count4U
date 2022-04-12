using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using Common.Main.Interface;
using Common.Presenter.Interface;
using Common.WCF;
using Count4U.Model.Service;
using Count4U.Model.ServiceContract;
using Main.Service;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;


namespace Common.Presenter.MainDB
{
	public class MainDBServicePresenter : BasePresenter<IMainView>, IMainDBServicePresenter
	{
		public readonly IMainService _service;
		public readonly IServiceLocator _serviceLocator;
		public readonly IUnityContainer _container;

		// Указание, где ожидать входящие сообщения.
		//Uri _addressLogMessage = new Uri(@"net.tcp://localhost:10001/LogMessageService");

		// Указание, как обмениваться сообщениями.
		//NetTcpBinding _bindingLogMessage = new NetTcpBinding();

		// Ссылка на экзкмпляр ServiceHost.
		//ServiceHost _logMessageHost;

		public MainDBServicePresenter(IMainService service,
			IServiceLocator serviceLocator,
			 IUnityContainer container)
		{
			this._serviceLocator = serviceLocator;
			this._service = service;
			this._container = container;
		}

		//====================================  MainDBService =======================================
		public void OpenService()
		{
			List<ServiceWpfInfo> serviceWpfInfoList = ServiceWpfRepository.GetServiceWpfInfoList(this._container);

			//Init.Common.ServerIP = Dns.GetHostName();	
			//Init.Common.ServerTcpPort = "2744";
			//Init.Common.ServerHttpPort = "2742";
			foreach(ServiceWpfInfo info in serviceWpfInfoList)
			{
				Tuple<string, string> urls = ServiceWpfRepository.StartTcpHost(info,
					serverIP: Init.Common.ServerIP,
					serverPort: Init.Common.ServerTcpPort,
					serverPortMex: Init.Common.ServerTcpPortMex);

				string infoABC = Environment.NewLine + "Address: " + urls.Item1 + Environment.NewLine +
					"AddressMex: " + urls.Item2 + Environment.NewLine +
					"Protocol: TCP" + Environment.NewLine +
					"Contract:" + info.InterfaceName + Environment.NewLine	 ;
				
				this.View.SetUrlText(urls.Item1)  ;
				this.View.SetUrlMexText(urls.Item2);

				this.View.SetOutputText(infoABC);
				this.View.SetRunOptionsText(infoABC);
			}

			//try
			//{
			//	if (base.ServiceHost == null)
			//	{
			//		// Создание экзкмпляра ServiceHost.
			//		base.ServiceHost = new ServiceHost(typeof(LogMessageService));
			//		//this._bindingLogMessage.ReceiveTimeout = TimeSpan.MaxValue;
			//		//this._bindingLogMessage.SendTimeout = TimeSpan.MaxValue;
			//		//this._bindingLogMessage.OpenTimeout = TimeSpan.FromMinutes(2f);
			//		//this._bindingLogMessage.CloseTimeout = TimeSpan.MaxValue;
			//		// Добавление "Конечной Точки".
			//		//base.ServiceHost.AddServiceEndpoint(typeof(ILogMessage), this._bindingLogMessage, this._addressLogMessage);

			//		// Начало ожидания прихода сообщений.
			//		base.ServiceHost.Open();
			//		//SetOutputText(string text);
			//		//this.View.SetOutputText("Server started: ILogMessage,  " + base.ServiceHost.ToString() + ", " + this._addressLogMessage.ToString() + ", " + DateTime.Now + Environment.NewLine);

			//		this.SetOutputText("Server started: ILogMessage,  ");
					
			//	}
			//}
			//catch (Exception ex)
			//{
			//	MessageBox.Show(exMain.Message);
			//}
		}

		private void SetOutputText(string operation)
		{
			ServiceDescription svcDesc = base.ServiceHost.Description;
			ServiceEndpointCollection serviceEndpoints = svcDesc.Endpoints;
			string name = "";
			string address = "";
			if (serviceEndpoints.Count > 0)
			{
				name = serviceEndpoints[0].Name;
				address = serviceEndpoints[0].ListenUri.ToString();
			}

			string infoABC = operation + name + ", " + address + ", " + DateTime.Now + Environment.NewLine; 
			this.View.SetOutputText(infoABC);
			this.View.SetRunOptionsText(infoABC);
	
		}

		public void CloseService()
		{
			//try
			//{
			//	if (base.ServiceHost != null)
			//	{
			//		// Завершение ожидания прихода сообщений.
			//		this.SetOutputText("Server closed: ILogMessage,  ");
			//		base.ServiceHost.Close();
			//		base.ServiceHost = null;
			//		//this.View.SetOutputText("Server closed: ILogMessage,  " + this._bindingLogMessage.ToString() + ", " + this._addressLogMessage.ToString() + ", " + DateTime.Now + Environment.NewLine);
			//	}
			//}
			//catch (Exception ex)
			//{
			//	MessageBox.Show(ex.Message);
			//}
		}

		
	
	}
}

