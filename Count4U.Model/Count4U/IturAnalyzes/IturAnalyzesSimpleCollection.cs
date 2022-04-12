using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class IturAnalyzesSimpleCollection : ObservableCollection<IturAnalyzesSimple>
    {
		public static IturAnalyzesSimpleCollection FromEnumerable(IEnumerable<IturAnalyzesSimple> list)
        {
			var collection = new IturAnalyzesSimpleCollection();
            return Fill(collection, list);
        }

		private static IturAnalyzesSimpleCollection Fill(IturAnalyzesSimpleCollection collection, IEnumerable<IturAnalyzesSimple> list)
        {
			foreach (IturAnalyzesSimple item in list)
                collection.Add(item);
            return collection;
        }

		public IturAnalyzesSimpleCollection CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
