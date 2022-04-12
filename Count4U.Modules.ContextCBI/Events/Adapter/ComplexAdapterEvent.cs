using Count4U.Model;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
	public class ComplexAdapterEvent : CompositePresentationEvent<ComplexAdapterEventPayload>
    {
        
    }

	public class ComplexAdapterEventPayload
    {
		public FromContext FromContext { get; set; }
		public ToContext ToContext { get; set; }
    }
}