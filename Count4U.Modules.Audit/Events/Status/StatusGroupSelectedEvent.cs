using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events.Status
{
    public class StatusGroupSelectedEvent : CompositePresentationEvent<StatusGroupSelectedEventPayload>
    {
         
    }

    public class StatusGroupSelectedEventPayload
    {
        public bool IsChecked { get; set; }
        public StatusIturGroups StatusGroups { get; set; }
    }
}