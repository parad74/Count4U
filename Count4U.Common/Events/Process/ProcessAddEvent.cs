using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.ProcessC4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Common.Events
{
	public class ProcessAddEvent : CompositePresentationEvent<ProcessAddedEventPayLoad>
    {
         
    }

	public class ProcessAddedEventPayLoad
    {
		public Process Process { get; set; }
		public bool AddUnknownProcess { get; set; }
		//public CBIContext Context { get; set; }
		//public string DbContext { get; set; }
    }
}