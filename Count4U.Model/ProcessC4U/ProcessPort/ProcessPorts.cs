using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.ProcessC4U
{

	public class ProcessPorts : ObservableCollection<ProcessPort>
    {
		public static ProcessPorts FromEnumerable(System.Collections.Generic.IEnumerable<ProcessPort> List)
        {
			ProcessPorts ports = new ProcessPorts();
			foreach (ProcessPort item in List)
            {
                ports.Add(item);
            }
            return ports;
        }

		public ProcessPort CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
