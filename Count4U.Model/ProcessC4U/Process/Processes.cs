using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.ProcessC4U
{

	public class Processes : ObservableCollection<Process>
    {
		public static Processes FromEnumerable(System.Collections.Generic.IEnumerable<Process> List)
        {
			Processes processes = new Processes();
			foreach (Process item in List)
            {
                processes.Add(item);
            }
            return processes;
        }

		public Process CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
