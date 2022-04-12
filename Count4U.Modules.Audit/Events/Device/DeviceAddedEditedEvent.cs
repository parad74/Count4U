using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
	public class DeviceAddedEditedEvent : CompositePresentationEvent<DeviceAddedEditedEventPayload>
    {

      
    }

	public class DeviceAddedEditedEventPayload
    {
        public bool IsNew { get; set; }
		public Device Device { get; set; }
    } 
}