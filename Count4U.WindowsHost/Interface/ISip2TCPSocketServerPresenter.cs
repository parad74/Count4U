using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Presenter.Interface;

namespace Common.Main.Interface
{
	public interface ISip2TCPSocketServerPresenter : IPresenter<IMainView>
	{
		void CloseSip2TCPSocketServer();
		void InitSip2TCPSocketServer();
		void RestartTimerSip2TCPSocketServer();
	}


}

