using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
	public class SectionTagChangedEvent : CompositePresentationEvent<SectionTagChangedEventPayload>
    {
         
    }

	public class SectionTagChangedEventPayload
    {
		public Sections Sections { get; set; }
		public string Tag { get; set; }
		public bool AllChange { get; set; }
    }
}