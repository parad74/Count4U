using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Common.Events
{
    public class IturAddEvent : CompositePresentationEvent<IturAddEventPayload>
    {

    }

    public class IturAddEventPayload
    {
        public Itur Itur { get; set; }
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
    }
}