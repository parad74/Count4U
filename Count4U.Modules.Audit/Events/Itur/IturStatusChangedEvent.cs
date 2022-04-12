using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
    public class IturStatusChangedEvent : CompositePresentationEvent<IturStatusChangedEventPayload>
    {
       
    }

    public class IturStatusChangedEventPayload
    {
        public Iturs Iturs { get; set; }
        public StatusItur Status { get; set; }
    } 
}