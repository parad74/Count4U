using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Common.Events
{
    public class LocationAddEvent : CompositePresentationEvent<LocationAddedEventPayLoad>
    {
         
    }

    public class LocationAddedEventPayLoad
    {
        public Location Location { get; set; }
        public bool AddUnknownLocation { get; set; }
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
    }
}