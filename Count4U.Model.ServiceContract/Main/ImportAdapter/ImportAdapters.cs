using System.Collections.ObjectModel;
using Count4U.Model.Main;
using System.Collections.Generic;

namespace Count4U.Model.Main
{
   	public class ImportAdapters : ObservableCollection<ImportAdapter>
    {
		public static ImportAdapters FromEnumerable(IEnumerable<ImportAdapter> list)
		{
			var collection = new ImportAdapters();
			return Fill(collection, list);
		}

		public static ImportAdapters Fill(ImportAdapters collection, IEnumerable<ImportAdapter> list)
		{
			if (list == null) return collection;
			foreach (ImportAdapter item in list)
			{
				if (ImportAdapters.Containts(collection, item) == false)
				{
					collection.Add(item);
				}
			}
			return collection;
		}

		private static bool Containts(ImportAdapters collection, ImportAdapter importAdapter)
		{
			var ch = @" ".ToCharArray();
			string adapterCode = importAdapter.AdapterCode.ToLower().Trim(ch);

			foreach (ImportAdapter item in collection)
			{
				if (item.AdapterCode.ToLower().Trim(ch) == adapterCode)
				{
					return true;
				}
			}
			return false;
		}

		public ImportAdapters CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get;  set; }
    }
}

