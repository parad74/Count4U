using System.Collections.Generic;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Events;
using Count4U.Model.Count4U;

namespace Count4U.Modules.Audit.Events
{
	public class SectionSelectedEvent : CompositePresentationEvent<SectionSelectedEventPayload>
    {
    }

	public class SectionSelectedEventPayload
    {
        public bool IsChecked { get; set; }
		public Sections Sections { get; set; }   
    }
}
