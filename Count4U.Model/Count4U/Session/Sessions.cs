using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
    public class Sessions : ObservableCollection<Session>
    {

        public static Sessions FromEnumerable(System.Collections.Generic.IEnumerable<Session> List)
        {
            Sessions sessions = new Sessions();
            foreach (Session item in List)
            {
                sessions.Add(item);
            }
            return sessions;
        }

        public Session CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
