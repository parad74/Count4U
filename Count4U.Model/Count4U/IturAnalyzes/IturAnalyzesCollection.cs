using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.MappingEF;

namespace Count4U.Model.Count4U
{
	public class IturAnalyzesCollection : ObservableCollection<IturAnalyzes>
    {
		public static IturAnalyzesCollection FromEnumerable(IEnumerable<IturAnalyzes> list)
        {
			//var collection1 = new ObservableCollection<IturAnalyzes>(list);
			//IturAnalyzesCollection collection = (IturAnalyzesCollection)collection1;
			//return collection; 
			var collection = new IturAnalyzesCollection();
			return Fill(collection, list);
        }

		private static IturAnalyzesCollection Fill(IturAnalyzesCollection collection, IEnumerable<IturAnalyzes> list)
        {
			foreach (IturAnalyzes item in list)
                collection.Add(item);
            return collection;
        }

		public static IturAnalyzesCollection FromEntityList(List<App_Data.IturAnalyzes> list)
		{
			//var collection = new IturAnalyzesCollection();
			//foreach (var item in list)	collection.Add(item.ToDomainObject());
			//return collection;

			var collection = new IturAnalyzesCollection();
			foreach (var item in list)	collection.Add(item.ToDomainObject());
			return collection;
		}

		public IturAnalyzesCollection CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
		public double SumQuantityEdit { get;  set; }
    }
}
