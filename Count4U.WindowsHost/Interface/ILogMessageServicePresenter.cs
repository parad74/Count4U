using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Presenter.Interface;

namespace Common.Main.Interface
{
	
	public interface ILogMessageServicePresenter : IPresenter<IMainView>
	{
		void OpenService();
		void CloseService();
	}


}

