using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
    public class IturLocationChangedEvent : CompositePresentationEvent<IturLocationChangedEventPayload>
    {
         
    }

    public class IturLocationChangedEventPayload
    {
        public Iturs Iturs { get; set; }
        public Location Location { get; set; }
		public bool AllChange { get; set; }
    }
}