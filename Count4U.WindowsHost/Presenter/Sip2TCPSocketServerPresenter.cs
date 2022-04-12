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
	// I_Sip2TCPSocketServer вызывает II_Request2ResponseClient
	// это хостинг сервера Sip2TCPSocket на Windows Form (есть аналогичная точка входа на WindowsService)
	// I_Sip2TCPSocketServer слушает Sip2(библиотека независимая от хостинга внешняя) -> который вызывает II_Request2ResponseClient
	public class Sip2TCPSocketServerPresenter : BasePresenter<IMainView>, ISip2TCPSocketServerPresenter
	{
		public readonly IMainService _service;
		public readonly IServiceLocator _serviceLocator;
		public readonly string HostingTCPSocketServer = "WinForm";

		private Sip2TCPSocketServer _sip2TCPSocketServer = null;
		private System.Timers.Timer _timer;
		
	
		public Sip2TCPSocketServerPresenter(IMainService service,
			IServiceLocator serviceLocator)
		{
			this._serviceLocator = serviceLocator;
			this._service = service;
		}


		//===================== Sip2TCPSocketServer =============================================
		public void InitSip2TCPSocketServer()
		{
			// это хостинг сервера Sip2TCPSocket на Windows Form (есть аналогичная точка входа на WindowsService)
			// Sip2 -> слушает Sip2TCPSocketServer (библиотека независимая от хостинга) -> который вызывает II_Request2ResponseClient
			this._sip2TCPSocketServer = new Sip2TCPSocketServer(12345, HostingTCPSocketServer, Program.ProgramArgs);

			this._timer = new System.Timers.Timer();
			this._timer.Enabled = true;
			//Интервал 10000мс - 10с.
			this._timer.Interval = 2000;
			this._timer.Elapsed += new System.Timers.ElapsedEventHandler(StartSip2TCPSocketServer);
			this._timer.AutoReset = true;
			this._timer.Start();
		}

		public void RestartTimerSip2TCPSocketServer()
		{
			this._timer.Start();
		}

		private void StartSip2TCPSocketServer(object source, ElapsedEventArgs e)
		{

			if (this._sip2TCPSocketServer != null)
			{
				try
				{
					//UiConnection.UpdateText("SocketHostStarting        " + DateTime.Now + Environment.NewLine);

					this._timer.Stop();
					this._sip2TCPSocketServer.Start();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			else
			{
				//UiConnection.UpdateText( "socketServer == null" +  Environment.NewLine);
			}
		}

		public void CloseSip2TCPSocketServer()
		{
			this._timer.Stop();
			if (this._sip2TCPSocketServer != null)
			{
				this._sip2TCPSocketServer.Close();
				string infoABC = "Server closed: Sip2TCPSocketServer,  address 12345 " + DateTime.Now + Environment.NewLine;
				this.View.SetOutputText(infoABC);
				this.View.SetRunOptionsText(infoABC);


			}
		}

		
	}
}

