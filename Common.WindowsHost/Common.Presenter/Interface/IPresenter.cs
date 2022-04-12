using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Common.View.Interface;

namespace Common.Presenter.Interface
{
	
	//Presenter
	public interface IPresenter<T> where T : IView
	{
		T View { get; set; }
		ServiceHost ServiceHost { get; set; }
	}

	//Base Class for Presenter
	public abstract class BasePresenter<T> : IPresenter<T> where T : IView
	{
		public T View { get; set; }
		// Ссылка на экзкмпляр ServiceHost.
		public ServiceHost ServiceHost{ get; set; }
	}
}
