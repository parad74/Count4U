using Count4U.Model;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
	public class ComplexAdapterRecountProductEvent : CompositePresentationEvent<ComplexAdapterRecountProductEventPayload>
    {
        
    }

	public class ComplexAdapterRecountProductEventPayload
    {
		string DBPath { get; set; }
    }
}