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
using Microsoft.Practices.ServiceLocation;

namespace Common.Main.Presenter
{
	public class Request2ResponseServicePresenter : BasePresenter<IMainView>, IRequest2ResponseServicePresenter
	{
		public readonly IMainService _service;
		public readonly IServiceLocator _serviceLocator;

		// Указание, где ожидать входящие сообщения.
		//Uri _address2ResponseObject = new Uri(@"net.tcp://localhost:10074/Request2ResponseService");
	
		// Указание, как обмениваться сообщениями.
		//NetTcpBinding _bindingRequest2Response = new NetTcpBinding();

		// Ссылка на экзкмпляр ServiceHost.
		ServiceHost _request2ResponseServiceHost;

		
		public Request2ResponseServicePresenter(IMainService service,
			IServiceLocator serviceLocator)
		{
			this._serviceLocator = serviceLocator;
			this._service = service;
		}
		
		//==================================== Request2ObjectService =======================================
		public void OpenRequest2ResponseServiceHost()
		{
			try
			{
				if (this._request2ResponseServiceHost == null)
				{
					// Создание экзкмпляра ServiceHost.
					//this._request2ResponseServiceHost = new ServiceHost(typeof(Request2ResponseService));

					//this._bindingRequest2Response.ReceiveTimeout = TimeSpan.MaxValue;
					//this._bindingRequest2Response.SendTimeout = TimeSpan.MaxValue;
					//this._bindingRequest2Response.OpenTimeout = TimeSpan.FromMinutes(2f);
					//this._bindingRequest2Response.CloseTimeout = TimeSpan.MaxValue;

					// Добавление "Конечной Точки".
					//this._request2ResponseServiceHost.AddServiceEndpoint(typeof(IRequest2Response), this._bindingRequest2Response, this._address2ResponseObject);

					// Начало ожидания прихода сообщений.
					this._request2ResponseServiceHost.Open();
					//SetOutputText(string text);
					//this.View.SetOutputText("Server started: IRequest2Response,  " + this._bindingRequest2Response.ToString() + ", " + this._address2ResponseObject.ToString() + ", " + DateTime.Now + Environment.NewLine);
					this.SetOutputText("Server started: IRequest2Response,  ");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void CloseRequest2ResponseServiceHost()
		{
			try
			{
				if (this._request2ResponseServiceHost != null)
				{
					// Завершение ожидания прихода сообщений.
					this.SetOutputText("Server closed: IRequest2Response,  ");
					this._request2ResponseServiceHost.Close();
					this._request2ResponseServiceHost = null;
					//this.View.SetOutputText("Server closed: IRequest2Response,  " + this._bindingRequest2Response.ToString() + ", " + this._address2ResponseObject.ToString() + ", " + DateTime.Now + Environment.NewLine);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		

		private void SetOutputText(string operation)
		{
			ServiceDescription svcDesc = this._request2ResponseServiceHost.Description;
			ServiceEndpointCollection serviceEndpoints = svcDesc.Endpoints;
			string name = "";
			string address = "";
			if (serviceEndpoints.Count > 0)
			{
				name = serviceEndpoints[0].Name;
				address = serviceEndpoints[0].ListenUri.ToString();
			}

			string infoABC = operation + name + ", " + address + ", " + DateTime.Now + Environment.NewLine ;
			this.View.SetOutputText(infoABC);
			this.View.SetRunOptionsText(infoABC);

		}

	}
}

