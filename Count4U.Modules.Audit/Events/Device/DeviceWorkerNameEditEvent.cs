using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
	public class DeviceWorkerNameEditEvent : CompositePresentationEvent<DeviceWorkerNameEditEventPayload>
    {
         
    }

	public class DeviceWorkerNameEditEventPayload
	{
		//public Device Device { get; set; }
   		public string OldWorkerName { get; set; }
		public string NewWorkerName { get; set; }
		public string DeviceID { get; set; }

	}
}