using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Common.Events
{
    public class ItursAddedEvent : CompositePresentationEvent<ItursAddedEventPayload>
    {

    }

    public class ItursAddedEventPayload
    {
        public List<int> ItursThatExist { get; set; }
    }
}