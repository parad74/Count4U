using System;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter.Items
{
    [Serializable]
    public class MdiFilterItem
    {
        public bool IsOpen { get; set; }
        public string Title { get; set; }
        public string DashboardName { get; set; }
        public string RegionName { get; set; }
        public string ViewName { get; set; }
    }
}