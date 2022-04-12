using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using Common.Presenter.Interface;
using Common.WCF;
using IdSip2.MessageContract;
using IdSip2.Service;
using IdSip2.ServiceContract;
using IdSip2.ServiceImplementation;
using Main.Interface;
using Microsoft.Practices.ServiceLocation;
using Sip2.TCPSocketServer;


namespace Common.Main.Presenter
{
	public class Request2ResponseProxyServicePresenter : BasePresenter<IMainView>, IRequest2ResponseProxyServicePresenter
	{
		public readonly IMainService _service;
		public readonly IServiceLocator _serviceLocator;

		// Указание, где ожидать входящие сообщения.
		//Uri _address2ResponseObject = new Uri(@"net.tcp://localhost:10074/Request2ResponseService");
	
		// Указание, как обмениваться сообщениями.
		//NetTcpBinding _bindingRequest2Response = new NetTcpBinding();

		// Ссылка на экзкмпляр ServiceHost.
		ServiceHost _topsysBibliothecaProxyServiceHost;

		public Request2ResponseProxyServicePresenter(IMainService service,
			IServiceLocator serviceLocator)
		{
			this._serviceLocator = serviceLocator;
			this._service = service;
		}



		//==================================== Request2ResponseProxyService =======================================
		public void OpenRequest2ResponseProxyServiceHost()
		{
			try
			{
				if (this._topsysBibliothecaProxyServiceHost == null)
				{
					// Создание экзкмпляра ServiceHost.
					this._topsysBibliothecaProxyServiceHost = new ServiceHost(typeof(TopsysBibliothecaService));
					// Начало ожидания прихода сообщений.
					this._topsysBibliothecaProxyServiceHost.Open();
					this.SetOutputText("Server started: IRequest2ResponseProxy,  ");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void CloseRequest2ResponseProxyServiceHost()
		{
			try
			{
				if (this._topsysBibliothecaProxyServiceHost != null)
				{
					// Завершение ожидания прихода сообщений.
					this.SetOutputText("Server closed: IRequest2ResponseProxy,  ");
					this._topsysBibliothecaProxyServiceHost.Close();
					this._topsysBibliothecaProxyServiceHost = null;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void SetOutputText(string operation)
		{
			ServiceDescription svcDesc = this._topsysBibliothecaProxyServiceHost.Description;
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

	}
}

