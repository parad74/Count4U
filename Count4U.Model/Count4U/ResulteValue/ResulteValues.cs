using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
	public class ResulteValues : ObservableCollection<ResulteValue>
    {

		public static ResulteValues FromEnumerable(System.Collections.Generic.IEnumerable<ResulteValue> List)
        {
			ResulteValues resulteValues = new ResulteValues();
			foreach (ResulteValue item in List)
            {
				resulteValues.Add(item);
            }
			return resulteValues;
        }

		public ResulteValue CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
