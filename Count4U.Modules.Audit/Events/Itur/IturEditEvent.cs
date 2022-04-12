using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
    public class IturEditEvent : CompositePresentationEvent<IturEditEventPayload>
    {
    }

    public class IturEditEventPayload
    {
        public Itur Itur { get; set; }
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
    }
}