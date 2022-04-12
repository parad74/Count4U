using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
	public class IturPrefixChangedEvent : CompositePresentationEvent<IturPrefixChangedEventPayload>
    {
         
    }

	public class IturPrefixChangedEventPayload
    {
        public Iturs Iturs { get; set; }
        public string PrefixOld { get; set; }
		public string PrefixNew { get; set; }
		public bool AllChange { get; set; }
    }
}