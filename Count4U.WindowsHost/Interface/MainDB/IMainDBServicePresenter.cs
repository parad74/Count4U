using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Presenter.Interface;

namespace Common.Main.Interface
{
	public interface IMainDBServicePresenter : IPresenter<IMainView>
	{
		void OpenService();
		void CloseService();
	}


}

