using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Presenter.Interface;

namespace Common.Main.Interface
{
	public interface IRequest2ResponseProxyServicePresenter : IPresenter<IMainView>
	{
		void OpenRequest2ResponseProxyServiceHost();
		void CloseRequest2ResponseProxyServiceHost();
	}


}

