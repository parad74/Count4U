using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
    public class IturStatusChangeEvent : CompositePresentationEvent<IturStatusChangeEventPayload>
    {
         
    }

    public class IturStatusChangeEventPayload
    {
        public Iturs Iturs { get; set; }
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
    }
}