using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class BuildingConfigs : ObservableCollection<BuildingConfig>
	{
		public static BuildingConfigs FromEnumerable(IEnumerable<BuildingConfig> list)
		{
			var collection = new BuildingConfigs();
			return Fill(collection, list);
		}

		private static BuildingConfigs Fill(BuildingConfigs collection, IEnumerable<BuildingConfig> list)
		{
			foreach (BuildingConfig item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public BuildingConfig CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

