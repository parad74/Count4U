using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Common.Events
{
	public class LocationMultiAddEvent : CompositePresentationEvent<LocationMultiAddedEventPayLoad>
    {
         
    }

	public class LocationMultiAddedEventPayLoad
    {
        public Location Location { get; set; }
		public int CountAdd { get; set; }
        public bool AddUnknownLocation { get; set; }
        public CBIContext Context { get; set; }
        public string DbContext { get; set; }
    }
}