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
using Main.Service;
using Microsoft.Practices.ServiceLocation;


namespace Common.Main.Presenter
{
	public class LogMessageServicePresenter : BasePresenter<IMainView>, ILogMessageServicePresenter
	{
		public readonly IMainService _service;
		public readonly IServiceLocator _serviceLocator;

		// Указание, где ожидать входящие сообщения.
		//Uri _addressLogMessage = new Uri(@"net.tcp://localhost:10001/LogMessageService");

		// Указание, как обмениваться сообщениями.
		//NetTcpBinding _bindingLogMessage = new NetTcpBinding();
			
		public LogMessageServicePresenter(IMainService service,
			IServiceLocator serviceLocator)
		{
			this._serviceLocator = serviceLocator;
			this._service = service;
		}

		//==================================== LogMessageService =======================================
		public void OpenService()
		{
			try
			{
				if (base.ServiceHost == null)
				{
					// Создание экзкмпляра ServiceHost.
					base.ServiceHost = new ServiceHost(typeof(LogMessageService));
					//this._bindingLogMessage.ReceiveTimeout = TimeSpan.MaxValue;
					//this._bindingLogMessage.SendTimeout = TimeSpan.MaxValue;
					//this._bindingLogMessage.OpenTimeout = TimeSpan.FromMinutes(2f);
					//this._bindingLogMessage.CloseTimeout = TimeSpan.MaxValue;
					// Добавление "Конечной Точки".
					//base.ServiceHost.AddServiceEndpoint(typeof(ILogMessage), this._bindingLogMessage, this._addressLogMessage);

					// Начало ожидания прихода сообщений.
					base.ServiceHost.Open();
					//SetOutputText(string text);
					//this.View.SetOutputText("Server started: ILogMessage,  " + base.ServiceHost.ToString() + ", " + this._addressLogMessage.ToString() + ", " + DateTime.Now + Environment.NewLine);

					this.SetOutputText("Server started: ILogMessage,  ");
					
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
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
			try
			{
				if (base.ServiceHost != null)
				{
					// Завершение ожидания прихода сообщений.
					this.SetOutputText("Server closed: ILogMessage,  ");
					base.ServiceHost.Close();
					base.ServiceHost = null;
					//this.View.SetOutputText("Server closed: ILogMessage,  " + this._bindingLogMessage.ToString() + ", " + this._addressLogMessage.ToString() + ", " + DateTime.Now + Environment.NewLine);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		
	
	}
}

