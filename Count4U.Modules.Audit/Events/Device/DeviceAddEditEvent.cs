using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Events;

namespace Count4U.Modules.Audit.Events
{
	public class DeviceAddEditEvent : CompositePresentationEvent<DeviceAddEditEventPayload>
    {
         
    }

	public class DeviceAddEditEventPayload
	{
		public Device Device { get; set; }
		public CBIContext Context { get; set; }
		public string DbContext { get; set; }
		public string PeriodFromInventorDate { get; set; }
		public string PeriodFromStartDate { get; set; }
		public double QuentetyEdit { get; set; }
		

	}
}