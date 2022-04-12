using System.Collections.ObjectModel;

namespace Count4U.Model
{
    /// <summary>
    /// Collection class for Branches entitites.
    /// </summary>
	public class ProcessJobs : ObservableCollection<ProcessJob>
    {

		public static ProcessJobs FromEnumerable(System.Collections.Generic.IEnumerable<ProcessJob> List)
        {
			ProcessJobs processJobs = new ProcessJobs();
			foreach (ProcessJob item in List)
            {
                processJobs.Add(item);
            }
            return processJobs;
        }

		public ProcessJob CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
