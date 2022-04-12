using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
    public class IturStateChangeEvent : CompositePresentationEvent<ItursStateChangeEventPayload>
    {
         
    }   

    public class ItursStateChangeEventPayload
    {
        public Iturs Iturs { get; set; }
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
    }
}