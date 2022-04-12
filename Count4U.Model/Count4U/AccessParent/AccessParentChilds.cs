using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{

	public class AccessParentChilds : ObservableCollection<AccessParentChild>
	{

		public static AccessParentChilds FromEnumerable(System.Collections.Generic.IEnumerable<AccessParentChild> List)
		{
			AccessParentChilds accessParentChilds = new AccessParentChilds();
			foreach (AccessParentChild item in List)
			{
				accessParentChilds.Add(item);
			}
			return accessParentChilds;
		}

		public AccessParentChild CurrentItem { get; set; }

		public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}

}
