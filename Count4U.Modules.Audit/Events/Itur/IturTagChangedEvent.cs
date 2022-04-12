using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
	public class IturTagChangedEvent : CompositePresentationEvent<IturTagChangedEventPayload>
    {
         
    }

	public class IturTagChangedEventPayload
    {
        public Iturs Iturs { get; set; }
        //public Locations Locations { get; set; }
		public string Tag { get; set; }
		public bool AllChange { get; set; }
    }
}