using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model
{
	public class SQLScripts : ObservableCollection<SQLScript>
    {
		public static SQLScripts FromEnumerable(IEnumerable<SQLScript> list)
        {
			var collection = new SQLScripts();
            return Fill(collection, list);
        }

		private static SQLScripts Fill(SQLScripts collection, IEnumerable<SQLScript> list)
        {
			foreach (SQLScript item in list)
                collection.Add(item);
            return collection;
        }

		public SQLScript CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
