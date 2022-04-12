using Count4U.Model;
using Microsoft.Practices.Prism.Events;
using Count4U.Model.Count4U;

namespace Count4U.Modules.Audit.Events
{
	public class LocationTagChangeEvent : CompositePresentationEvent<LocationTagChangeEventPayload>
    {
         
    }

	public class LocationTagChangeEventPayload
    {
        public Iturs Iturs { get; set; }  
      	public Locations Locations { get; set; }   
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
		//public bool IsChecked { get; set; }
		//public bool AllChange { get; set; }
    }
}