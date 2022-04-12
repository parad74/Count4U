using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{

	public class UploadUnits : ObservableCollection<UploadUnit>
    {
		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ
		public static UploadUnits FromEnumerable(System.Collections.Generic.IEnumerable<UploadUnit> List)
        {
			UploadUnits suppliers = new UploadUnits();
			foreach (UploadUnit item in List)
            {
                suppliers.Add(item);
            }
            return suppliers;
        }

		public UploadUnit CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
