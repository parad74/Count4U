using System;
using Count4U.Model.Interface.Count4U;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Count4U.Model.Interface;
using Codeplex.Reactive;
using Codeplex.Reactive.Notifiers;
using Codeplex.Reactive.Asynchronous; // namespace for Asynchronous Extension Methods
using Codeplex.Reactive.Extensions;
using System.Collections.Generic;   // namespace for Extensions(OnErroRetry etc...)
using System.Linq;

namespace Count4U.Model.Count4U
{
	public static class ResulteValueTokenReactiveProperty
	{
		//public static ReactiveProperty<string> SumQuantityEditByDocumentCode { get;/* private set; */}
		private static ReactiveProperty<string> _sumQuantityEditByDocumentCode;

		public static ReactiveProperty<string> SumQuantityEditByDocumentCode
		{
			get{
			//IObservable<CountChangedStatus>   connect
			//var SumQuantityEditByDocumentCode = connect
			//        .Select(x => (x != CountChangedStatus.Empty) ? "loading..." : "complete")
			//        .ToReactiveProperty();
				//ResulteValueTokenObserver<string> str = new ResulteValueTokenObserver<string>();
				//str.OnNext("12");
				List<string> LocationItems = new List<string>();
				LocationItems.Add("12");
				IObservable<string> val = LocationItems.ToObservable();
				//var val = LocationItems as IEnumerable<string>;
				_sumQuantityEditByDocumentCode = val.ToReactiveProperty(); 
				//var sum = val.Select(x => x).ToReactiveProperty();

				return _sumQuantityEditByDocumentCode;
			}
		}
	}
}
