using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Presenter.Interface;

namespace Common.Main.Interface
{
	public interface IRequest2ObjectServicePresenter : IPresenter<IMainView>
	{
		void OpenRequest2ObjectServiceHost();
		void CloseRequest2ObjectServiceHost();
	}


}

