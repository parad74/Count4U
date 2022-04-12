using System;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace Count4U.Model.Count4U
{
	public class ResulteValueTokenObserver<T> : IObserver<T>
	{
		//ILog _log;
		public ResulteValueTokenObserver(/*ILog log*/)
		{
			//this._log = log;
		}

		public void OnNext(T value)
		{
			Observable.Return(value);
		}

		public void OnCompleted()
		{

		}

		public void OnError(Exception error)
		{
			//_log.Add(MessageTypeEnum.Error, error.Message);
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

/*
 namespace SimpleRX
{
    public class Observer<T> : IObserver<T>
    {
        private Action<T> OnNext_ = p => { };
        private Action OnCompleted_ = () => { };
        private Action<Exception> OnError_ = ex => { };

        public Observer(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.OnNext_ = onNext;
            this.OnError_ = onError;
            this.OnCompleted_ = onCompleted;
        }

        public Observer(Action<T> onNext, Action<Exception> onError)
        {
            this.OnNext_ = onNext;
            this.OnError_ = onError;
        }

        public Observer(Action<T> onNext)
        {
            this.OnNext_ = onNext;
        }

        public void OnCompleted()
        {
            OnCompleted_();
        }

        public void OnError(Exception error)
        {
            OnError_(error);
        }

        public void OnNext(T value)
        {
            OnNext_(value);
        }
    }
}
*/