using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Presenter.Interface;

namespace Common.Main.Interface
{
	public interface IRequest2ResponseServicePresenter : IPresenter<IMainView>
	{
		void OpenRequest2ResponseServiceHost();
		void CloseRequest2ResponseServiceHost();
	}


}

