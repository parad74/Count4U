using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
	public class LocationTagChangedEvent : CompositePresentationEvent<LocationTagChangedEventPayload>
    {
         
    }

	public class LocationTagChangedEventPayload
    {
        public Iturs Iturs { get; set; }
        public Locations Locations { get; set; }
		public string Tag { get; set; }
		public bool AllChange { get; set; }
    }
}