using Count4U.Model;
using Microsoft.Practices.Prism.Events;
using Count4U.Model.Count4U;

namespace Count4U.Modules.Audit.Events
{
	public class IturPrefixChangeEvent : CompositePresentationEvent<IturPrefixChangeEventPayload>
    {
         
    }

	public class IturPrefixChangeEventPayload
    {
        public Iturs Iturs { get; set; }        
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
    }
}