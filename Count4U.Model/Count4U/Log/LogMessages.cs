using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class LogMessages : ObservableCollection<LogMessage>
    {
		public static LogMessages FromEnumerable(IEnumerable<LogMessage> list)
        {
			var collection = new LogMessages();
            return Fill(collection, list);
        }

		private static LogMessages Fill(LogMessages collection, IEnumerable<LogMessage> list)
        {
			foreach (LogMessage item in list)
			{
				collection.Add(item);
			}
            return collection;
        }

		public LogMessages CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
