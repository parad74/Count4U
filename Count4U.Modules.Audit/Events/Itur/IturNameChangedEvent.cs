using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
	public class IturNameChangedEvent : CompositePresentationEvent<IturNameChangedEventPayload>
    {
         
    }

    public class IturNameChangedEventPayload
    {
        public Iturs Iturs { get; set; }
		public string Name { get; set; }
		public string ERPCode { get; set; }
       // public Location Location { get; set; }
    }
}