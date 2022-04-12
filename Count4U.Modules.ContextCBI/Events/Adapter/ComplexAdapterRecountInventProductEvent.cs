using Count4U.Model;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.ContextCBI.Events
{
	public class ComplexAdapterRecountInventProductEvent : CompositePresentationEvent<ComplexAdapterRecountInventProductEventPayload>
    {
        
    }

	public class ComplexAdapterRecountInventProductEventPayload
    {
		string DBPath { get; set; }
    }
}