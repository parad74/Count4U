using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using Common.Presenter.Interface;
using Common.WCF;
using IdSip2.MessageContract;
using IdSip2.Service;
using IdSip2.ServiceContract;
using Main.Interface;
using Microsoft.Practices.ServiceLocation;
using Sip2.TCPSocketServer;


namespace Common.Main.Presenter
{
	public class Request2ObjectServicePresenter : BasePresenter<IMainView>, IRequest2ObjectServicePresenter
	{
		public readonly IMainService _service;
		public readonly IServiceLocator _serviceLocator;

		// Указание, где ожидать входящие сообщения.
		Uri _addressRequest2Object = new Uri(@"net.tcp://localhost:10274/Request2ObjectService");

		// Указание, как обмениваться сообщениями.
		NetTcpBinding _bindingRequest2Object = new NetTcpBinding();

		// Ссылка на экзкмпляр ServiceHost.
		ServiceHost _request2ObjectServiceHost;

		public Request2ObjectServicePresenter(IMainService service,
			IServiceLocator serviceLocator)
		{
			this._serviceLocator = serviceLocator;
			this._service = service;
		}
		
		//==================================== Request2ObjectService =======================================
		public void OpenRequest2ObjectServiceHost()
		{
			try
			{
				if (this._request2ObjectServiceHost == null)
				{
					// Создание экзкмпляра ServiceHost.
					this._request2ObjectServiceHost = new ServiceHost(typeof(Request2ObjectService));

					this._bindingRequest2Object.ReceiveTimeout = TimeSpan.MaxValue;
					this._bindingRequest2Object.SendTimeout = TimeSpan.MaxValue;
					this._bindingRequest2Object.OpenTimeout = TimeSpan.FromMinutes(2f);
					this._bindingRequest2Object.CloseTimeout = TimeSpan.MaxValue;
					   
					// Добавление "Конечной Точки".
					this._request2ObjectServiceHost.AddServiceEndpoint(typeof(IRequest2Object), this._bindingRequest2Object, this._addressRequest2Object);

					// Начало ожидания прихода сообщений.
					this._request2ObjectServiceHost.Open();
					//SetOutputText(string text);
					this.View.SetOutputText("Server started: IRequest2Object,  " + this._bindingRequest2Object.ToString() + ", " + this._addressRequest2Object.ToString() + ", " + DateTime.Now + Environment.NewLine);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void CloseRequest2ObjectServiceHost()
		{
			try
			{
				if (this._request2ObjectServiceHost != null)
				{
					// Завершение ожидания прихода сообщений.
					this._request2ObjectServiceHost.Close();
					this._request2ObjectServiceHost = null;
					this.View.SetOutputText("Server closed: IRequest2Object,  " + _bindingRequest2Object.ToString() + ", " + _addressRequest2Object.ToString() + ", " + DateTime.Now + Environment.NewLine);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		
	
	}
}

