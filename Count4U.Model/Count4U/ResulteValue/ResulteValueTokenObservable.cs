using System;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace Count4U.Model.Count4U
{
	public class ResulteValueTokenObservable<T> : IObservable<T>
	{
		ILog _log;
		public ResulteValueTokenObservable(ILog log)
		{
			this._log = log;
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			return new Unsubscriber(observer);
		}

		private class Unsubscriber : IDisposable
		{
			private IObserver<T> _observer;

			public Unsubscriber(IObserver<T> observer)
			{
				this._observer = observer;
			}

			public void Dispose()
			{
				//здесь все очищаем
			}
		}

		public static IObservable<T> Empty<T>()
		{
			return Observable.Create<T>(o =>
			{
				o.OnCompleted();
				return Disposable.Empty;
			});
		}
		public static IObservable<T> Return<T>(T value)
		{
			return Observable.Create<T>(o =>
			{
				o.OnNext(value);
				o.OnCompleted();
				return Disposable.Empty;
			});
		}
		public static IObservable<T> Never<T>()
		{
			return Observable.Create<T>(o =>
			{
				return Disposable.Empty;
			});
		}
		public static IObservable<T> Throws<T>(Exception exception)
		{
			return Observable.Create<T>(o =>
			{
				o.OnError(exception);
				return Disposable.Empty;
			});
		}

	}
}
