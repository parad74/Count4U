using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Count4U.GenerationReport
{
    /// <summary>
	/// Collection class for Reports entitites.
    /// </summary>
	public class Reports : ObservableCollection<Report>
    {
		public static Reports FromEnumerable(IEnumerable<Report> list)
		{
			var collection = new Reports();
			return Fill(collection, list);
		}

		public static Reports Fill(Reports collection, IEnumerable<Report> list)
		{
			foreach (Report item in list)
			{
				if (item.FileName == "-")
				{
					collection.Add(item);
				}
				else if (Reports.Containts(collection, item) == false)
				{
					collection.Add(item);
				}
			}
			return collection;
		}

		public static bool Containts(Reports collection, Report report)
		{
			if (collection == null) return false;
			if (report == null) return false;
			var ch = @" \".ToCharArray();
			string reportFileName = report.FileName.ToLower().Trim(ch);
			if (string.IsNullOrWhiteSpace(reportFileName) == true) return false;
			string reportPath = report.Path.ToLower().Trim(ch);

			foreach (Report item in collection)
			{
				if ((item.Path.ToLower().Trim(ch)) == reportPath &&
					(item.FileName.ToLower().Trim(ch)) == reportFileName) 
				{
					return true;
				}
			}
			return false;
		}

		public Reports CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }

		//internal static Reports SortNN(Reports menuReports)
		//{
		//    return menuReports.OrderBy(e => e.NN).Select(e => e).ToList();
		//}
	}
}

