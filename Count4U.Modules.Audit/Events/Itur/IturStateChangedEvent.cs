using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
    public class IturStateChangedEvent : CompositePresentationEvent<ItursStateChangedEventPayload>
    {
         
    }  

    public class ItursStateChangedEventPayload
    {
        public Iturs Iturs { get; set; }
        public bool Disabled { get; set; }
    }
}