using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events.Status
{
    public class StatusSelectedEvent : CompositePresentationEvent<StatusSelectedEventPayload>
    {
    }

    public class StatusSelectedEventPayload
    {
        public bool IsChecked { get; set; }
        public StatusIturs Statuses { get; set; }
    }
}